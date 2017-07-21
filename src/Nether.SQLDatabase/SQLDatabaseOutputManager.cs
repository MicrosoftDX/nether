// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Nether.Ingest;
using System.Data.SqlClient;
using System.Data;
using System.Text.RegularExpressions;

namespace Nether.SQLDatabase
{
    /// <summary>
    /// SQLDatabaseOutputManager allows to output messages to Azure SQL Database. For testing purposes you can enable autoCreateTablesAndStoredProcedures
    /// parameter in the constructor, it will generate dynamic SQL to auto-create table and stored procedure for insert operations in the database.
    /// Auto-create uses input data validation to protect from SQL Injection (only alphanumeric characters as well as '-' and '_' are allowed 
    /// for table/type and field/column names). Please disable auto-create for production to avoid even minimal SQL Injection risk. 
    /// SQLDatabaseOutputManager uses Type parameter of the incoming message as destination table name as well as field names as column names to output all the data.
    /// By default all the types are strings(nvarchar(50) in the database) you can override this behavoir for any field by providing columnMapping Dictionary
    /// in the format of 'field/column name', SqlDbType, dimension.You need to only provide field for which you'd like to have different datatype than string.
    /// </summary>
    public class SQLDatabaseOutputManager : IOutputManager
    {
        //TODO: Write documentation snippet on how to use SQL Database Output Manager
        private string _sqlConnectionString;
        public bool _autoCreateTablesAndStoredProcedures = false;
        public Dictionary<string, Tuple<SqlDbType, int>> _columnMapping; //mapping specific json field names to target datatypes

        public SQLDatabaseOutputManager(string sqlConnectionString, bool autoCreateTablesAndStoredProcedures = false)
        {
            _sqlConnectionString = sqlConnectionString;
            _autoCreateTablesAndStoredProcedures = autoCreateTablesAndStoredProcedures;
        }


        /// <summary>
        /// In column mapping discionary every mapping is presented in a way: field, SQL Database type to map, dimension (i.e. number of characters for varchar)
        /// </summary>
        public SQLDatabaseOutputManager(string sqlConnectionString, Dictionary<string, Tuple<SqlDbType, int>> columnMapping, bool autoCreateTablesAndStoredProcedures = true)
        {
            _sqlConnectionString = sqlConnectionString;
            _autoCreateTablesAndStoredProcedures = autoCreateTablesAndStoredProcedures;
            _columnMapping = columnMapping;
        }

        Task IOutputManager.FlushAsync(string partitionId)
        {
            return Task.CompletedTask; //Flushing mechanism for SQL Database is not supported in the current implementation
        }

        Task IOutputManager.OutputMessageAsync(string partitionId, string pipelineName, int index, Message msg)
        {
            if (msg != null)
            {
                using (SqlConnection sqlConnection = new SqlConnection(_sqlConnectionString))
                {
                    try
                    {
                        InsertUsingStoredProcedure(msg, sqlConnection);
                        return Task.CompletedTask;
                    }
                    catch (Exception)
                    {
                        //check whether the table is present in the database

                        if (CheckIfTableExist(msg, sqlConnection))
                        {
                            if (CheckIfSPExist(msg, sqlConnection))
                                throw new Exception("Both Table and stored procedure are present in the database but couldn't insert new rows. Please check that columns in the existing table and stored procedure are identical to the message data including data types of columns");
                            else
                            {
                                if (_autoCreateTablesAndStoredProcedures)
                                {
                                    CreateStoredProcedureInDatabase(msg, sqlConnection);
                                    InsertUsingStoredProcedure(msg, sqlConnection);
                                }
                                else
                                    throw new Exception("Table is present in the database but stored procedure for insert is missing. Please either create stored procedure for insert operation or enable auto create parameter in class constructor");
                            }
                        }
                        else
                        {
                            if (_autoCreateTablesAndStoredProcedures)
                            {
                                CreateTableAndSPInDatabase(msg, sqlConnection);
                                InsertUsingStoredProcedure(msg, sqlConnection);
                            }
                            else
                                throw new Exception("Table is not present in the database and auto create is disabled. Please either enable auto create tables and stored procedures parameter in class constructor or manually create table in database with column data types identical to source message");
                        }
                    }
                }
            };
            return Task.CompletedTask;
        }

        private bool CheckIfTableExist(Message msg, SqlConnection sqlConnection)
        {
            using (SqlCommand sqlCommand = new SqlCommand("select case when exists((select * from information_schema.tables where table_name = @table_name)) then 1 else 0 end", sqlConnection))
            {
                sqlCommand.Parameters.AddWithValue("@table_name", msg.Type);

                if (sqlConnection.State == ConnectionState.Closed)
                    sqlConnection.Open();

                if ((int)sqlCommand.ExecuteScalar() == 1)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        private void CreateTableAndSPInDatabase(Message msg, SqlConnection sqlConnection)
        {
            // SQL Injection protection
            CheckValidNoSQLInjectionRisk(msg.Type);

            StringBuilder createTableStatement = new StringBuilder($"CREATE TABLE [dbo].[{msg.Type}] ([");
            StringBuilder createInsertSPStatement = new StringBuilder($"CREATE PROCEDURE [dbo].[sp_InsertInto{msg.Type}] ");

            foreach (string column in msg.Properties.Keys)
            {
                // SQL Injection protection
                CheckValidNoSQLInjectionRisk(column);

                createTableStatement.Append(column);
                createInsertSPStatement.Append($"@{column}");

                if (_columnMapping != null)
                {
                    if (_columnMapping.ContainsKey(column))
                    {
                        createTableStatement.Append($"] [{GetSqlServerTypeName(_columnMapping[column].Item1, _columnMapping[column].Item2)}] NULL");
                        createInsertSPStatement.Append($" {GetSqlServerTypeName(_columnMapping[column].Item1, _columnMapping[column].Item2)} NULL");
                    }
                    else
                    {
                        createTableStatement.Append("] [nvarchar] (50) NULL");
                        createInsertSPStatement.Append(" nvarchar(50) NULL");
                    }
                }
                else
                {
                    createTableStatement.Append("] [nvarchar] (50) NULL");
                    createInsertSPStatement.Append(" nvarchar(50) NULL");
                }
                createTableStatement.Append(",[");
                createInsertSPStatement.Append(",");
            }
            createTableStatement.Remove(createTableStatement.Length - 2, 2);
            createInsertSPStatement.Remove(createInsertSPStatement.Length - 1, 1);

            createTableStatement.Append(")");
            createInsertSPStatement.Append($" AS {ConstructParametrizedInsertStatement(msg)}");

            using (SqlCommand sqlCommand = new SqlCommand(createTableStatement.ToString(), sqlConnection))
            {
                if (sqlConnection.State == ConnectionState.Closed)
                    sqlConnection.Open();

                int result = sqlCommand.ExecuteNonQuery();

                if (!CheckIfTableExist(msg, sqlConnection))
                {
                    throw new Exception("Failed to create table in the database.");
                }

                sqlCommand.CommandText = createInsertSPStatement.ToString();

                if (sqlConnection.State == ConnectionState.Closed)
                    sqlConnection.Open();
                try
                {
                    result = sqlCommand.ExecuteNonQuery();
                }
                catch (Exception)
                {
                    if (CheckIfSPExist(msg, sqlConnection)) return;
                }
                if (!CheckIfSPExist(msg, sqlConnection))
                {
                    throw new Exception("Failed to create stored procedure in the database.");
                }
            }
        }

        private void CreateStoredProcedureInDatabase(Message msg, SqlConnection sqlConnection)
        {
            // SQL Injection protection
            CheckValidNoSQLInjectionRisk(msg.Type);

            StringBuilder createInsertSPStatement = new StringBuilder($"CREATE PROCEDURE [dbo].[sp_InsertInto{msg.Type}] ");

            foreach (string column in msg.Properties.Keys)
            {
                // SQL Injection protection
                CheckValidNoSQLInjectionRisk(column);

                createInsertSPStatement.Append($"@{column}");

                if (_columnMapping != null)
                {
                    if (_columnMapping.ContainsKey(column))
                    {
                        createInsertSPStatement.Append($" {GetSqlServerTypeName(_columnMapping[column].Item1, _columnMapping[column].Item2)} NULL");
                    }
                    else
                    {
                        createInsertSPStatement.Append(" nvarchar(50) NULL");
                    }
                }
                else
                {
                    createInsertSPStatement.Append(" nvarchar(50) NULL");
                }
                createInsertSPStatement.Append(",");
            }
            createInsertSPStatement.Remove(createInsertSPStatement.Length - 1, 1);

            createInsertSPStatement.Append($" AS {ConstructParametrizedInsertStatement(msg)}");

            using (SqlCommand sqlCommand = new SqlCommand(createInsertSPStatement.ToString(), sqlConnection))
            {
                if (sqlConnection.State == ConnectionState.Closed)
                    sqlConnection.Open();

                sqlCommand.ExecuteNonQuery();
                if (!CheckIfSPExist(msg, sqlConnection))
                {
                    throw new Exception("Failed to create stored procedure in the database.");
                }
            }
        }

        private bool CheckIfSPExist(Message msg, SqlConnection sqlConnection)
        {
            using (SqlCommand sqlCommand = new SqlCommand("select case when exists((select * from sys.procedures where Name = @sp_name)) then 1 else 0 end", sqlConnection))
            {
                sqlCommand.Parameters.AddWithValue("@sp_name", $"sp_InsertInto{msg.Type}");

                if (sqlConnection.State == ConnectionState.Closed)
                    sqlConnection.Open();

                if ((int)sqlCommand.ExecuteScalar() == 1)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }


        private void InsertUsingStoredProcedure(Message msg, SqlConnection sqlConnection)
        {
            // SQL Injection protection
            CheckValidNoSQLInjectionRisk(msg.Type);

            using (SqlCommand sqlCommand = new SqlCommand($"sp_InsertInto{msg.Type}", sqlConnection))
            {
                sqlCommand.CommandType = CommandType.StoredProcedure;

                foreach (string column in msg.Properties.Keys)
                {
                    if (_columnMapping != null)
                    {
                        if (_columnMapping.ContainsKey(column))
                        {
                            sqlCommand.Parameters.Add($"@{column.ToString()}", _columnMapping[column].Item1);
                            sqlCommand.Parameters[$"@{column.ToString()}"].Value = msg.Properties[column];
                        }
                        else
                            sqlCommand.Parameters.AddWithValue($"@{column.ToString()}", msg.Properties[column]);
                    }
                    else
                        sqlCommand.Parameters.AddWithValue($"@{column.ToString()}", msg.Properties[column]);
                }

                if (sqlConnection.State == ConnectionState.Closed)
                    sqlConnection.Open();

                int result = sqlCommand.ExecuteNonQuery();

                if (result < 0)
                {
                    throw new Exception("Calling stored procedue for insert operation failed. Either existing stored procedure has different fields or the stored procedure itself is missing from the database - please enable autocreate or create stored procedure and table manually");
                }
            }
        }

        private void InsertUsingParameterizedQuery(Message msg, SqlConnection sqlConnection)
        {
            StringBuilder insertStatement = ConstructParametrizedInsertStatement(msg);

            using (SqlCommand sqlCommand = new SqlCommand(insertStatement.ToString(), sqlConnection))
            {
                foreach (string column in msg.Properties.Keys)
                {
                    if (_columnMapping != null)
                    {
                        if (_columnMapping.ContainsKey(column))
                        {
                            sqlCommand.Parameters.Add($"@{column.ToString()}", _columnMapping[column].Item1);
                            sqlCommand.Parameters[$"@{column.ToString()}"].Value = msg.Properties[column];
                        }
                        else
                            sqlCommand.Parameters.AddWithValue($"@{column.ToString()}", msg.Properties[column]);
                    }
                    else
                        sqlCommand.Parameters.AddWithValue($"@{column.ToString()}", msg.Properties[column]);
                }

                if (sqlConnection.State == ConnectionState.Closed)
                    sqlConnection.Open();

                int result = sqlCommand.ExecuteNonQuery();

                if (result < 0)
                {
                    throw new Exception("Insert operation failed. Either existing table has different fields or the table is missing from database - enable autocreate or create table manually");
                }
            }
        }

        private static StringBuilder ConstructParametrizedInsertStatement(Message msg)
        {
            // SQL Injection protection
            CheckValidNoSQLInjectionRisk(msg.Type);

            StringBuilder insertStatement = new StringBuilder("INSERT INTO dbo.[");
            insertStatement.Append(msg.Type);
            insertStatement.Append("] (");
            foreach (string column in msg.Properties.Keys)
            {
                // SQL Injection protection
                CheckValidNoSQLInjectionRisk(column);

                insertStatement.Append(column);
                insertStatement.Append(",");
            }
            insertStatement.Remove(insertStatement.Length - 1, 1);

            insertStatement.Append(") VALUES (@");

            foreach (string column in msg.Properties.Keys)
            {
                insertStatement.Append(column);
                insertStatement.Append(",@");
            }

            insertStatement.Remove(insertStatement.Length - 2, 2);
            insertStatement.Append(")");
            return insertStatement;
        }

        public static string GetSqlServerTypeName(SqlDbType dbType, int size)
        {
            if (size > 0)
                return $"{Enum.GetName(typeof(SqlDbType), dbType)} ({size})";

            else
                return Enum.GetName(typeof(SqlDbType), dbType);
        }


        /// <summary>
        /// check for SQL Injection attacks or invalid input. 
        /// The only allowed values for table name/type are alphanumeric characters plus "-" and "_" symbols after the first aplhanumeric character, no spaces allowed. 
        /// Written accoring recomendation of dynamic SQL protection from SQL Injection - https://msdn.microsoft.com/en-us/library/ms161953(SQL.105).aspx
        /// </summary>
        /// <param name="inputString"></param>
        public static void CheckValidNoSQLInjectionRisk(string inputString)
        {
            Regex r = new Regex("^[a-zA-Z0-9][a-zA-Z0-9_-]*$");
            if (!r.IsMatch(inputString))
                throw new Exception("Invalid type/table name. Only alphanumeric characters without spaces allowed as well as \"_\" and \"-\" symbols after the first alphanumeric character. ");
        }
    }
}

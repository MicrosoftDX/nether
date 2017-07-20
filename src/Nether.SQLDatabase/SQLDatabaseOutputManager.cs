// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Nether.Ingest;
using System.Data.SqlClient;
using Newtonsoft.Json;
using System.Data;

namespace Nether.SQLDatabase
{
    public class SQLDatabaseOutputManager : IOutputManager
    {
        private string _sqlConnectionString;
        public bool _autoCreateTables = false;
        public Dictionary<string, Tuple<SqlDbType, int>> _columnMapping; //mapping specific json field names to target datatypes
        
        public SQLDatabaseOutputManager(string sqlConnectionString, bool autoCreateTables = false)
        {
            _sqlConnectionString = sqlConnectionString;
            _autoCreateTables = autoCreateTables;
        }

        //In column mapping discionary every mapping is presented in a way: field, SQL Database type to map, dimension (i.e. number of characters for varchar)
        public SQLDatabaseOutputManager(string sqlConnectionString, Dictionary<string, Tuple<SqlDbType, int>> columnMapping, bool autoCreateTables = true)
        {
            _sqlConnectionString = sqlConnectionString;
            _autoCreateTables = autoCreateTables;
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
                        InsertIntoSQLDatabase(msg, sqlConnection);
                        return Task.CompletedTask;
                    }
                    catch (Exception)
                    {
                        //check whether the table is present in the database

                        if (CheckIfTableExist(msg, sqlConnection))
                        {
                            throw new Exception("Table is present in the database but couldn't insert new rows. Please check that columns in the existing table are identical to the message data including data types of columns");
                        }
                        else
                        {
                            if (_autoCreateTables)
                            {
                                CreateTableInDatabase(msg, sqlConnection);
                                InsertIntoSQLDatabase(msg, sqlConnection);
                            }
                            else
                                throw new Exception("Table is not present in the database and auto create is disabled. Please either enable auto create tables parameter in class constructor or manually create table in database with column data types identical to source message");
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

                if (sqlConnection.State == System.Data.ConnectionState.Closed)
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

        private void CreateTableInDatabase(Message msg, SqlConnection sqlConnection)
        {
            //TODO - SQL Injection threat!!! convert to stored procedure before production
            StringBuilder createStatement = new StringBuilder($"CREATE TABLE [dbo].[{msg.Type}] ([");

            foreach (string column in msg.Properties.Keys)
            {
                createStatement.Append(column);
                if (_columnMapping != null)
                {
                    if (_columnMapping.ContainsKey(column))
                    {
                        createStatement.Append($"] [{GetSqlServerTypeName(_columnMapping[column].Item1, _columnMapping[column].Item2)}] NULL");
                        //switch (_columnMapping[column])
                        //{
                        //    case SqlDbType.
                        //}                        
                    }
                    else
                        createStatement.Append("] [nvarchar] (50) NULL");
                }
                else
                    createStatement.Append("] [nvarchar] (50) NULL");
                createStatement.Append(",[");
            }
            createStatement.Remove(createStatement.Length - 2, 2);
            createStatement.Append(")");

            using (SqlCommand sqlCommand = new SqlCommand(createStatement.ToString(), sqlConnection))
            {

                if (sqlConnection.State == System.Data.ConnectionState.Closed)
                    sqlConnection.Open();

                int result = sqlCommand.ExecuteNonQuery();

                if (!CheckIfTableExist(msg, sqlConnection))
                {
                    throw new Exception("Failed to create table in database.");
                }
            }
        }

        private void InsertIntoSQLDatabase(Message msg, SqlConnection sqlConnection)
        {
            StringBuilder insertStatement = new StringBuilder("INSERT INTO dbo.[");
            insertStatement.Append(msg.Type);       
            insertStatement.Append("] (");
            foreach (string column in msg.Properties.Keys)
            {
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
                            sqlCommand.Parameters.AddWithValue("@" + column.ToString(), msg.Properties[column]);
                    }
                    else
                        sqlCommand.Parameters.AddWithValue("@" + column.ToString(), msg.Properties[column]);

                }

                if (sqlConnection.State == System.Data.ConnectionState.Closed)
                    sqlConnection.Open();

                int result = sqlCommand.ExecuteNonQuery();

                if (result < 0)
                {
                    throw new Exception("Insert operation failed. Either existing table has different fields, either the table is missing from database - enable autocreate or create table manually");
                }
            }
        }
        public static string GetSqlServerTypeName(SqlDbType dbType, int size)
        {
            if (size > 0)
                return $"{Enum.GetName(typeof(SqlDbType), dbType)} ({size})";

            else
                return Enum.GetName(typeof(SqlDbType), dbType);

        }
    }
}

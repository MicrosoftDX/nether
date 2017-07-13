using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Nether.Ingest;
using System.Data.SqlClient;
using Newtonsoft.Json;


namespace Nether.SQLDatabase
{
    public class SQLDatabaseOutputManager : IOutputManager
    {
        private string _sqlConnectionString;
        //private SqlConnection _sqlConnection;

        public SQLDatabaseOutputManager(string sqlConnectionString)
        {
            _sqlConnectionString = sqlConnectionString;
            //   _sqlConnection = new SqlConnection(_sqlConnectionString);
        }

        Task IOutputManager.FlushAsync(string partitionId)
        {
            return Task.CompletedTask; //Flushing mechanism for SQL Database is not supported in current implementation
        }

        Task IOutputManager.OutputMessageAsync(string partitionId, string pipelineName, int index, Message msg)
        {
            if (msg != null)
            {
                using (SqlConnection sqlConnection = new SqlConnection(_sqlConnectionString))
                {
                    try
                    {
                        StringBuilder insertStatement = new StringBuilder("INSERT INTO dbo.[");
                        insertStatement.Append(msg.Type);
                        //insertStatement.Append(" (Id,EnqueuedTimeUtc");
                        insertStatement.Append("] (");
                        foreach (string column in msg.Properties.Keys)
                        {
                            insertStatement.Append(column);
                            insertStatement.Append(",");
                        }
                        insertStatement.Remove(insertStatement.Length - 1, 1);

                        //insertStatement.Append(") VALUES (@id,@EnqueuedTimeUtc,");
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
                            // sqlCommand.Parameters.AddWithValue("@id", msg.Id);
                            // sqlCommand.Parameters.AddWithValue("@EnqueuedTimeUtc", msg.EnqueuedTimeUtc);

                            foreach (string column in msg.Properties.Keys)
                            {
                                sqlCommand.Parameters.AddWithValue("@" + column.ToString(), msg.Properties[column]);
                            }

                            if (sqlConnection.State == System.Data.ConnectionState.Closed)
                                sqlConnection.Open();

                            int result = sqlCommand.ExecuteNonQuery();

                            if (result < 0)// Check Error
                            {
                                ; //TODO: implement the right approach to failed insert
                            }
                        }





                        return Task.CompletedTask;
                    }
                    catch (Exception e)
                    {
                        //check that table is present in the database

                        using (SqlCommand sqlCommand = new SqlCommand("select case when exists((select * from information_schema.tables where table_name = '@table_name')) then 1 else 0 end", sqlConnection))
                        {
                            sqlCommand.Parameters.AddWithValue("@table_name", msg.Type);

                            if (sqlConnection.State == System.Data.ConnectionState.Closed)
                                sqlConnection.Open();

                            if ((int)sqlCommand.ExecuteScalar() == 1)
                            {
                                ;//table exist
                            }
                            else
                            {
                                //table doesn't exist, create new
                                StringBuilder createStatement = new StringBuilder("CREATE TABLE [dbo].[@table_name] ([@");

                               // sqlCommand.Parameters.AddWithValue("@table_name", msg.Type);
                                foreach (string column in msg.Properties.Keys)
                                {
                                    createStatement.Append(column);
                                    createStatement.Append("] [nvarchar] (50) NULL");
                                    createStatement.Append(",[@");

                                    sqlCommand.Parameters.AddWithValue("@" + column.ToString(), msg.Properties[column]);
                                }
                                createStatement.Remove(createStatement.Length - 3, 3);
                                createStatement.Append(")");

                                sqlCommand.CommandText = createStatement.ToString();

                                int result = sqlCommand.ExecuteNonQuery();

                                if (result < 0)// Check Error
                                {
                                    ; //TODO: implement the right approach to failes create table
                                }
                            }
                        }

                    }
                }
            };
            return Task.CompletedTask;
        }

    }
}

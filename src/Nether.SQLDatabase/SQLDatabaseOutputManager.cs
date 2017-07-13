using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Nether.Ingest;
using System.Data.SqlClient;
using Newtonsoft.Json;


namespace Nether.SQLDatabase
{
    class SQLDatabaseOutputManager : IOutputManager
    {
        private string _sqlConnectionString;
        private SqlConnection _sqlConnection;

        public SQLDatabaseOutputManager(string sqlConnectionString)
        {
            _sqlConnectionString = sqlConnectionString;
            _sqlConnection = new SqlConnection(_sqlConnectionString);
        }

        Task IOutputManager.FlushAsync(string partitionId)
        {
            return Task.CompletedTask; //Flushing mechanism for SQL Database is not supported in current implementation
        }

        Task IOutputManager.OutputMessageAsync(string partitionId, string pipelineName, int index, Message msg)
        {
            if (msg != null)
            {
                try
                {
                    StringBuilder insertStatement = new StringBuilder("INSERT INTO dbo.");
                    insertStatement.Append(msg.Type);
                    insertStatement.Append(" (Id,EnqueuedTimeUtc");
                    foreach (string column in msg.Properties.Keys)
                    {
                        insertStatement.Append(",");
                        insertStatement.Append(column);
                    }
                    insertStatement.Append(") VALUES (@id,@EnqueuedTimeUtc,");

                    foreach (string column in msg.Properties.Keys)
                    {
                        insertStatement.Append(",@");
                        insertStatement.Append(column);
                    }
                    insertStatement.Append(")");

                    using (SqlCommand sqlCommand = new SqlCommand(insertStatement.ToString(), _sqlConnection))
                    {
                        sqlCommand.Parameters.AddWithValue("@id", msg.Id);
                        sqlCommand.Parameters.AddWithValue("@EnqueuedTimeUtc", msg.EnqueuedTimeUtc);

                        foreach (string column in msg.Properties.Keys)
                        {
                            sqlCommand.Parameters.AddWithValue("@" + column.ToString(), msg.Properties[column]);
                        }

                        _sqlConnection.Open();
                        int result = sqlCommand.ExecuteNonQuery();

                        // Check Error
                        if (result < 0)
                        {
                            ; //TODO: implement the right approach to failed insert
                        }
                    }


                    return Task.CompletedTask;
                }
                catch (Exception e)
                {
                    ;
                }
            };
            return Task.CompletedTask;
        }

    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Transactions;
using System.Web;

namespace MsSqlDbLib
{
    public class MsSqlHelper : IDisposable
    {
        private SqlConnection _Connection = null;

        public MsSqlHelper()
        {
            //_Connection = new SqlConnection();
        }
        public void Dispose()
        {
            if (_Connection != null)
            {
                if (_Connection.State == ConnectionState.Open)
                    _Connection.Close();
            }
        }

        public int ExecuteNonQuery(string connectionString, string query, List<SqlParameter> paramList, CommandType cmdType = CommandType.Text )
        {
            int count = 0;
            try
            {
                using (_Connection = new SqlConnection(connectionString))
                using (SqlCommand command = _Connection.CreateCommand())
                {
                    command.CommandType = cmdType;
                    command.CommandText = query;
                    command.Parameters.AddRange(paramList.ToArray());

                    if (command.Connection.State != ConnectionState.Open)
                        command.Connection.Open();
                    count = command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return count;
        }

        public SqlDataReader ExecuteReader(string connectionString, string query, List<SqlParameter> paramList, CommandType cmdType = CommandType.Text, int timeOut = 30)
        {
            SqlDataReader reader = null;
            try
            {
                _Connection = new SqlConnection(connectionString);
                SqlCommand command = _Connection.CreateCommand();
                command.CommandType = cmdType;
                command.CommandText = query;
                command.Parameters.AddRange(paramList.ToArray());

                if (timeOut > 30 && timeOut <= 60 * 10)
                    command.CommandTimeout = timeOut;
                if (command.Connection.State != ConnectionState.Open)
                    command.Connection.Open();
                reader = command.ExecuteReader(CommandBehavior.CloseConnection); // CommandBehavior.CloseConnection
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return reader;
        }

        public DataSet ExecuteDataSet(string connectionString, string query, List<SqlParameter> paramList, CommandType cmdType = CommandType.Text, int timeOut = 30)
        {
            DataSet ds = null;
            try
            {
                using (_Connection = new SqlConnection(connectionString))
                using (SqlCommand command = _Connection.CreateCommand())
                {
                    command.CommandType = cmdType;
                    command.CommandText = query;
                    command.Parameters.AddRange(paramList.ToArray());

                    if (timeOut > 30 && timeOut <= 60 * 10)
                        command.CommandTimeout = timeOut;
                    if (command.Connection.State != ConnectionState.Open)
                        command.Connection.Open();
                    using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                    {
                        ds = new DataSet();
                        adapter.Fill(ds);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return ds;
        }

        public SqlDataReader ExecuteReaderByContext(string query, List<SqlParameter> paramList)
        {
            SqlDataReader reader = null;
            try
            {
                SqlConnection connection = new SqlConnection("context connection = true");
                SqlCommand command = connection.CreateCommand();
                command.CommandType = CommandType.Text;
                command.CommandText = query;
                command.Parameters.AddRange(paramList.ToArray());
                if (command.Connection.State != ConnectionState.Open)
                    command.Connection.Open();
                reader = command.ExecuteReader();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return reader;
        }
    }
}

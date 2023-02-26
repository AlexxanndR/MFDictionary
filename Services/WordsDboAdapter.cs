using MFDictionary.MVVM.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.IO;

namespace MFDictionary.Services
{
    internal class WordsDboAdapter
    {
        private const string dbName = "DictionaryDB";
        private string _connectionString;
        SqlConnection _sqlConnection;

        public WordsDboAdapter()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            _sqlConnection = new SqlConnection(_connectionString);

            if (isDatabaseExists() == false)
            {
                CreateDatabase();
                CreateTable();
                CreateInsertProcedure();
            }
        }

        private bool isDatabaseExists()
        {
            bool result = false;

            try
            {
                string connectionString = "Data Source=.\\SQLEXPRESS;Initial Catalog=;Integrated Security=true";
                SqlConnection sqlConnection = new SqlConnection(connectionString);
                string isDbCreatedQuery = String.Format("SELECT database_id FROM sys.databases WHERE Name = '{0}'", dbName);

                using (sqlConnection)
                {
                    using (SqlCommand sqlcommand = new SqlCommand(isDbCreatedQuery, sqlConnection))
                    {
                        sqlConnection.Open();

                        object resultObj = sqlcommand.ExecuteScalar();

                        int databaseID = 0;
                        if (resultObj != null)
                            int.TryParse(resultObj.ToString(), out databaseID);

                        _sqlConnection.Close();

                        result = (databaseID > 0);
                    }
                }
            }
            catch (Exception ex)
            {
                result = false;
            }

            return result;
        }

        private bool CreateDatabase()
        {
            string connectionString = "Data Source=.\\SQLEXPRESS;Initial Catalog=;Integrated Security=true";
            SqlConnection sqlConnection = new SqlConnection(connectionString);

            string[] files = { Path.Combine(AppDomain.CurrentDomain.BaseDirectory, dbName + ".mdf"),
                               Path.Combine(AppDomain.CurrentDomain.BaseDirectory, dbName + ".ldf")};

            string createDbQuery = String.Format("CREATE DATABASE DictionaryDB ON "
                + "PRIMARY (name=" + dbName + "_data, filename='{0}', "
                + "size=500MB, maxsize=5GB, filegrowth=10%)"
                + "LOG ON (name=" + dbName + "_log, filename='{1}', "
                + "size=100MB, maxsize=500MB, filegrowth=10%)", files[0], files[1]);

            try
            {
                using (sqlConnection)
                {
                    sqlConnection.Open();

                    SqlCommand sqlCommand = new SqlCommand(createDbQuery, sqlConnection);
                    sqlCommand.ExecuteNonQuery();
                }
            } 
            catch (Exception ex)
            {
                return false;
            }

            return true;
        }

        private bool CreateTable()
        {
            bool isCreated = false;

            _sqlConnection.Open();

            string createTableQuery = "CREATE TABLE Words" +
                "(Id INTEGER NOT NULL IDENTITY(1,1) PRIMARY KEY," +
                "Text NVARCHAR(50) NOT NULL, Translation NVARCHAR(50) NOT NULL, Example1 NVARCHAR(150), Example2 NVARCHAR(150), Example3 NVARCHAR(150))";

            try
            {
                SqlCommand createTableCommand = new SqlCommand(createTableQuery, _sqlConnection);
                createTableCommand.ExecuteNonQuery();
                isCreated = true;
            }
            catch (Exception ex)
            {
                isCreated = false;
            }
            finally
            {
                _sqlConnection.Close();
            }

            return isCreated;
        }

        private bool CreateInsertProcedure()
        {
            bool isCreated = false;

            string query =
              "CREATE PROCEDURE sp_InsertWord ( " +
              "@Text NVARCHAR(50)," +
              "@Translation NVARCHAR(50)," + 
              "@Example1 NVARCHAR(150)," +
              "@Example2 NVARCHAR(150)," + 
              "@Example3 NVARCHAR(150))" +
              "AS INSERT INTO Words(Text,Translation,Example1,Example2,Example3) Values(@Text,@Translation,@Example1,@Example2,@Example3)";

            SqlCommand sqlCommand = new SqlCommand(query, _sqlConnection);

            try
            {
                _sqlConnection.Open();
                sqlCommand.ExecuteNonQuery();
                isCreated = true;
            }
            catch (SqlException e)
            {
                isCreated = false;
            }
            finally
            {
                _sqlConnection.Close();
            }

            return isCreated;
        }

        public ObservableCollection<Word> GetAll()
        {
            _sqlConnection.Open();

            ObservableCollection<Word> words = new ObservableCollection<Word>();

            SqlCommand sqlCommand = new SqlCommand("SELECT * FROM Words", _sqlConnection);

            SqlDataReader reader = sqlCommand.ExecuteReader();
            while (reader.Read())
            {
                Word word = new Word();

                word.Id = (long)reader["Id"];
                word.Text = reader["Text"].ToString();
                word.Translation = reader["Translation"].ToString();
                word.Example1 = reader["Example1"]?.ToString() ?? String.Empty;
                word.Example2 = reader["Example2"]?.ToString() ?? String.Empty;
                word.Example3 = reader["Example3"]?.ToString() ?? String.Empty;

                words.Add(word);
            }

            _sqlConnection.Close();

            return words;
        }

        public void Insert(Word word)
        {
            _sqlConnection.Open();

            SqlCommand sqlCommand = new SqlCommand("sp_InsertWord", _sqlConnection);
            sqlCommand.CommandType = CommandType.StoredProcedure;

            sqlCommand.Parameters.AddWithValue("@text", word.Text);
            sqlCommand.Parameters.AddWithValue("@translation", word.Translation);
            sqlCommand.Parameters.AddWithValue("@example1", (object)word.Example1 ?? DBNull.Value);
            sqlCommand.Parameters.AddWithValue("@example2", (object)word.Example2 ?? DBNull.Value);
            sqlCommand.Parameters.AddWithValue("@example3", (object)word.Example3 ?? DBNull.Value);

            sqlCommand.ExecuteNonQuery();

            _sqlConnection.Close();
        }

        public void Delete(long id)
        {
            _sqlConnection.Open();

            SqlCommand sqlCommand = new SqlCommand("DELETE FROM Words WHERE Id = " + id, _sqlConnection);
            sqlCommand.ExecuteNonQuery();

            _sqlConnection.Close();
        }

        public long GetRecordsCount()
        {
            _sqlConnection.Open();

            SqlCommand sqlCommand = new SqlCommand("SELECT COUNT(*) FROM Words", _sqlConnection);
            long count = Convert.ToInt64(sqlCommand.ExecuteScalar());

            _sqlConnection.Close();

            return count; 
        }

        public List<Word> GetRandomWords(int wordsNum)
        {
            _sqlConnection.Open();

            List<Word> words = new List<Word>();

            SqlCommand sqlCommand = new SqlCommand("SELECT TOP " + wordsNum + " * FROM Words ORDER BY NEWID()", _sqlConnection);

            SqlDataReader reader = sqlCommand.ExecuteReader();
            while (reader.Read())
            {
                Word word = new Word();

                word.Id = (long)reader["Id"];
                word.Text = (string)reader["Text"];
                word.Translation = (string)reader["Translation"];
                word.Example1 = reader["Example1"]?.ToString() ?? String.Empty;
                word.Example2 = reader["Example2"]?.ToString() ?? String.Empty;
                word.Example3 = reader["Example3"]?.ToString() ?? String.Empty;

                words.Add(word);
            }

            _sqlConnection.Close();

            return words;
        }
    }
}

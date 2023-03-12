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
                "Text NVARCHAR(50) NOT NULL, Transcription NVARCHAR(50), Translation NVARCHAR(MAX) NOT NULL, Examples NVARCHAR(MAX), ExamplesTranslation NVARCHAR(MAX))";

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
             "@Transcription NVARCHAR(MAX)," +
             "@Translation NVARCHAR(MAX)," + 
             "@Examples NVARCHAR(MAX)," +
             "@ExamplesTranslation NVARCHAR(MAX))" +
             "AS INSERT INTO Words(Text,Transcription,Translation,Examples,ExamplesTranslation) Values(@Text,@Transcription,@Translation,@Examples,@ExamplesTranslation)";

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

        public async Task<ObservableCollection<Word>> GetAllAsync()
        {
            ObservableCollection<Word> words = new ObservableCollection<Word>();

            await Task.Run(() =>
            {
                _sqlConnection.Open();

                SqlCommand sqlCommand = new SqlCommand("SELECT * FROM Words", _sqlConnection);

                SqlDataReader reader = sqlCommand.ExecuteReader();
                while (reader.Read())
                {
                    Word word = new Word();

                    word.Id = (int)reader["Id"];
                    word.Text = reader["Text"].ToString();
                    word.Transcription = reader["Transcription"].ToString() ?? String.Empty;
                    word.Translation = reader["Translation"].ToString().Split(' ').ToList();
                    word.Examples = reader["Examples"]?.ToString().Split(' ').ToList();
                    word.ExamplesTranslation = reader["ExamplesTranslation"]?.ToString().Split(' ').ToList();

                    words.Add(word);
                }

                _sqlConnection.Close();
            });

            return words;
        }

        public void Insert(Word word)
        {
            _sqlConnection.Open();

            SqlCommand sqlCommand = new SqlCommand("sp_InsertWord", _sqlConnection);
            sqlCommand.CommandType = CommandType.StoredProcedure;

            sqlCommand.Parameters.AddWithValue("@Text", word.Text);
            sqlCommand.Parameters.AddWithValue("@Transcription", (object)word.Transcription ?? DBNull.Value);

            string translation = String.Join(" ", word.Translation);
            sqlCommand.Parameters.AddWithValue("@Translation", translation);
            string examples = word.Examples != null ? String.Join(" ", word.Examples) : String.Empty;
            sqlCommand.Parameters.AddWithValue("@Examples", (object)examples ?? DBNull.Value);
            string examplesTranslation = word.ExamplesTranslation != null ? String.Join(" ", word.ExamplesTranslation) : String.Empty;
            sqlCommand.Parameters.AddWithValue("@ExamplesTranslation", (object)examplesTranslation ?? DBNull.Value);

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

                word.Id = (int)reader["Id"];
                word.Text = reader["Text"].ToString();
                word.Transcription = reader["Transcription"]?.ToString() ?? String.Empty;
                word.Translation = reader["Translation"].ToString().Split(' ').ToList();
                word.Examples = reader["Examples"]?.ToString().Split(' ').ToList();
                word.ExamplesTranslation = reader["ExamplesTranslation"]?.ToString().Split(' ').ToList();

                words.Add(word);
            }

            _sqlConnection.Close();

            return words;
        }
    }
}

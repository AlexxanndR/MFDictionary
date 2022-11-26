using MFDictionary.MVVM.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MFDictionary.Services
{
    internal class WordsDboAdapter
    {
        private string _connectionString;
        SqlConnection _sqlConnection;

        public WordsDboAdapter()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            _sqlConnection = new SqlConnection(_connectionString);
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
            SqlParameter parameter = sqlCommand.Parameters.Add("@Id", SqlDbType.BigInt, 0, "Id");
            parameter.Direction = ParameterDirection.Output;

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
        public ObservableCollection<Word> GetRandomWords(int wordsNum)
        {
            _sqlConnection.Open();

            ObservableCollection<Word> words = new ObservableCollection<Word>();

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

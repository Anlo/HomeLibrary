using System.Data;
using Microsoft.Data.SqlClient;
using HomeLibrary.Helpers;
using HomeLibrary.Models;

namespace HomeLibrary.Data
{
    public class BookRepository
    {
        private static SqlConnection OpenConnection()
        {
            var conn = new SqlConnection(ConnectionHelper.ConnectionString);
            try
            {
                conn.Open();
            }
            catch (SqlException ex)
            {
                throw new Exception(
                    "Не удалось подключиться к БД. Убедитесь, что SQL Server LocalDB запущен и база HomeLibrary создана.", ex);
            }
            return conn;
        }

        public List<Book> GetAll()
        {
            using var conn = OpenConnection();
            using var cmd = new SqlCommand("dbo.Book_SelectAll", conn)
            { CommandType = CommandType.StoredProcedure };

            return ReadBooks(cmd);
        }

        public Book? GetById(int id)
        {
            using var conn = OpenConnection();
            using var cmd = new SqlCommand("dbo.Book_SelectById", conn)
            { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@Id", id);

            var books = ReadBooks(cmd);
            return books.FirstOrDefault();
        }

        public int Insert(Book book)
        {
            using var conn = OpenConnection();
            using var cmd = new SqlCommand("dbo.Book_Insert", conn)
            { CommandType = CommandType.StoredProcedure };

            cmd.Parameters.AddWithValue("@Title", book.Title);
            cmd.Parameters.AddWithValue("@Author", book.Author);
            cmd.Parameters.AddWithValue("@PublishYear", book.PublishYear);
            cmd.Parameters.AddWithValue("@PageCount", book.PageCount);
            cmd.Parameters.AddWithValue("@TableOfContents",
                string.IsNullOrWhiteSpace(book.TableOfContentsXml)
                    ? DBNull.Value
                    : (object)book.TableOfContentsXml);

            var result = cmd.ExecuteScalar();
            return Convert.ToInt32(result);
        }

        public void Update(Book book)
        {
            using var conn = OpenConnection();
            using var cmd = new SqlCommand("dbo.Book_Update", conn)
            { CommandType = CommandType.StoredProcedure };

            cmd.Parameters.AddWithValue("@Id", book.Id);
            cmd.Parameters.AddWithValue("@Title", book.Title);
            cmd.Parameters.AddWithValue("@Author", book.Author);
            cmd.Parameters.AddWithValue("@PublishYear", book.PublishYear);
            cmd.Parameters.AddWithValue("@PageCount", book.PageCount);
            cmd.Parameters.AddWithValue("@TableOfContents",
                string.IsNullOrWhiteSpace(book.TableOfContentsXml)
                    ? DBNull.Value
                    : (object)book.TableOfContentsXml);

            cmd.ExecuteNonQuery();
        }

        public void Delete(int id)
        {
            using var conn = OpenConnection();
            using var cmd = new SqlCommand("dbo.Book_Delete", conn)
            { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@Id", id);
            cmd.ExecuteNonQuery();
        }

        public List<Book> Search(string searchString)
        {
            using var conn = OpenConnection();
            using var cmd = new SqlCommand("dbo.Book_Search", conn)
            { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@SearchString", searchString);

            return ReadBooks(cmd);
        }

        private static List<Book> ReadBooks(SqlCommand cmd)
        {
            var list = new List<Book>();
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new Book
                {
                    Id = reader.GetInt32(0),
                    Title = reader.GetString(1),
                    Author = reader.GetString(2),
                    PublishYear = reader.GetInt32(3),
                    PageCount = reader.GetInt32(4),
                    TableOfContentsXml = reader.IsDBNull(5) ? string.Empty : reader.GetString(5),
                    CreatedAt = reader.GetDateTime(6),
                    UpdatedAt = reader.GetDateTime(7)
                });
            }
            return list;
        }
    }
}
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentsApp.Database
{
    public class ApplicationDbContext
    {
        public SQLiteAsyncConnection _dbConnection;

        public readonly static string nameSpace = "StudentsApp.Database.";

        public const string DatabaseFileName = "StudentsApp.Database.db3";

        public static string databasePath => Path.Combine(FileSystem.AppDataDirectory, DatabaseFileName);

        public const SQLite.SQLiteOpenFlags Flags =
            SQLiteOpenFlags.ReadWrite |
            SQLiteOpenFlags.SharedCache |
            SQLiteOpenFlags.Create;

        public ApplicationDbContext()
        {
            if (_dbConnection is null)
            {
                _dbConnection = new SQLiteAsyncConnection(databasePath, Flags);
                _dbConnection.CreateTableAsync<Student>();
            }
        }

        public async Task<int> CreateAsync<TEntity>(TEntity entity) where TEntity : class
        {
            return await _dbConnection.InsertAsync(entity);
        }

        public async Task<int> UpdateAsync<TEntity>(TEntity entity) where TEntity : class
        {
            return await _dbConnection.UpdateAsync(entity);
        }

        public async Task<int> DeleteAsync<TEntity>(TEntity entity) where TEntity : class
        {
            return await _dbConnection.DeleteAsync(entity);
        }

        public async Task<int> UpsertAsync<TEntity>(TEntity entity) where TEntity : class
        {
            return await _dbConnection.InsertOrReplaceAsync(entity);
        }

        public List<T> GetTableRows<T>(string tableName) where T : class
        {
            object[] obj = new object[] { };
            TableMapping map = new TableMapping(Type.GetType(nameSpace + tableName));
            string query = "SELECT * FROM [" + tableName + "]";
            return _dbConnection.QueryAsync(map, query, obj).Result.Cast<T>().ToList();
        }

        public object GetTableRow(string tableName, string column , string value)
        {
            object[] obj = new object[] { };
            TableMapping map = new TableMapping(Type.GetType(nameSpace + tableName));
            string query = "SELECT * FROM " + tableName + " WHERE " + column + " = '" + value + "'";
            return _dbConnection.QueryAsync(map, query, obj).Result.FirstOrDefault();
        }
    }
}

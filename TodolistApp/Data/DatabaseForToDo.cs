using SQLite;
using TodolistApp.Model;

namespace TodolistApp.Data
{
    public class DatabaseForToDo
    {
        private readonly SQLiteAsyncConnection _database;
        private bool _isInitialized = false;

        public DatabaseForToDo()
        {
            string path = Path.Combine(FileSystem.AppDataDirectory, "ToDo.db");
            _database = new SQLiteAsyncConnection(path);
        }

        private async Task<bool> InitializeAsync()
        {
            if (_isInitialized)
                return true;

            var result = await _database.CreateTableAsync<DatabaseForToDo>();
            _isInitialized = result > 0;
            return _isInitialized;
        }

        public async Task<List<MyTask>> GetItemsAsync()
        {
            var initialized = await InitializeAsync();

            if (!initialized)
                return new List<MyTask>();

            return await _database.Table<MyTask>().ToListAsync();
        }

        public async Task<bool> SaveItemAsync(MyTask task)
        {
            var initialized = await InitializeAsync();

            if (!initialized)
                return false;

            if (task.Id == 0)
            {
                var result = await _database.InsertAsync(task);
                return result > 0;
            }
            else
            {
                var result = await _database.UpdateAsync(task);
                return result > 0;
            }
        }

        public async Task<bool> DeleteItemAsync(MyTask task)
        {
            var initialized = await InitializeAsync();
            if (!initialized)
                return false;

            var result = await _database.DeleteAsync(task);
            return result > 0;
        }
    }
}

using SQLite;
using TodolistApp.Model;

namespace TodolistApp.Data
{
    public class DatabaseForToDo
    {
        private SQLiteAsyncConnection _database;

        public DatabaseForToDo()
        {
            _ = InitializeAsync();
        }

        private async Task InitializeAsync()
        {
            if (_database != null)
                return;

            try
            {
                var databasePath = Path.Combine(FileSystem.AppDataDirectory, "ToDoList.db");
                _database = new SQLiteAsyncConnection(databasePath);

                var result = await _database.CreateTableAsync<MyTask>();
                System.Diagnostics.Debug.WriteLine($"Таблица создана: {result}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка инициализации базы данных: {ex.Message}");
            }
        }

        public async Task<List<MyTask>> GetItemsAsync()
        {
            await InitializeAsync();
            return await _database.Table<MyTask>().ToListAsync();
        }

        public async Task<int> SaveItemAsync(MyTask item)
        {
            await InitializeAsync();
            if (item.Id == 0)
                return await _database.InsertAsync(item);
            else
                return await _database.UpdateAsync(item);
        }

        public async Task<int> DeleteItemAsync(MyTask item)
        {
            await InitializeAsync();
            return await _database.DeleteAsync(item);
        }
    }
}

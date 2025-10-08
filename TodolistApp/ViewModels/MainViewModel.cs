using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using TodolistApp.Data;
using TodolistApp.Model;

namespace TodolistApp.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        private readonly DatabaseForToDo _database;
        private PeriodicTimer _timer;

        public DateTime Today => DateTime.Today;

        public List<string> TaskFlags { get; } = new()
        {
            "Home",
            "Study",
            "Work",
            "Another"
        };

        public List<string> FilterFlags { get; } = new()
        {
            "Without flags",
            "Home",
            "Study",
            "Work",
            "Another"
        };

        [ObservableProperty]
        private string selectedFilter = "Without flags";

        [ObservableProperty]
        private ObservableCollection<MyTask> tasksCollection;

        [ObservableProperty]
        private ObservableCollection<MyTask> completedTasksCollection;

        [ObservableProperty]
        private ObservableCollection<MyTask> deadlineTasksCollection;

        [ObservableProperty]
        private string newTaskTitle;

        [ObservableProperty]
        private string newTaskDescription;

        [ObservableProperty]
        private string newTaskFlag = "Another";

        [ObservableProperty]
        private DateTime newTaskDeadline = DateTime.Now.AddDays(1);

        public MainViewModel()
        {
            _database = new DatabaseForToDo();
            TasksCollection = new ObservableCollection<MyTask>();
            CompletedTasksCollection = new ObservableCollection<MyTask>();
            DeadlineTasksCollection = new ObservableCollection<MyTask>();

            _ = LoadTaskAndFilterAsync();
            _ = StartDeadlineChecker();
        }

        [RelayCommand]
        private async Task LoadTaskAndFilterAsync()
        {
            try
            {
                var items = await _database.GetItemsAsync();

                if (SelectedFilter == "Without flags")
                {
                    TasksCollection = new ObservableCollection<MyTask>(items.Where(item => !item.IsCompleted && !item.IsDeadlineOver));
                    CompletedTasksCollection = new ObservableCollection<MyTask>(items.Where(item => item.IsCompleted));
                    DeadlineTasksCollection = new ObservableCollection<MyTask>(items.Where(item => item.IsDeadlineOver));
                }
                else
                {
                    var filteredItems = items.Where(item => item.TaskFlag == SelectedFilter);
                    TasksCollection = new ObservableCollection<MyTask>(filteredItems.Where(item => !item.IsCompleted && !item.IsDeadlineOver));
                    CompletedTasksCollection = new ObservableCollection<MyTask>(filteredItems.Where(item => item.IsCompleted));
                    DeadlineTasksCollection = new ObservableCollection<MyTask>(filteredItems.Where(item => item.IsDeadlineOver));
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Ошибка", $"Не удалось загрузить задачи: {ex.Message}", "OK");
            }
        }

        partial void OnSelectedFilterChanged(string value)
        {
            _ = LoadTaskAndFilterAsync();
        }

        [RelayCommand]
        private async Task ToggleComplete(MyTask task)
        {
            if (task != null)
            {
                try
                {
                    task.IsCompleted = !task.IsCompleted;
                    await _database.SaveItemAsync(task);
                    await LoadTaskAndFilterAsync();
                }
                catch (Exception ex)
                {
                    await Application.Current.MainPage.DisplayAlert("Ошибка", $"Не удалось обновить задачу: {ex.Message}", "OK");
                }
            }
        }

        

        private async Task CheckDeadlineTasks()
        {
            var deadlineTasks = new List<MyTask>();

            foreach (var task in TasksCollection)
            {
                if (task.TaskDeadline < DateTime.Now)
                {
                    deadlineTasks.Add(task);
                }
            }

            if (deadlineTasks.Count > 0)
            {
                await MarkTasksAsDeadlineOver(deadlineTasks);
            }
        }

        private async Task MarkTasksAsDeadlineOver(List<MyTask> tasks)
        {
            foreach (var task in tasks)
            {
                task.IsDeadlineOver = true;
                await _database.SaveItemAsync(task);
            }
            await LoadTaskAndFilterAsync();
        }

        private async Task StartDeadlineChecker()
        {
            _timer = new PeriodicTimer(TimeSpan.FromSeconds(10));

            while (await _timer.WaitForNextTickAsync())
            {
                await CheckDeadlineTasks();
            }
        }

    }
}

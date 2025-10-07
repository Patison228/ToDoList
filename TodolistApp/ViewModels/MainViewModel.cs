using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using TodolistApp.Data;
using TodolistApp.Model;

namespace TodolistApp.ViewModels
{ 
    
    public partial class MainViewModel : ObservableObject
    {
        private readonly DatabaseForToDo database; // база данных

        public List<string> TaskFiltFlag { get; } = new()
        {
            "without filters",
            "Home",
            "Study",
            "Work",
            "Another"
        };

        public List<string> TaskFlags { get; } = new()
        {
            "Home",
            "Study",
            "Work",
            "Another"
        };

        public DateTime Today => DateTime.Today; // Для минимального для в датапикере

        [ObservableProperty]
        ObservableCollection<MyTask> tasksCollection; // Коллекция заданий

        [ObservableProperty]
        ObservableCollection<MyTask> deadlineTasksCollection;

        [ObservableProperty]
        ObservableCollection<MyTask> completedTasksCollection;

        [ObservableProperty]
        private DateTime newTaskDeadline; // Для дедлайна
        
        [ObservableProperty]
        private string newTaskTitle; // Для названия

        [ObservableProperty]
        private string newTaskDescription; // Для описания

        [ObservableProperty]
        private string newTaskFlag; // Поле выбранного флага новому заданию

        [ObservableProperty]
        private string selectedFilter;

        public MainViewModel()
        {
            tasksCollection = new ObservableCollection<MyTask>();
            completedTasksCollection = new ObservableCollection<MyTask>();
            deadlineTasksCollection = new ObservableCollection<MyTask>();

            newTaskDeadline = DateTime.Now.AddDays(1);
            newTaskFlag = "Another";
            newTaskTitle = string.Empty;
            newTaskDescription = string.Empty;

            selectedFilter = "Without filters";

            database = new DatabaseForToDo();

        }

        private void ClearForm()
        {
            NewTaskTitle = string.Empty;
            NewTaskDescription = string.Empty;
            NewTaskFlag = "Another";
            NewTaskDeadline = DateTime.Now.AddDays(1);
        }

        

        [RelayCommand]
        private async Task LoadTaskAndFilterAsync()
        {
            var tasks = await database.GetItemsAsync();

            if (selectedFilter == "Without filters")
            {
                TasksCollection = new ObservableCollection<MyTask>(tasks.Where(item => !item.IsCompleted && !item.IsDeadlineOver));

                CompletedTasksCollection = new ObservableCollection<MyTask>(tasks.Where(item => item.IsCompleted == true));

                DeadlineTasksCollection = new ObservableCollection<MyTask>(tasks.Where(item => item.IsDeadlineOver == true));
            }
            else
            {
                var filteredItems = tasks.Where(item => item.TaskFlag == SelectedFilter);
                TasksCollection = new ObservableCollection<MyTask>(filteredItems.Where(item => !item.IsCompleted && !item.IsDeadlineOver));

                CompletedTasksCollection = new ObservableCollection<MyTask>(filteredItems.Where(item => item.IsCompleted == true));

                DeadlineTasksCollection = new ObservableCollection<MyTask>(filteredItems.Where(item => item.IsDeadlineOver == true));
            }
        }

        partial void OnSelectedFilterChanged(string value)
        {
            _ = LoadTaskAndFilterAsync();
        }

        [RelayCommand]
        private void AddTask()
        {
            if (NewTaskTitle == string.Empty)
                return;

            var newTask = new MyTask(NewTaskTitle, NewTaskDescription, NewTaskDeadline, NewTaskFlag, false);

            tasksCollection.Add(newTask);

            ClearForm();
        }

        [RelayCommand]
        private void DeleteTask(MyTask task)
        {
            if (task != null)
            {
                tasksCollection.Remove(task);
            }
        }
    }
}

using CommunityToolkit.Mvvm.ComponentModel;
using SQLite;

namespace TodolistApp.Model
{
    public partial class MyTask : ObservableObject
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public DateTime Today => DateTime.Today;

        [ObservableProperty]
        private string taskName;

        [ObservableProperty]
        private string taskDescription;

        [ObservableProperty]
        private bool isCompleted;

        [ObservableProperty]
        private bool isDeadlineOver;

        [ObservableProperty]
        private DateTime taskDeadline;

        [ObservableProperty]
        private string taskFlag;

        public MyTask()
        {
            taskName = string.Empty;
            taskDescription = string.Empty;
            taskFlag = "Another";
            taskDeadline = DateTime.Now.AddDays(1);
        }

        public MyTask(string name, string description, DateTime deadline, string flag, bool completed, bool overDeadline)
        {
            taskName = name;
            taskDescription = description;
            taskDeadline = deadline;
            taskFlag = flag;
            isCompleted = completed;
            isDeadlineOver = overDeadline;
        }
    }
}

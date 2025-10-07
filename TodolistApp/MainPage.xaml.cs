using TodolistApp.ViewModels;

namespace TodolistApp
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();

            BindingContext = new MainViewModel();
        }

    }

}

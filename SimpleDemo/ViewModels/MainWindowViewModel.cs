using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleDemo.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public string Greeting => "Welcome to Avalonia!";

        public MapViewModel MapViewModel { get; set; }

        public MainWindowViewModel()
        {
            MapViewModel = new MapViewModel();
        }
    }
}

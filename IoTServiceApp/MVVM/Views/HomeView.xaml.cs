using IoTServiceApp.MVVM.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Windows;
using System.Windows.Controls;

namespace IoTServiceApp.MVVM.Views
{
    public partial class HomeView : UserControl
    {
        public HomeView()
        {
            InitializeComponent();
        }

        private void Device_Tile_Clicked(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;

            var viewModel = DataContext as HomeViewModel;
            viewModel!.SwitchDeviceOnOffCommand.Execute(button);
        }
    }
}

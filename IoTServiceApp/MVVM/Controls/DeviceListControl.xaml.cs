using IoTServiceApp.MVVM.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace IoTServiceApp.MVVM.Controls
{
    public partial class DeviceListControl : UserControl
    {
        public DeviceListControl(HomeViewModel homeViewModel)
        {
            InitializeComponent();
            DataContext = homeViewModel;

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            var device = (DeviceInfoViewModel)button.DataContext;
            
            var viewModel = DataContext as HomeViewModel;
            var confirmation = MessageBox.Show("Are you sure?", "Confirmation", MessageBoxButton.YesNo);
            
            if(confirmation == MessageBoxResult.Yes)
                viewModel!.DeleteDeviceFromCloudHomeCommand.Execute(device.DeviceInfo.Id);

        }

    }
}

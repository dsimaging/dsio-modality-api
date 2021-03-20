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

namespace WpfSample
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            DataContext = new MainViewModel();
        }

        public MainViewModel ViewModel => DataContext as MainViewModel;

        private void BtnLogin_OnClick(object sender, RoutedEventArgs e)
        {
            // Try to login with credentials
            ViewModel.Login(ViewModel.ApiUserName, ViewModel.ApiKey)
                .ContinueWith(task =>
                {
                    // Display error message if exception occurred
                    if (task.IsFaulted)
                    {
                        MessageBox.Show(task.Exception?.Message,"Login Error");
                    }
                });
        }
    }
}

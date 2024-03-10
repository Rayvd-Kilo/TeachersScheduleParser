using System.Net.Http;
using System.Net.Http.Json;
using System.Net.Sockets;
using System.Text;
using System.Windows;
using System.Windows.Input;

using Microsoft.Win32;

using TeachersScheduleParser.Runtime.Interfaces;
using TeachersScheduleParser.Runtime.Structs;

namespace TeachersScheduleParser
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private readonly IFileReaderDataFactory<Schedule[], string> _scheduleFactory;
        private readonly IDataContainerModel<Schedule[]> _dataContainerModel;

        public MainWindow(
            IFileReaderDataFactory<Schedule[], string> scheduleFactory,
            IDataContainerModel<Schedule[]> dataContainerModel)
        {
            _scheduleFactory = scheduleFactory;
            _dataContainerModel = dataContainerModel;
            InitializeComponent();
        }

        private void FileOpenerButton_OnClick(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                DefaultExt = ".xlsx",
                Filter = "Excel Files (*.xlsx)|*.xlsx"
            };
            
            var result = openFileDialog.ShowDialog();
            
            if (result == true)
            {
                var filePath = openFileDialog.FileName;

                var schedules = _scheduleFactory.Create(filePath);
                
                _dataContainerModel.SaveData(schedules);

                var httpClient = new HttpClient();

                using var response = 
                    httpClient.PostAsync("http://localhost:55000/add-json", JsonContent.Create(schedules, typeof(Schedule[])));
                
                MessageBox.Show("Request Send");
            }
        }
        
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                DragMove();
            }
        }

        private void ApplicationCloseButton_OnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ApplicationHideButton_OnClick(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }
    }
}
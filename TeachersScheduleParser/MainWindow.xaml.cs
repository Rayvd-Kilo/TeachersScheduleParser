using System.Windows;

using Microsoft.Win32;

using TeachersScheduleParser.Runtime.Factories;
using TeachersScheduleParser.Runtime.Structs;

namespace TeachersScheduleParser
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private readonly IFileReaderDataFactory<Schedule[], string> _scheduleFactory;

        public MainWindow(IFileReaderDataFactory<Schedule[], string> scheduleFactory)
        {
            _scheduleFactory = scheduleFactory;
            InitializeComponent();
        }

        private void FileOpenerButton_OnClick(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                DefaultExt = ".xlsx",
                Filter = "Excel Files (*.xlsx)|*.xlsx"
            };
            
            bool? result = openFileDialog.ShowDialog();
            
            if (result == true)
            {
                string filePath = openFileDialog.FileName;

                var schedules = _scheduleFactory.Create(filePath);
            }
        }
    }
}
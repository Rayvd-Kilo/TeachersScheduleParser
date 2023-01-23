using System.Windows;

using Microsoft.Win32;

using TeachersScheduleParser.Runtime.Controllers;
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
        private readonly TelegramBotController _telegramBotController;
        private readonly IDataContainerService<Schedule[]> _dataContainerService;

        public MainWindow(IFileReaderDataFactory<Schedule[], string> scheduleFactory,
            TelegramBotController telegramBotController,
            IDataContainerService<Schedule[]> dataContainerService)
        {
            _scheduleFactory = scheduleFactory;
            _telegramBotController = telegramBotController;
            _dataContainerService = dataContainerService;
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
                
                _dataContainerService.SaveData(schedules);
                
                _telegramBotController.SetSchedules(schedules);
            }
        }
    }
}
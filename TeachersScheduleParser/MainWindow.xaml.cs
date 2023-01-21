using System.Data;
using System.Windows;

using DataReaders.ValueTypes;

using ExelParser.Runtime.Factories;

using Microsoft.Win32;

namespace TeachersScheduleParser
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private readonly IFileReaderDataFactory<DataSet, string> _dataSetFactory;
        private readonly IFileReaderDataFactory<DataInMatrix<string>[], DataSet> _datesFactory;

        public MainWindow(
            IFileReaderDataFactory<DataSet, string> dataSetFactory,
            IFileReaderDataFactory<DataInMatrix<string>[], DataSet> datesFactory)
        {
            _dataSetFactory = dataSetFactory;
            _datesFactory = datesFactory;
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

                var dataSet = _dataSetFactory.Create(filePath);

                var dates = _datesFactory.Create(dataSet);
            }
        }
    }
}
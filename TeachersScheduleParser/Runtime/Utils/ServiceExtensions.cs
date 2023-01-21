using System;
using System.Data;

using DataReaders.Readers;
using DataReaders.Readers.DataSetReader;
using DataReaders.Readers.Interfaces;
using DataReaders.Readers.RegexReaders;
using DataReaders.ValueTypes;

using Microsoft.Extensions.DependencyInjection;

using TeachersScheduleParser.Runtime.Factories;

namespace TeachersScheduleParser.Runtime.Utils
{
    public static class ServiceExtensions
    {
        public static void AddDataSetFactory(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<IDataReader<DataSet>, ExcelDataSetReader>();
            
            serviceCollection.AddTransient<IFileReader<DataSet>, FileStreamDataReader<DataSet>>();
            
            serviceCollection.AddSingleton<Func<string, DataSet>>(x => filePath =>
            {
                var dataReader = x.GetService<IDataReader<DataSet>>();
                var fileStreamReader = x.GetService<IFileReader<DataSet>>();
                return dataReader.ReadData(fileStreamReader, filePath);
            });
            
            serviceCollection.AddSingleton<IFileReaderDataFactory<DataSet, string>,
                FileReaderDataFactory<DataSet, string>>();
        }
        
        public static void AddDateFactory(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<ITableReader<DataInMatrix<string>>, TableReader<DataInMatrix<string>>>();

            serviceCollection.AddTransient<DateReader>();
            
            serviceCollection.AddSingleton<Func<DataSet, DataInMatrix<string>[]>>(x => dataSet =>
            {
                var reader = x.GetService<DateReader>();

                return reader.ReadData(dataSet.Tables[0]);
            });
            
            serviceCollection.AddSingleton<IFileReaderDataFactory<DataInMatrix<string>[], DataSet>,
                FileReaderDataFactory<DataInMatrix<string>[], DataSet>>();
        }
    }
}
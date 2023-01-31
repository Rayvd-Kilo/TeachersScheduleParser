using System;
using System.Data;

using DataReaders.Readers;
using DataReaders.Readers.DataSetReader;
using DataReaders.Readers.Interfaces;
using DataReaders.Readers.JSONReaders;
using DataReaders.Readers.RegexReaders;
using DataReaders.ValueTypes;

using Microsoft.Extensions.DependencyInjection;

using TeachersScheduleParser.Runtime.Controllers;
using TeachersScheduleParser.Runtime.Factories;
using TeachersScheduleParser.Runtime.Interfaces;
using TeachersScheduleParser.Runtime.Models;
using TeachersScheduleParser.Runtime.Structs;

using Telegram.Bot.Types;

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
                return dataReader!.ReadData(fileStreamReader!, filePath);
            });
            
            serviceCollection.AddSingleton<IFileReaderDataFactory<DataSet, string>,
                FileReaderDataFactory<DataSet, string>>();
        }
        
        public static void AddDateFactory(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<ITableReader<DataInMatrix<string>>, TableReader<DataInMatrix<string>>>();

            serviceCollection.AddTransient<DateRegexTableReader>();
            
            serviceCollection.AddSingleton<Func<DataSet, DataInMatrix<string>[]>>(x => dataSet =>
            {
                var reader = x.GetService<DateRegexTableReader>();

                return reader!.ReadData(dataSet.Tables[0]);
            });
            
            serviceCollection.AddSingleton<IFileReaderDataFactory<DataInMatrix<string>[], DataSet>,
                FileReaderDataFactory<DataInMatrix<string>[], DataSet>>();
        }

        public static void AddSchedulesReaderService(this IServiceCollection serviceCollection)
        {
            var jsonReader = new JsonReader<Schedule[]>();

            var fileStreamDataReader = new FileStreamDataReader<Schedule[]>();
            
            var schedulesDataModelInstance = new SchedulesDataModel(jsonReader, fileStreamDataReader);

            serviceCollection.AddSingleton<IReactiveValue<Schedule[]>>(schedulesDataModelInstance);

            serviceCollection.AddSingleton<IDataContainerModel<Schedule[]>>(schedulesDataModelInstance);
        }

        public static void AddTelegramBotSystem(this IServiceCollection serviceCollection)
        {
            var clientDataModelInstance = new ClientDataModel();

            var reportsModel = new ClientsReportsModel();

            serviceCollection.AddSingleton<IDataSaver<ClientData>>(reportsModel);

            serviceCollection.AddSingleton<IDataGetter<ClientReports[]>>(reportsModel);
            
            serviceCollection.AddSingleton<IAsyncReactiveValue<ClientData>>(clientDataModelInstance);

            serviceCollection.AddSingleton<IDataContainerModel<ClientData[]>>(clientDataModelInstance);

            serviceCollection.AddTransient<IAsyncResultHandler<Exception>, BotErrorHandler>();

            serviceCollection.AddTransient<IAsyncResultHandler<Update>, BotUpdateHandler>();
            
            serviceCollection.AddSingleton<IAsyncStartable, TelegramBotController>();
        }
    }
}
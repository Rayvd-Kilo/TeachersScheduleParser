using System.Collections.Generic;
using System.Data;

using DataReaders.ValueTypes;

using TeachersScheduleParser.Runtime.Creators;
using TeachersScheduleParser.Runtime.Interfaces;
using TeachersScheduleParser.Runtime.Services;
using TeachersScheduleParser.Runtime.Structs;

namespace TeachersScheduleParser.Runtime.Factories
{
    public class TeachersSchedulesFactory : IFileReaderDataFactory<Schedule[], string>
    {
        private readonly DataSetParsingService _dataSetParsingService;
        private readonly IFileReaderDataFactory<DataSet, string> _dataSetFactory;
        private readonly IFileReaderDataFactory<DataInMatrix<string>[], DataSet> _datesFactory;

        public TeachersSchedulesFactory(
            DataSetParsingService dataSetParsingService,
            IFileReaderDataFactory<DataSet, string> dataSetFactory,
            IFileReaderDataFactory<DataInMatrix<string>[], DataSet> datesFactory)
        {
            _dataSetParsingService = dataSetParsingService;
            _dataSetFactory = dataSetFactory;
            _datesFactory = datesFactory;
        }

        Schedule[] IFileReaderDataFactory<Schedule[], string>.Create(string data)
        {
            var scheduleList = new List<Schedule>();

            var scheduleCreator = new ScheduleCreator();

            var dataSet = _dataSetFactory.Create(data);
            
            var personsData = _dataSetParsingService.GetPersons(dataSet);

            var scheduleInformation = 
                _dataSetParsingService.GetSubjectsData(_datesFactory.Create(dataSet), dataSet);

            foreach (var personData in personsData)
            {
                scheduleList.Add(scheduleCreator.CreatePersonSchedule(personData, scheduleInformation));
            }

            return scheduleList.ToArray();
        }
    }
}
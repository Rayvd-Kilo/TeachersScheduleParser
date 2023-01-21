using System;
using System.Collections.Generic;
using System.Data;

using DataReaders.Readers.DataSetReader;
using DataReaders.Readers.RegexReaders;
using DataReaders.ValueTypes;

using TeachersScheduleParser.Runtime.Factories;
using TeachersScheduleParser.Runtime.Services;
using TeachersScheduleParser.Runtime.Structs;

namespace TeachersScheduleParser.Runtime.Controllers
{
    public class TeachersScheduleInitializingController : IDisposable
    {
        private readonly DataSet _fileDataSet;
        private readonly DataSetParsingService _dataSetParsingService;
        private readonly List<Schedule> _schedulesContainer;

        public TeachersScheduleInitializingController(
            DataSet fileDataSet,
            DataSetParsingService dataSetParsingService,
            List<Schedule> schedulesContainer)
        {
            _fileDataSet = fileDataSet;
            _dataSetParsingService = dataSetParsingService;
            _schedulesContainer = schedulesContainer;
        }

        public void Initialize()
        {
            _schedulesContainer.Clear();

            var scheduleCreator = new ScheduleCreator();
            
            var personsData = _dataSetParsingService.GetPersons(_fileDataSet);
        
            var dates =
                new DateReader(new TableReader<DataInMatrix<string>>()).ReadData(_fileDataSet.Tables[0]);

            var scheduleInformation = _dataSetParsingService.GetSubjectsData(dates, _fileDataSet);

            foreach (var personData in personsData)
            {
                _schedulesContainer.Add(scheduleCreator.CreatePersonSchedule(personData, scheduleInformation));
            }
        }

        void IDisposable.Dispose()
        {
            _schedulesContainer.Clear();
        }
    }
}
using System.Collections.Generic;
using System.Linq;

using DataReaders.ValueTypes;

using ExelParser.Runtime.Structs;

namespace ExelParser.Runtime.Factories
{
    public class ScheduleCreator
    {
        private readonly DailyScheduleCreator _dailyScheduleCreator;

        public ScheduleCreator()
        {
            _dailyScheduleCreator = new DailyScheduleCreator();
        }
        
        public Schedule CreatePersonSchedule(PersonData targetPerson, DailyScheduleInformation[] dailySchedulesInformation)
        {
            var daysValueSet = GetDatesValue(dailySchedulesInformation.Select(x => x.Date));

            return new Schedule(targetPerson, daysValueSet.Select(dayValue =>
                _dailyScheduleCreator.CreateDailySchedule(targetPerson, dailySchedulesInformation, dayValue)).ToArray());
        }

        private string[] GetDatesValue(IEnumerable<DataInMatrix<string>> datesDataSet)
        {
            var daysValueSet = new HashSet<string>();

            foreach (var data in datesDataSet)
            {
                daysValueSet.Add(data.DataValue);
            }

            return daysValueSet.ToArray();
        }
    }
}
using DataReaders.Readers;
using DataReaders.Readers.Interfaces;
using DataReaders.Readers.JSONReaders;
using DataReaders.Utils;

using TeachersScheduleParser.Runtime.Structs;
using TeachersScheduleParser.Runtime.Utils;

namespace TeachersScheduleParser.Runtime.Creators
{
    public class SubjectCreator
    {
        private readonly string _dailyInstructionsFilePath = FilePathGetter.GetPath("daily.json");
        private readonly string _weekendInstructionsFilePath = FilePathGetter.GetPath("weekend.json");

        private readonly TimeConverterInstruction _dailyInstruction;

        private readonly TimeConverterInstruction _weekendInstruction;
        
        public SubjectCreator()
        {
            IDataReader<TimeConverterInstruction> dataReader = new JsonReader<TimeConverterInstruction>();

            IFileReader<TimeConverterInstruction> fileReader = new FileStreamDataReader<TimeConverterInstruction>();

            _dailyInstruction = dataReader.ReadData(fileReader, _dailyInstructionsFilePath);

            _weekendInstruction = dataReader.ReadData(fileReader, _weekendInstructionsFilePath);
        }
        
        public Subject CreateSubject(string[] rowData, bool isDaily)
        {
            var subjectNumber = int.Parse(rowData[1]);

            var subjectTime = GetTimeBySubjectNumber(subjectNumber, isDaily);

            var subjectName = rowData[2];

            var subjectCabinet = rowData[4];

            var subjectGroup = rowData[0];

            return new Subject(subjectNumber, subjectTime.ConvertToStringTime(), subjectName,
                subjectName.ConvertToSubject(), subjectCabinet, subjectGroup);
        }

        private int GetTimeBySubjectNumber(int subjectNumber, bool isDaily)
        {
            return SubjectOrderToTimeConverter.ConvertOrderNumberToMinutes(subjectNumber,
                isDaily ? _dailyInstruction : _weekendInstruction);
        }
    }
}
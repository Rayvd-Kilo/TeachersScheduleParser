using System.Collections.Generic;
using System.Data;
using System.Numerics;

using DataReaders.Readers.DataSetReader;
using DataReaders.Readers.RegexReaders;
using DataReaders.ValueTypes;

using TeachersScheduleParser.Runtime.Enums;
using TeachersScheduleParser.Runtime.Structs;

namespace TeachersScheduleParser.Runtime.Services
{
    public class DataSetParsingService
    {
        public PersonData[] GetPersons(DataSet fileDataSet, string regexPattern, PersonType personType)
        {
            var fullNamesList = new List<PersonData>();

            UniqStringsRegexTableReader fullNameReader = new UniqStringsRegexTableReader(new TableReader<string>(), regexPattern);

            var fullNames = fullNameReader.ReadData(fileDataSet.Tables[0]);

            foreach (var fullName in fullNames)
            {
                fullNamesList.Add(new PersonData(fullName, personType));
            }

            return fullNamesList.ToArray();
        }

        public DailyScheduleInformation[] GetSubjectsData(DataInMatrix<string>[] dates, DataSet data)
        {
            var matrixDataCollection = new List<DailyScheduleInformation>();

            for (int i = 0; i < dates.Length; i++)
            {
                var currentDate = dates[i];

                var nextDate = new DataInMatrix<string>();

                if (i < dates.Length - 1)
                {
                    nextDate = dates[i + 1];
                }

                if ((i + 1) % 6 == 0)
                {
                    nextDate = new DataInMatrix<string>()
                    {
                        DataValue = string.Empty,
                        MatrixIndexes = new Vector2(currentDate.MatrixIndexes.X + 4, currentDate.MatrixIndexes.Y)
                    };
                }

                matrixDataCollection.Add(ReadScheduleData(currentDate, nextDate, data));
                matrixDataCollection.Add(ReadScheduleData(currentDate, nextDate, data, 4));
            }

            return matrixDataCollection.ToArray();
        }
        
        private DailyScheduleInformation ReadScheduleData(DataInMatrix<string> currentDate, DataInMatrix<string> nextDate,
            DataSet data, int offset = 0)
        {
            var matrixData =
                new MatrixData<string>(
                    new string[(int) (nextDate.MatrixIndexes.X - currentDate.MatrixIndexes.X) - 1, 5]);

            for (int j = 0; j < (nextDate.MatrixIndexes.X - currentDate.MatrixIndexes.X) - 1; j++)
            {
                for (int k = 0; k < 5; k++)
                {
                    var row = k == 0 ? 0 : j + (int) currentDate.MatrixIndexes.X + 1;

                    var column = k == 0
                        ? k + (int) currentDate.MatrixIndexes.Y + offset
                        : (k - 1) + (int) currentDate.MatrixIndexes.Y + offset;

                    var value = data.Tables[0].Rows[row][column].ToString()!;

                    if (value == string.Empty && k == 1)
                    {
                        value = (j + 1).ToString();
                    }
                
                    matrixData.Data[j, k] = value;
                }
            }

            return new DailyScheduleInformation(currentDate, matrixData);
        }
    }
}
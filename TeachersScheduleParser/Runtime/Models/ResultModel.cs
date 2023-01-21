using System;
using System.Text;

using TeachersScheduleParser.Runtime.Structs;

namespace TeachersScheduleParser.Runtime.Models
{
    public class ResultModel
    {
        public event Action<string> ResultChanged;

        public event Action ResultCleared;
        
        private readonly StringBuilder _stringBuilder;

        public ResultModel()
        {
            _stringBuilder = new StringBuilder();
        }

        public string GetResultText(Schedule[] schedules)
        {
            //todo: result logic
            
            return _stringBuilder.ToString();
        }
    }
}
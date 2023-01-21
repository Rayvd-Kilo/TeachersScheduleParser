using System.Windows.Controls;

namespace TeachersScheduleParser.Runtime.Views
{
    public class ResultView
    {
        private readonly TextBox _resultTextBox;

        public ResultView(TextBox resultTextBox)
        {
            _resultTextBox = resultTextBox;
        }

        public string SetResultText(string text)
        {
            return _resultTextBox.Text = text;
        }
    }
}
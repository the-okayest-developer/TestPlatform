using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace TestProject_2.model
{
    public abstract class QuestionVisitor
    {
        public abstract int Visit(SingleChoiceQuestion question, GroupBox questionBox);
        public abstract int Visit(MultipleChoiceQuestion question, GroupBox questionBox);
        public abstract int Visit(HalfCorrectChoiceQuestion question, GroupBox questionBox);
        public abstract int Visit(OpenEndedQuestion question, GroupBox questionBox);
        public abstract int Visit(ChronologyQuestion question, GroupBox questionBox);
        public abstract int Visit(MatchingQuestion question, GroupBox questionBox);
    }

    class QuizModeVisitor : QuestionVisitor
    {
        public override int Visit(SingleChoiceQuestion question, GroupBox questionBox)
        {
            var selectedRadioButton = FindSelectedRadioButton(questionBox);
            if (selectedRadioButton == null)
            {
                throw new ArgumentException($"Please check your answers for {questionBox.Header}. You did not choose any.");
            } 
            
            char selectedOption = selectedRadioButton.Content.ToString()[0];

            return question.CheckAnswer(selectedOption) ? 1 : 0;
        }

        public override int Visit(MultipleChoiceQuestion question, GroupBox questionBox)
        {
            var selectedCheckBoxes = FindSelectedCheckBoxes(questionBox);
            if (!selectedCheckBoxes.Any())
            {
                throw new ArgumentException($"Please check your answers for {questionBox.Header}. You did not choose any.");
            }
            
            List<char> selectedOptions = selectedCheckBoxes.Select(cb => cb.Content.ToString()[0]).ToList();
            return question.CheckAnswer(selectedOptions) ? 1 : 0;
        }

        public override int Visit(HalfCorrectChoiceQuestion question, GroupBox questionBox)
        {
            var selectedCheckBoxes = FindSelectedCheckBoxes(questionBox);
            if (!selectedCheckBoxes.Any())
            {
                throw new ArgumentException($"Please check your answers for {questionBox.Header}. You did not choose any.");
            }

            List<char> selectedOptions = selectedCheckBoxes.Select(cb => cb.Content.ToString()[0]).ToList();
            return question.CheckAnswer(selectedOptions) ? 1 : 0;
        }

        public override int Visit(OpenEndedQuestion question, GroupBox questionBox)
        {
            var textBoxes = FindVisualChildren<TextBox>(questionBox);
            if (!textBoxes.Any() || string.IsNullOrEmpty(textBoxes.Select(cb => cb.Text.ToString()).First()))
            {
                throw new ArgumentException($"Please check your answers for {questionBox.Header}. Your input is empty");
            }
            string userInput = textBoxes.Select(cb => cb.Text.ToString()).First();
            return question.CheckAnswer(userInput) ? 1 : 0;
        }

        public override int Visit(ChronologyQuestion question, GroupBox questionBox)
        {
            IEnumerable<string> ordersInput = FindVisualChildren<TextBox>(questionBox)
                            .Where(tb => tb.Tag != null && tb.Tag.ToString() == "Order")
                            .Select(tb => tb.Text);

            foreach (var item in ordersInput)
            {
                if (string.IsNullOrEmpty(item))
                {
                    throw new ArgumentException($"Please check your answers for {questionBox.Header}. Your input is empty");
                }
            }

            var orders = ordersInput
                .Select(input => int.Parse(input))
                .ToList();
            var options = FindVisualChildren<Label>(questionBox)
                .Select(ob => ob.Content.ToString()[0])
                .ToList();

            if (orders.Count != orders.Distinct().Count())
            {
                throw new Exception($"Please check your answers for {questionBox.Header}. There can not be several identical values");
            }

            Dictionary<char, int> userOrder = new Dictionary<char, int>();
            for (int i = 0; i < orders.Count; i++)
            {
                userOrder[options[i]] = orders[i];
            }
            List<char> sortedUserOrder = userOrder.OrderBy(kv => kv.Value).Select(kv => kv.Key).ToList();

            if (question.CheckAnswer(sortedUserOrder))
            {
                return 1;
            }

            return 0;
        }
        public override int Visit(MatchingQuestion question, GroupBox questionBox)
        {
            var answerPanel = questionBox.Content as StackPanel;

            Dictionary<char, String> questions = new Dictionary<char, string>();
            Dictionary<char, String> answers = new Dictionary<char, string>();

            foreach (var stackPanel in answerPanel.Children.OfType<StackPanel>())
            {
                var leftPanel = stackPanel.Children.OfType<DockPanel>()
                    .Where(dp => dp.Tag.Equals("Left")).First();
                var optionLabel = leftPanel.Children.OfType<Label>().FirstOrDefault();
                var leftTextBlock = leftPanel.Children.OfType<TextBlock>().FirstOrDefault();

                var rightPanel = stackPanel.Children.OfType<DockPanel>()
                    .Where(dp => dp.Tag.Equals("Right")).First();
                var matchingAnswerBox = rightPanel.Children.OfType<TextBox>().FirstOrDefault();
                var rightTextBlock = rightPanel.Children.OfType<TextBlock>().FirstOrDefault();

                if (string.IsNullOrEmpty(matchingAnswerBox.Text))
                {
                    throw new ArgumentException($"Please check your answers for {questionBox.Header}. Your input is empty");
                }
                questions[optionLabel.Content.ToString().ToLower()[0]] = leftTextBlock.Text;

                if (answers.ContainsKey(matchingAnswerBox.Text.Trim().ToLower()[0]))
                {
                    throw new Exception("Please check your answers for {questionBox.Header}. There can not be several identical values");
                }

                answers[matchingAnswerBox.Text.Trim().ToLower()[0]] = rightTextBlock.Text;
            }
            Dictionary<String, String> userInput = new Dictionary<string, string>();

            foreach (var key in questions.Keys)
            {
                if (!answers.ContainsKey(key))
                {
                    return 0;
                }
                userInput.Add(questions[key], answers[key]);
            }
            if (question.CheckAnswer(userInput)) 
            { 
                return 1; 
            }
            return 0;
        }

        private RadioButton FindSelectedRadioButton(GroupBox questionBox)
        {
            var radioButtons = FindVisualChildren<RadioButton>(questionBox);
            return radioButtons.FirstOrDefault(rb => rb.IsChecked == true);
        }

        private List<CheckBox> FindSelectedCheckBoxes(GroupBox questionBox)
        {
            var checkBoxes = FindVisualChildren<CheckBox>(questionBox);
            return checkBoxes.Where(cb => cb.IsChecked == true).ToList();
        }

        private IEnumerable<T> FindVisualChildren<T>(DependencyObject parent) where T : DependencyObject
        {
            var children = new List<T>();
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is T typedChild)
                {
                    children.Add(typedChild);
                }
                else
                {
                    var foundChild = FindVisualChildren<T>(child);
                    if (foundChild != null)
                    {
                        children.AddRange(foundChild);
                    }
                }
            }
            return children;
        }
    }
}

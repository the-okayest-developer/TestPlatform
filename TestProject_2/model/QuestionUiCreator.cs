using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using TestProject_2.components;

namespace TestProject_2.model
{
    public abstract class QuestionUiCreator
    {
        public GroupBox createEditableQuestionBox(int item)
        {
            return createEditableQuestionBox(null, item);
        }

        public GroupBox createEditableQuestionBox(Question? question, int item)
        {
            GroupBox groupBox = new GroupBox { Header = $"Question {item + 1}", Width = 500, Margin = new Thickness(10), BorderThickness = new Thickness(1) };
            string questionText = question == null ? "Enter your question here." : question.QuestionText;
            StackPanel questionPanel = CreateBaseEditableQuestionPanel(questionText, item, groupBox);

            AddAnswerControls(questionPanel, question);

            if (question != null)
            {
                foreach (Answer answer in question.Answers)
                {
                    AddEditableAnswerOption(questionPanel, answer);
                }
            }

            groupBox.Content = questionPanel;
            return groupBox;
        }

        public virtual GroupBox createNonEditableQuestionBox(Question question, int item)
        {
            GroupBox groupBox = new GroupBox { Header = $"Question {item + 1}", Width = 500, Margin = new Thickness(10), BorderThickness = new Thickness(1) };
            StackPanel questionPanel = CreateNonEditableQuestionPanel(question.QuestionText, item, groupBox);

            foreach (Answer answer in question.Answers)
            {
                AddNonEditableAnswerOption(questionPanel, answer);
            }

            groupBox.Content = questionPanel;
            return groupBox;
        }

        private StackPanel CreateBaseEditableQuestionPanel(String questionText, int item, GroupBox groupBox)
        {
            TextBox questionTextBox = new TextBox
            {
                Text = questionText,
                Width = 468,
                Margin = new Thickness(10, 10, 10, 0)
            };
            Button deleteQuestionButton = new Button
            {
                Content = "X",
                FontSize = 8,
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Top,
                Width = 15,
                Height = 15,
                Margin = new Thickness(0, -15, -15, 0)
            };


            StackPanel questionPanel = new StackPanel { Orientation = Orientation.Vertical, Name = $"rbGroup{item}" };
            questionPanel.Children.Add(deleteQuestionButton);

            questionPanel.Children.Add(questionTextBox);
            deleteQuestionButton.Click += (s, args) =>
            {
                var parent = VisualTreeHelper.GetParent(groupBox) as Panel;
                if (parent != null)
                {
                    parent.Children.Remove(groupBox);
                }
            };
            return questionPanel;
        }

        protected StackPanel CreateNonEditableQuestionPanel(String questionText, int item, GroupBox groupBox)
        {
            TextBlock questionTextBlock = new TextBlock
            {
                Text = questionText,
                Margin = new Thickness(10, 10, 10, 10)
            };

            StackPanel questionPanel = new StackPanel { Orientation = Orientation.Vertical, Name = $"rbGroup{item}" };

            questionPanel.Children.Add(questionTextBlock);
            return questionPanel;
        }

        protected virtual void AddAnswerControls(StackPanel questionPanel, Question question)
        {
            Button addAnswerOptionButton = new Button
            {
                Content = "Add Answer Option",
                Margin = new Thickness(10, 10, 10, 10)
            };
            questionPanel.Children.Add(addAnswerOptionButton);

            addAnswerOptionButton.Click += (s, args) => AddAnswerOption(questionPanel);
        }

        protected void AddAnswerOption(StackPanel questionPanel)
        {
            AddEditableAnswerOption(questionPanel, null);
        }

        public abstract void AddEditableAnswerOption(StackPanel questionPanel, Answer? answer);
        public abstract void AddNonEditableAnswerOption(StackPanel questionPanel, Answer? answer);

        public abstract Question CreateQuestionFromUiEntry(GroupBox groupBox);

        protected char GetNextOptionLabel(StackPanel questionPanel)
        {
            int optionCount = 0;
            foreach (var child in questionPanel.Children)
            {
                if (child is DockPanel || child is StackPanel)
                {
                    optionCount++;
                }
            }
            return (char)('A' + optionCount);
        }

        protected virtual void checkAnswers(Question question, int minAnswers, string questionName)
        {
            if (question.GetAnswers() == null || question.GetAnswers().Count < minAnswers)
            {
                throw new ArgumentException($"{questionName} has less than {minAnswers} answers.");
            }
        }

    }

    public class SingleChoiceQuestionCreator : QuestionUiCreator
    {
        public override void AddEditableAnswerOption(StackPanel questionPanel, Answer? answer)
        {
            DockPanel answerPanel = new DockPanel { Margin = new Thickness(0, 0, 0, 10) };
            RadioButton radioButton = new RadioButton
            {
                IsChecked = answer == null ? false : answer.IsCorrect,
                Content = answer == null ? GetNextOptionLabel(questionPanel) : answer.Option,
                VerticalAlignment = VerticalAlignment.Center,
                GroupName = questionPanel.Name,
                Margin = new Thickness(7, 2, 5, 0)
            };
            TextBox txtOption = new TextBox { Width = 430, Text = answer == null ? "" : answer.Text};

            Button deleteButton = new Button
            {
                Content = "X",
                FontSize = 8,
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Center,
                Width = 15,
                Height = 15,
                Margin = new Thickness(0, 0, -15, 0),
            };
            deleteButton.Click += (s, e) =>
            {
                questionPanel.Children.Remove(answerPanel);
                RenumberOptions(questionPanel);
            };

            answerPanel.Children.Add(deleteButton);
            DockPanel.SetDock(deleteButton, Dock.Right);
            answerPanel.Children.Add(radioButton);
            DockPanel.SetDock(radioButton, Dock.Left);
            answerPanel.Children.Add(txtOption);
            DockPanel.SetDock(txtOption, Dock.Right);


            questionPanel.Children.Add(answerPanel);


        }
        
        public override void AddNonEditableAnswerOption(StackPanel questionPanel, Answer? answer)
        {
            DockPanel answerPanel = new DockPanel { Margin = new Thickness(0, 0, 0, 10) };
            RadioButton radioButton = new RadioButton
            {
                Content = answer == null ? GetNextOptionLabel(questionPanel) : answer.Option,
                VerticalAlignment = VerticalAlignment.Center,
                GroupName = questionPanel.Name,
                Margin = new Thickness(7, 1, 5, 0)
            };
            TextBlock txtOption = new TextBlock { Width = 430, Text = answer == null ? "" : answer.Text};

            answerPanel.Children.Add(radioButton);
            DockPanel.SetDock(radioButton, Dock.Left);
            answerPanel.Children.Add(txtOption);
            DockPanel.SetDock(txtOption, Dock.Right);

            questionPanel.Children.Add(answerPanel);
        }

        public override Question CreateQuestionFromUiEntry(GroupBox groupBox)
        {
            var stackPanel = groupBox.Content as StackPanel;
            var txtQuestion = stackPanel.Children.OfType<TextBox>().FirstOrDefault();
            var questionText = txtQuestion?.Text;

            if (string.IsNullOrEmpty(questionText))
            {
                throw new ArgumentException($"Please check question text for {groupBox.Header}. It can not be empty");
            }

            var question = new SingleChoiceQuestion(questionText);
            foreach (var dockPanel in stackPanel.Children.OfType<DockPanel>())
            {
                var radioButton = dockPanel.Children.OfType<RadioButton>().FirstOrDefault();
                var txtOption = dockPanel.Children.OfType<TextBox>().FirstOrDefault();

                if (radioButton != null && txtOption != null)
                {
                    var option = txtOption.Text;
                    if (string.IsNullOrEmpty(option))
                    {
                        throw new ArgumentException($"Please check answers for {groupBox.Header}. Answers can not be empty");
                    }
                    question.AddAnswer(new Answer(radioButton.Content.ToString()[0], option, radioButton.IsChecked == true));
                }
            }
            checkAnswers(question, 2, groupBox.Header.ToString());

            var correctAnswers = question.Answers.Select(a => a.IsCorrect).Where(correct => correct).ToList().Count();
            if (correctAnswers != 1)
            {
                throw new ArgumentException($"Please check answers for {groupBox.Header}. There should be 1 correct answer.");
            }

            return question;
        }

        private void RenumberOptions(StackPanel questionPanel)
        {
            char optionChar = 'A';
            foreach (DockPanel answerPanel in questionPanel.Children.OfType<DockPanel>())
            {
                foreach (var child in answerPanel.Children)
                {
                    if (child is RadioButton button)
                    {
                        button.Content = optionChar.ToString();
                        optionChar++;
                        break;
                    }
                }
            }
        }
    }

    public class MultipleChoiceQuestionCreator : QuestionUiCreator
    {

        public override void AddEditableAnswerOption(StackPanel questionPanel, Answer? answer)
        {
            DockPanel answerPanel = new DockPanel { Margin = new Thickness(0, 0, 0, 10) };

            CheckBox checkBox = new CheckBox
            {
                IsChecked = answer == null ? false : answer.IsCorrect,
                Content = answer == null ? GetNextOptionLabel(questionPanel) : answer.Option,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(7, 2, 0, 0)
            };

            TextBox txtOption = new TextBox { Width = 430, Text = answer == null ? "" : answer.Text };

            Button deleteButton = new Button
            {
                Content = "X",
                FontSize = 8,
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Center,
                Width = 15,
                Height = 15,
                Margin = new Thickness(0, 0, -15, 0),
            };
            deleteButton.Click += (s, e) =>
            {
                questionPanel.Children.Remove(answerPanel);
                RenumberOptions(questionPanel);
            };

            answerPanel.Children.Add(deleteButton);
            DockPanel.SetDock(deleteButton, Dock.Right);

            answerPanel.Children.Add(checkBox);
            DockPanel.SetDock(checkBox, Dock.Left);
            answerPanel.Children.Add(txtOption);
            DockPanel.SetDock(txtOption, Dock.Right);

            questionPanel.Children.Add(answerPanel);
        }

        public override void AddNonEditableAnswerOption(StackPanel questionPanel, Answer? answer)
        {
            DockPanel answerPanel = new DockPanel { Margin = new Thickness(0, 0, 0, 10) };

            CheckBox checkBox = new CheckBox
            {
                Content = answer == null ? GetNextOptionLabel(questionPanel) : answer.Option,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(7, 1, 0, 0)
            };

            TextBlock txtOption = new TextBlock { Width = 430, Text = answer == null ? "" : answer.Text };

            answerPanel.Children.Add(checkBox);
            DockPanel.SetDock(checkBox, Dock.Left);
            answerPanel.Children.Add(txtOption);
            DockPanel.SetDock(txtOption, Dock.Right);

            questionPanel.Children.Add(answerPanel);
        }

        public override Question CreateQuestionFromUiEntry(GroupBox groupBox)
        {
            var stackPanel = groupBox.Content as StackPanel;
            var txtQuestion = stackPanel.Children.OfType<TextBox>().FirstOrDefault();
            var questionText = txtQuestion?.Text;

            if (string.IsNullOrEmpty(questionText))
            {
                throw new ArgumentException($"Please check question text for {groupBox.Header}. It can not be empty");
            }

            var question = new MultipleChoiceQuestion(questionText);
            foreach (var dockPanel in stackPanel.Children.OfType<DockPanel>())
            {
                var checkBox = dockPanel.Children.OfType<CheckBox>().FirstOrDefault();
                var txtOption = dockPanel.Children.OfType<TextBox>().FirstOrDefault();

                if (checkBox != null && txtOption != null)
                {
                    var option = txtOption.Text;
                    if (string.IsNullOrEmpty(option))
                    {
                        throw new ArgumentException($"Please check answers for {groupBox.Header}. Answers can not be empty");
                    }
                    question.AddAnswer(new Answer(checkBox.Content.ToString()[0], option, checkBox.IsChecked == true));
                }
            }
            checkAnswers(question, 2, groupBox.Header.ToString());

            var correctAnswers = question.Answers.Select(a => a.IsCorrect).Where(correct => correct).ToList().Count();
            if (correctAnswers == 0)
            {
                throw new ArgumentException($"Please check answers for {groupBox.Header}. There should be at least 1 correct answer.");
            }

            return question;
        }

        private void RenumberOptions(StackPanel questionPanel)
        {
            char optionChar = 'A';
            foreach (DockPanel answerPanel in questionPanel.Children.OfType<DockPanel>())
            {
                foreach (var child in answerPanel.Children)
                {
                    if (child is CheckBox checkBox)
                    {
                        checkBox.Content = optionChar.ToString();
                        optionChar++;
                        break;
                    }
                }
            }
        }
    }

    public class HalfCorrectQuestionCreator : QuestionUiCreator
    {

        public override void AddEditableAnswerOption(StackPanel questionPanel, Answer? answer)
        {
            DockPanel answerPanel = new DockPanel { Margin = new Thickness(0, 0, 0, 10) };

            CheckBox checkBox = new CheckBox
            {
                IsChecked = answer == null ? false : answer.IsCorrect,
                Content = answer == null ? GetNextOptionLabel(questionPanel) : answer.Option,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(7, 2, 0, 0)
            };

            TextBox txtOption = new TextBox { Width = 430, Text = answer == null ? "" : answer.Text };

            Button deleteButton = new Button
            {
                Content = "X",
                FontSize = 8,
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Center,
                Width = 15,
                Height = 15,
                Margin = new Thickness(0, 0, -15, 0)
            };
            deleteButton.Click += (s, e) =>
            {
                questionPanel.Children.Remove(answerPanel);
                RenumberOptions(questionPanel);
            };

            answerPanel.Children.Add(deleteButton);
            DockPanel.SetDock(deleteButton, Dock.Right);

            answerPanel.Children.Add(checkBox);
            DockPanel.SetDock(checkBox, Dock.Left);
            answerPanel.Children.Add(txtOption);
            DockPanel.SetDock(txtOption, Dock.Right);

            questionPanel.Children.Add(answerPanel);
        }

        public override void AddNonEditableAnswerOption(StackPanel questionPanel, Answer? answer)
        {
            DockPanel answerPanel = new DockPanel { Margin = new Thickness(0, 0, 0, 10) };

            CheckBox checkBox = new CheckBox
            {
                Content = answer == null ? GetNextOptionLabel(questionPanel) : answer.Option,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(7, 1, 0, 0)
            };

            TextBlock txtOption = new TextBlock { Width = 430, Text = answer == null ? "" : answer.Text };

            answerPanel.Children.Add(checkBox);
            DockPanel.SetDock(checkBox, Dock.Left);
            answerPanel.Children.Add(txtOption);
            DockPanel.SetDock(txtOption, Dock.Right);

            questionPanel.Children.Add(answerPanel);
        }

        public override Question CreateQuestionFromUiEntry(GroupBox groupBox)
        {
            var stackPanel = groupBox.Content as StackPanel;
            var txtQuestion = stackPanel.Children.OfType<TextBox>().FirstOrDefault();
            var questionText = txtQuestion?.Text;

            if (string.IsNullOrEmpty(questionText))
            {
                throw new ArgumentException($"Please check question text for {groupBox.Header}. It can not be empty");
            }

            var question = new HalfCorrectChoiceQuestion(questionText);
            foreach (var dockPanel in stackPanel.Children.OfType<DockPanel>())
            {
                var checkBox = dockPanel.Children.OfType<CheckBox>().FirstOrDefault();
                var txtOption = dockPanel.Children.OfType<TextBox>().FirstOrDefault();

                if (checkBox != null && txtOption != null)
                {
                    var option = txtOption.Text;
                    if (string.IsNullOrEmpty(option))
                    {
                        throw new ArgumentException($"Please check answers for {groupBox.Header}. Answers can not be empty");
                    }

                    question.AddAnswer(new Answer(checkBox.Content.ToString()[0], option, checkBox.IsChecked == true));
                }
            }
            checkAnswers(question, 2, groupBox.Header.ToString());

            var correctAnswers = question.Answers.Select(a => a.IsCorrect).Where(correct => correct).ToList().Count();
            if (correctAnswers == 0)
            {
                throw new ArgumentException($"Please check answers for {groupBox.Header}. There should be at least 1 correct answer.");
            }

            return question;
        }

        private void RenumberOptions(StackPanel questionPanel)
        {
            char optionChar = 'A';
            foreach (DockPanel answerPanel in questionPanel.Children.OfType<DockPanel>())
            {
                foreach (var child in answerPanel.Children)
                {
                    if (child is CheckBox checkBox)
                    {
                        checkBox.Content = optionChar.ToString();
                        optionChar++;
                        break;
                    }
                }
            }
        }
    }

    public class OpenEndedQuestionCreator : QuestionUiCreator
    {

        public override void AddEditableAnswerOption(StackPanel questionPanel, Answer? answer)
        {
            TextBox answerTextBox = new TextBox
            {
                Width = 430,
                Margin = new Thickness(10, 10, 10, 10),
                Text = answer == null ? "Enter correct answer here" : answer.Text
            };

            questionPanel.Children.Add(answerTextBox);
        }

        public override void AddNonEditableAnswerOption(StackPanel questionPanel, Answer? answer)
        { 
            TextBox answerTextBlock = new TextBox
            {
                Width = 430,
                Margin = new Thickness(10, 10, 10, 10),
                Text = "Insert your answer here"
            };

            questionPanel.Children.Add(answerTextBlock);
        }

        public override Question CreateQuestionFromUiEntry(GroupBox groupBox)
        {
            var stackPanel = groupBox.Content as StackPanel;
            var txtQuestion = stackPanel.Children.OfType<TextBox>().FirstOrDefault();
            var questionText = txtQuestion?.Text;

            if (string.IsNullOrEmpty(questionText))
            {
                throw new ArgumentException($"Please check question text for {groupBox.Header}. It can not be empty");
            }

            var question = new OpenEndedQuestion(questionText);

            var answerTextBox = stackPanel.Children.OfType<TextBox>().Skip(1).FirstOrDefault();
            if (answerTextBox != null)
            {
                string answer = answerTextBox.Text.Trim().ToLower();
                if (string.IsNullOrEmpty(answer))
                {
                    throw new ArgumentException($"Please check answers for {groupBox.Header}. Answer can not be empty");
                }

                question.AddAnswer(new Answer(answer));
            }
            checkAnswers(question, 1, groupBox.Header.ToString());

            return question;
        }

        protected override void AddAnswerControls(StackPanel questionPanel, Question question)
        {
            if (question == null)
            {
                AddAnswerOption(questionPanel);
            }
        }
    }

    public class ChronologyQuestionCreator : QuestionUiCreator
    {
        public override void AddEditableAnswerOption(StackPanel questionPanel, Answer? answer)
        {
            DockPanel answerPanel = new DockPanel { Margin = new Thickness(0, 0, 0, 10) };

            Label optionLabel = new Label
            {
                Width = 30,
                Padding = new Thickness(5, 0, 0, 0),
                Content = answer == null ? GetNextOptionLabel(questionPanel) : answer.Option,
                Margin = new Thickness(7, 2, 5, 0)
            }; 


            TextBox answerTextBox = new TextBox
            {
                Width = 390,
                Text = answer == null ? "" : answer.Text,
                Margin = new Thickness(0, 2, 5, 0),
                Tag = "Answer"
            };

            NumericTextBox orderTextBox = new NumericTextBox
            {
                Width = 30,
                Text = answer == null ? "" : answer.Order.ToString(),
                Margin = new Thickness(7, 2, 5, 0),
                VerticalAlignment = VerticalAlignment.Center,
                Tag = "Order"
            };

            Button deleteButton = new Button
            {
                Content = "X",
                FontSize = 8,
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Center,
                Width = 15,
                Height = 15,
                Margin = new Thickness(0, 0, -15, 0)
            };
            deleteButton.Click += (s, e) =>
            {
                questionPanel.Children.Remove(answerPanel);
                RenumberOptions(questionPanel);
            };

            answerPanel.Children.Add(deleteButton);
            DockPanel.SetDock(deleteButton, Dock.Right);

            answerPanel.Children.Add(optionLabel);
            DockPanel.SetDock(optionLabel, Dock.Left);


            answerPanel.Children.Add(orderTextBox);
            DockPanel.SetDock(orderTextBox, Dock.Right);

            answerPanel.Children.Add(answerTextBox);
            DockPanel.SetDock(answerTextBox, Dock.Right);

            questionPanel.Children.Add(answerPanel);
        }

        public override void AddNonEditableAnswerOption(StackPanel questionPanel, Answer? answer)
        {
            DockPanel answerPanel = new DockPanel { Margin = new Thickness(0, 0, 0, 10) };

            Label optionLabel = new Label
            {
                Width = 30,
                Padding = new Thickness(5, 0, 0, 0),
                Content = answer == null ? GetNextOptionLabel(questionPanel) : answer.Option,
                Margin = new Thickness(7, 2, 5, 0)
            };


            TextBlock answerTextBox = new TextBlock
            {
                Width = 390,
                Text = answer == null ? "" : answer.Text,
                Margin = new Thickness(0, 2, 5, 0),
                Tag = "Answer"
            };

            NumericTextBox orderTextBox = new NumericTextBox
            {
                Width = 30,
                Margin = new Thickness(7, 2, 5, 0),
                VerticalAlignment = VerticalAlignment.Center,
                Tag = "Order"
            };

            answerPanel.Children.Add(optionLabel);
            DockPanel.SetDock(optionLabel, Dock.Left);


            answerPanel.Children.Add(orderTextBox);
            DockPanel.SetDock(orderTextBox, Dock.Right);

            answerPanel.Children.Add(answerTextBox);
            DockPanel.SetDock(answerTextBox, Dock.Right);

            questionPanel.Children.Add(answerPanel);
        }

        public override Question CreateQuestionFromUiEntry(GroupBox groupBox)
        {
            var stackPanel = groupBox.Content as StackPanel;
            var txtQuestion = stackPanel.Children.OfType<TextBox>().FirstOrDefault();
            var questionText = txtQuestion?.Text;

            if (string.IsNullOrEmpty(questionText))
            {
                throw new ArgumentException($"Please check question text for {groupBox.Header}. It can not be empty");
            }

            var question = new ChronologyQuestion(questionText);
            foreach (var dockPanel in stackPanel.Children.OfType<DockPanel>())
            {
                var optionLabel = dockPanel.Children.OfType<Label>().FirstOrDefault();
                var answerTextBox = dockPanel.Children.OfType<TextBox>()
                    .Where(tb => tb.Tag != null && tb.Tag.ToString() == "Answer")
                    .FirstOrDefault();
                var orderTextBox = dockPanel.Children.OfType<TextBox>()
                    .Where(tb => tb.Tag != null && tb.Tag.ToString() == "Order")
                    .FirstOrDefault(); ;

                if (orderTextBox != null && answerTextBox != null)
                {
                    char option = optionLabel.Content.ToString()[0];
                    var answerText = answerTextBox.Text;

                    if (string.IsNullOrEmpty(orderTextBox.Text) || string.IsNullOrEmpty(answerText))
                    {
                        throw new ArgumentException($"Please check answers for {groupBox.Header}. Answer can not be empty");
                    }
                    var order = int.Parse(orderTextBox.Text);

                    question.AddAnswer(new Answer(option, answerText, order));
                }
            }
            checkAnswers(question, 2, groupBox.Header.ToString());

            return question;
        }


        private void RenumberOptions(StackPanel questionPanel)
        {
            char optionChar = 'A';
            foreach (DockPanel answerPanel in questionPanel.Children.OfType<DockPanel>())
            {
                foreach (var child in answerPanel.Children)
                {
                    if (child is Label label)
                    {
                        label.Content = optionChar.ToString();
                        optionChar++;
                        break;
                    }
                }
            }
        }
    }

    public class MatchingQuestionCreator : QuestionUiCreator
    {

        public override GroupBox createNonEditableQuestionBox(Question question, int item)
        {
            GroupBox groupBox = new GroupBox { Header = $"Question {item + 1}", Width = 500, Margin = new Thickness(10), BorderThickness = new Thickness(1) };
            StackPanel questionPanel = CreateNonEditableQuestionPanel(question.QuestionText, item, groupBox);

            Random rng = new Random();
            var randomizedMatchingAnswers = question.Answers
                .Select(a => a.MatchingAnswer)
                .OrderBy(item => rng.Next())
                .ToList();

            for (int i = 0; i < question.Answers.Count; i++)
            {
                Answer answer = new Answer(question.Answers[i].Option, question.Answers[i].Text, randomizedMatchingAnswers[i]);
                AddNonEditableAnswerOption(questionPanel, answer);
            }

            groupBox.Content = questionPanel;
            return groupBox;
        }

        public override void AddEditableAnswerOption(StackPanel questionPanel, Answer? answer)
        {
            StackPanel answerPanel = new StackPanel { Margin = new Thickness(0, 0, 0, 10), Orientation = Orientation.Horizontal };

            DockPanel leftPanel = new DockPanel { Margin = new Thickness(0, 0, 0, 10), Tag = "Left" };
            Label optionLabel = new Label
            {
                Width = 30,
                Padding = new Thickness(5, 0, 0, 0),
                Content = answer == null ? GetNextOptionLabel(questionPanel) : answer.Option,
                Margin = new Thickness(7, 2, 5, 0)
            }; 
            TextBox answerTextBox = new TextBox
            {
                Width = 190,
                Text = answer == null ? "" : answer.Text,
                Margin = new Thickness(0, 2, 5, 0),
                Tag = "Question"
            };

            DockPanel rightPanel = new DockPanel { Margin = new Thickness(0, 0, 0, 10), Tag = "Right" };
            Label optionMatchingLabel = new Label
            {
                Width = 30,
                Padding = new Thickness(5, 0, 0, 0),
                Content = answer == null ? GetNextOptionLabel(questionPanel) : answer.Option,
                Margin = new Thickness(7, 2, 5, 0)
            }; 
            TextBox answerMatchingTextBox = new TextBox
            {
                Width = 190,
                Text = answer == null ? "" : answer.MatchingAnswer,
                Margin = new Thickness(0, 2, 5, 0)
            };

            leftPanel.Children.Add(optionLabel);
            leftPanel.Children.Add(answerTextBox);
            DockPanel.SetDock(optionLabel, Dock.Left);
            DockPanel.SetDock(answerTextBox, Dock.Right);

            rightPanel.Children.Add(optionMatchingLabel);
            rightPanel.Children.Add(answerMatchingTextBox);
            DockPanel.SetDock(optionMatchingLabel, Dock.Left);
            DockPanel.SetDock(answerMatchingTextBox, Dock.Right);


            answerPanel.Children.Add(leftPanel);
            answerPanel.Children.Add(rightPanel);

            Button deleteButton = new Button
            {
                Content = "X",
                FontSize = 8,
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Center,
                Width = 15,
                Height = 15,
                Margin = new Thickness(0, -8, -28, 0)
            };
            deleteButton.Click += (s, e) =>
            {
                questionPanel.Children.Remove(answerPanel);
                RenumberOptions(questionPanel);
            };

            answerPanel.Children.Add(deleteButton);

            questionPanel.Children.Add(answerPanel);
        }

        public override void AddNonEditableAnswerOption(StackPanel questionPanel, Answer? answer)
        {
            StackPanel answerPanel = new StackPanel { Margin = new Thickness(0, 0, 0, 10), Orientation = Orientation.Horizontal };

            DockPanel leftPanel = new DockPanel { Margin = new Thickness(0, 0, 0, 10), Tag = "Left" };
            Label optionLabel = new Label
            {
                Width = 30,
                Padding = new Thickness(5, 0, 0, 0),
                Content = answer == null ? GetNextOptionLabel(questionPanel) : answer.Option,
                Margin = new Thickness(7, 2, 5, 0)
            };
            TextBlock answerOptionTextBox = new TextBlock
            {
                Width = 190,
                Text = answer == null ? "" : answer.Text,
                Margin = new Thickness(0, 2, 5, 0),
                Tag = "Question"
            };

            DockPanel rightPanel = new DockPanel { Margin = new Thickness(0, 0, 0, 10), Tag = "Right" };
            TextBox optionMatchingTextBox = new TextBox
            {
                Width = 30,
                Padding = new Thickness(5, 0, 0, 0),
                Margin = new Thickness(7, 2, 5, 0)
            };
            TextBlock answerMatchingTextBlock = new TextBlock
            {
                Width = 190,
                Text = answer == null ? "" : answer.MatchingAnswer,
                Margin = new Thickness(0, 2, 5, 0),
                Tag = "Answer"
            };

            leftPanel.Children.Add(optionLabel);
            leftPanel.Children.Add(answerOptionTextBox);
            DockPanel.SetDock(optionLabel, Dock.Left);
            DockPanel.SetDock(answerOptionTextBox, Dock.Right);

            rightPanel.Children.Add(optionMatchingTextBox);
            rightPanel.Children.Add(answerMatchingTextBlock);
            DockPanel.SetDock(optionMatchingTextBox, Dock.Left);
            DockPanel.SetDock(answerMatchingTextBlock, Dock.Right);

            answerPanel.Children.Add(leftPanel);
            answerPanel.Children.Add(rightPanel);

            questionPanel.Children.Add(answerPanel);
        }

        public override Question CreateQuestionFromUiEntry(GroupBox groupBox)
        {
            var stackPanel = groupBox.Content as StackPanel;
            var txtQuestion = stackPanel.Children.OfType<TextBox>().FirstOrDefault();

            var question = new MatchingQuestion(txtQuestion?.Text);

            if (string.IsNullOrEmpty(txtQuestion?.Text))
            {
                throw new ArgumentException($"Please check question text for {groupBox.Header}. It can not be empty");
            }

            foreach (var answerPanel in stackPanel.Children.OfType<StackPanel>())
            {
                var leftPanel = answerPanel.Children.OfType<DockPanel>()
                    .Where(dp => dp.Tag.Equals("Left")).First();

                var optionLabel = leftPanel.Children.OfType<Label>().FirstOrDefault();
                var answerTextBox = leftPanel.Children.OfType<TextBox>().FirstOrDefault();

                var rightPanel = answerPanel.Children.OfType<DockPanel>()
                    .Where(dp => dp.Tag.Equals("Right")).First();

                var matchingAnswerBox = rightPanel.Children.OfType<TextBox>().FirstOrDefault();

                char option = optionLabel.Content.ToString()[0];
                var answerText = answerTextBox.Text;

                if (string.IsNullOrEmpty(matchingAnswerBox.Text) || string.IsNullOrEmpty(answerText))
                {
                    throw new ArgumentException($"Please check answers for {groupBox.Header}. Answer can not be empty");
                }
                question.AddAnswer(new Answer(option, answerText, matchingAnswerBox.Text));
            }
            checkAnswers(question, 2, groupBox.Header.ToString());

            return question;
        }


        private void RenumberOptions(StackPanel questionPanel)
        {
            char optionChar = 'A';
            foreach (var answerPanel in questionPanel.Children.OfType<StackPanel>())
            {
                foreach (DockPanel dockPanel in answerPanel.Children.OfType<DockPanel>())
                {
                    foreach (var child in dockPanel.Children)
                    {
                        if (child is Label label)
                        {
                            label.Content = optionChar.ToString();
                            break;
                        }
                    }
                }
                optionChar++;

            }
        }
    }
}

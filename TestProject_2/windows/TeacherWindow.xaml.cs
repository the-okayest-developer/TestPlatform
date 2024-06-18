using Microsoft.Win32;
using Newtonsoft.Json;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using TestProject_2.model;

namespace TestProject_2.windows
{
    public partial class TeacherWindow : Window
    {
        private Dictionary<QuestionType, QuestionUiCreator> questionCreators = new Dictionary<QuestionType, QuestionUiCreator>()
        {
            { QuestionType.SINGLE_OPTION, new SingleChoiceQuestionCreator() },
            { QuestionType.MULTIPLE_OPTION, new MultipleChoiceQuestionCreator() },
            { QuestionType.HALF_CORRECT, new HalfCorrectQuestionCreator() },
            { QuestionType.OPEN, new OpenEndedQuestionCreator() },
            { QuestionType.CHRONOLOGY, new ChronologyQuestionCreator() },
            { QuestionType.MATCHING, new MatchingQuestionCreator() }
        };

        private Dictionary<GroupBox, QuestionType> questionBoxes = new Dictionary<GroupBox, QuestionType>();

        public TeacherWindow()
        {
            InitializeComponent();

            string[] availableQuestionTypes = questionCreators.Keys.Select(type => type.GetLabel()).ToArray();
            questionTypeBox.ItemsSource = availableQuestionTypes;
            questionTypeBox.SelectedIndex = 0; 

        }

        private void onWindow_Closing(object sender, EventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
        }


        private void addQuestionButton_Click(object sender, RoutedEventArgs e)
        {
            QuestionType selectedQuestionType = QuestionTypeExtensions.GetQuestionTypeByLabel(questionTypeBox.SelectedItem as string);
            GroupBox questionBox = questionCreators[selectedQuestionType].createEditableQuestionBox(testQuestionsPanel.Children.Count);
            testQuestionsPanel.Children.Add(questionBox);

            questionBoxes.Add(questionBox, selectedQuestionType);
        }

        private void saveTestButton_Click(object sender, RoutedEventArgs e)
        {
            Test test = new Test(testNameTextBox.Text);
            foreach (var questionBox in testQuestionsPanel.Children.OfType<GroupBox>())
            {
                Question question = questionCreators[questionBoxes[questionBox]].CreateQuestionFromUiEntry(questionBox);
                test.AddQuestion(question);
            }

            string json = JsonConvert.SerializeObject(test, Newtonsoft.Json.Formatting.Indented);
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*";
            saveFileDialog.Title = "Save Test File";
            saveFileDialog.FileName = $"{test.Name}.json";
            if (saveFileDialog.ShowDialog() == true)
            {
                string filePath = saveFileDialog.FileName;

                // Save the JSON to the selected file path
                File.WriteAllText(filePath, json);

                MessageBox.Show($"Test saved successfully to: {filePath}", "Save Successful", MessageBoxButton.OK, MessageBoxImage.Information);
            }

        }

        private void openTestButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*";
            openFileDialog.FilterIndex = 1;
            openFileDialog.RestoreDirectory = true;

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    testQuestionsPanel.Children.Clear();
                    questionBoxes.Clear();

                    string serializedTest = File.ReadAllText(openFileDialog.FileName);
                    Test loadedTest = JsonConvert.DeserializeObject<Test>(serializedTest);

                    testNameTextBox.Text = loadedTest.Name;
                    for (int i = 0; i < loadedTest.Questions.Count; i++)
                    {
                        Question question = loadedTest.Questions[i];
                        GroupBox questionBox = questionCreators[question.QuestionType].createEditableQuestionBox(question, i);
                        questionBoxes.Add(questionBox, question.QuestionType);
                        testQuestionsPanel.Children.Add(questionBox);
                    }

                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error loading test: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {

        }
    }
}

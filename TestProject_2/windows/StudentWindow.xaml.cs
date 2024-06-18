using Microsoft.Win32;
using Newtonsoft.Json;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using TestProject_2.model;

namespace TestProject_2.windows
{
    public partial class StudentWindow : Window
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

        private QuestionVisitor questionVisitor = new QuizModeVisitor();

        private Test loadedTest;

        private Dictionary<Question, GroupBox> testQuestions = new Dictionary<Question, GroupBox>();

        public StudentWindow()
        {
            InitializeComponent();
        }

        private void onWindow_Closing(object sender, EventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
        }

        private void onWindow_Loaded(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*";
            openFileDialog.FilterIndex = 1;
            openFileDialog.RestoreDirectory = true;

            if (openFileDialog.ShowDialog() == true)
            {
                try
                { 
                    string serializedTest = File.ReadAllText(openFileDialog.FileName);
                    loadedTest = JsonConvert.DeserializeObject<Test>(serializedTest);

                    testNameTextBox.Text = loadedTest.Name;
                    for (int i = 0; i < loadedTest.Questions.Count; i++)
                    {
                        Question question = loadedTest.Questions[i];
                        GroupBox questionBox = questionCreators[question.QuestionType].createNonEditableQuestionBox(question, i);
                        testQuestionsPanel.Children.Add(questionBox);
                        testQuestions.Add(question, questionBox);
                    }

                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error loading test: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        private void submitAnswersButton_Click(object sender, RoutedEventArgs e)
        {
            int correct = 0;

            try
            {
                foreach (var question in testQuestions.Keys)
                {
                    correct += question.Accept(questionVisitor, testQuestions[question]);
                }
                MessageBox.Show($"Test is finished! You answered {correct} correctly out of {testQuestions.Keys.Count}");
                this.Close();
            }
            catch (Exception ex) 
            {
                MessageBox.Show($"Error evaluating test results: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            
        }
    }
}

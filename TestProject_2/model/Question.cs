using System.Windows.Controls;

namespace TestProject_2.model
{
    public abstract class Question
    {
        public string QuestionText { get; set; }
        public List<Answer> Answers { get; set; }

        public QuestionType QuestionType { get; set; }

        public Question(string questionText, QuestionType questionType)
        {
            QuestionText = questionText;
            Answers = new List<Answer>();
            QuestionType = questionType;
        }

        public Question(QuestionType questionType)
        {
            Answers = new List<Answer>();
            QuestionType = questionType;
        }

        public string GetQuestionText()
        {
            return QuestionText;
        }
        public string SetQuestionText(string text)
        {
            return this.QuestionText = text;
        }
        
        public List<Answer> GetAnswers()
        {
            return this.Answers;
        }

        public virtual void AddAnswer(Answer answer)
        {
            Answers.Add(answer);
        }

        public bool CheckAnswer(char userAnswer)
        {
            var answer = Answers.FirstOrDefault(a => a.Option == userAnswer);
            return answer != null && answer.IsCorrect;
        }

        public abstract int Accept(QuestionVisitor visitor, GroupBox questionBox);
    }

    public class SingleChoiceQuestion : Question
    {
        public SingleChoiceQuestion(string questionText) : base(questionText, QuestionType.SINGLE_OPTION) {}
        public SingleChoiceQuestion() : base(QuestionType.SINGLE_OPTION) {}

        public override int Accept(QuestionVisitor visitor, GroupBox questionBox)
        {
            return visitor.Visit(this, questionBox);
        }
    }

    public class MultipleChoiceQuestion : Question
    {
        public MultipleChoiceQuestion(string questionText) : base(questionText, QuestionType.MULTIPLE_OPTION) {}
        public MultipleChoiceQuestion() : base(QuestionType.MULTIPLE_OPTION) {}

        public bool CheckAnswer(List<char> userAnswers)
        {
            if (userAnswers.Count != CountCorrectAnswers())
                return false;

            return userAnswers.All(a => IsCorrectAnswer(a));
        }

        private int CountCorrectAnswers()
        {
            return GetAnswers().Count(a => a.IsCorrect);
        }

        private bool IsCorrectAnswer(char userAnswer)
        {
            return GetAnswers().Any(a => a.Option == userAnswer && a.IsCorrect);
        }

        public override int Accept(QuestionVisitor visitor, GroupBox questionBox)
        {
            return visitor.Visit(this, questionBox);
        }
    }

    public class HalfCorrectChoiceQuestion : Question
    {
        public HalfCorrectChoiceQuestion(string questionText) : base(questionText, QuestionType.HALF_CORRECT)
        {
        }

        public HalfCorrectChoiceQuestion() : base(QuestionType.HALF_CORRECT)
        {
        }

        public bool CheckAnswer(List<char> userAnswers)
        {
            int numCorrectAnswers = CountCorrectAnswers();
            int minCorrectAnswersRequired = numCorrectAnswers / 2 + numCorrectAnswers % 2;

            if (userAnswers.Count < minCorrectAnswersRequired)
                return false;

            int correctAnswersSelected = userAnswers.Count(a => IsCorrectAnswer(a));

            return correctAnswersSelected >= minCorrectAnswersRequired;
        }

        private int CountCorrectAnswers()
        {
            return GetAnswers().Count(a => a.IsCorrect);
        }

        private bool IsCorrectAnswer(char userAnswer)
        {
            return GetAnswers().Any(a => a.Option == userAnswer && a.IsCorrect);
        }

        public override int Accept(QuestionVisitor visitor, GroupBox questionBox)
        {
            return visitor.Visit(this, questionBox);
        }
    }

    public class OpenEndedQuestion : Question
    {

        public OpenEndedQuestion() : base(QuestionType.OPEN)
        {
        }
        public OpenEndedQuestion(string questionText) : base(questionText, QuestionType.OPEN)
        {
        }


        public bool CheckAnswer(string userAnswer)
        {
            if (Answers.Count == 0)
            {
                throw new InvalidOperationException("There's no correct answer");
            }

            return Answers[0].Text.Trim().ToLower() == userAnswer.Trim().ToLower();
        }

        public virtual void AddAnswer(Answer answer)
        {
            if (Answers.Count > 0)
            {
                throw new InvalidOperationException("Cannot add more that 1 correct answer");
            }
            Answers.Add(answer);
        }

        public override int Accept(QuestionVisitor visitor, GroupBox questionBox)
        {
            return visitor.Visit(this, questionBox);
        }
    }

    public class ChronologyQuestion : Question
    {
        public ChronologyQuestion() : base(QuestionType.CHRONOLOGY)
        {
        }
        public ChronologyQuestion(string questionText) : base(questionText, QuestionType.CHRONOLOGY)
        {
        }

        public bool CheckAnswer(List<char> userAnswers)
        {
            List<char> correctOrder = Answers
                .OrderBy(answer => answer.Order)
                .Select(answer => answer.Option)
                .ToList();
            return userAnswers.SequenceEqual(correctOrder);
        }

        public override int Accept(QuestionVisitor visitor, GroupBox questionBox)
        {
            return visitor.Visit(this, questionBox);
        }
    }

    public class MatchingQuestion : Question
    {
        public MatchingQuestion() : base(QuestionType.MATCHING)
        {
        }
        public MatchingQuestion(string questionText) : base(questionText, QuestionType.MATCHING)
        {
        }

        public virtual void AddAnswer(Answer answer)
        {
            if (answer.MatchingAnswer == null)
            {
                throw new InvalidOperationException("MatchingQuestion has to have a matching answer");
            }
            Answers.Add(answer);
        }

        public bool CheckAnswer(Dictionary<string, string> userPairs)
        {
            foreach (Answer correctAnswer in this.Answers)
            {
                if (!correctAnswer.MatchingAnswer.Equals(userPairs[correctAnswer.Text]))
                {
                    return false;
                }
            }
            return true;
        }


        public override int Accept(QuestionVisitor visitor, GroupBox questionBox)
        {
            return visitor.Visit(this, questionBox);
        }
    }
}

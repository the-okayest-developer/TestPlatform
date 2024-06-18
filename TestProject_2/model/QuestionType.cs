namespace TestProject_2.model
{
    public enum QuestionType
    {
        SINGLE_OPTION,
        MULTIPLE_OPTION,
        HALF_CORRECT,
        OPEN,
        CHRONOLOGY,
        MATCHING 

    }

    public static class QuestionTypeExtensions
    {
        public static string GetLabel(this QuestionType questionType)
        {
            switch (questionType)
            {
                case QuestionType.SINGLE_OPTION:
                    return "Single Option";
                case QuestionType.MULTIPLE_OPTION:
                    return "Multiple Option";
                case QuestionType.HALF_CORRECT:
                    return "Half Correct Question";
                case QuestionType.OPEN:
                    return "Open question";
                case QuestionType.CHRONOLOGY:
                    return "Chronology Question";
                case QuestionType.MATCHING:
                    return "Matching question";
                default:
                    throw new ArgumentException($"Unsupported question type: {questionType}");
            }
        }
        public static QuestionType GetQuestionTypeByLabel(string label)
        {
            switch (label)
            {
                case "Single Option":
                    return QuestionType.SINGLE_OPTION;
                case "Multiple Option":
                    return QuestionType.MULTIPLE_OPTION;
                case "Half Correct Question":
                    return QuestionType.HALF_CORRECT;
                case "Open question":
                    return QuestionType.OPEN;
                case "Chronology Question":
                    return QuestionType.CHRONOLOGY;
                case "Matching question":
                    return QuestionType.MATCHING;
                default:
                    throw new ArgumentException($"Unsupported label: {label}");
            }
        }
    }
}

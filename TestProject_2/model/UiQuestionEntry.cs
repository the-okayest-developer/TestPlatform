using System.Windows.Controls;

namespace TestProject_2.model
{
    public class UiQuestionEntry
    {
        private GroupBox GroupBox;
        private QuestionType QuestionType;

        public UiQuestionEntry(GroupBox groupBox, QuestionType questionType)
        {
            GroupBox = groupBox;
            QuestionType = questionType;
        }

        public GroupBox GetGroupBox()
        {
            return GroupBox;
        }
        public QuestionType GetQuestionType()
        {
            return QuestionType;
        }
    }
}

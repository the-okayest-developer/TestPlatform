namespace TestProject_2.model
{
    public class Answer
    { 
        public char Option { get; set; }
        public string Text { get; set; }
        public bool IsCorrect { get; set; }
        public int Order { get; set; }

        public string MatchingAnswer { get; set; }

        public Answer() { }

        public Answer(string text)
        {
            Text = text;
        }
        public Answer(char option, string text, bool isCorrect)
        {
            Option = option;
            Text = text;
            IsCorrect = isCorrect;
        }     

        public Answer(char option, string text, int order)
        {
            Option = option;
            Text = text;
            Order = order;
        }     

        public Answer(char option, string text, string matchingAnswer)
        {
            Option = option;
            Text = text;
            MatchingAnswer = matchingAnswer;
        }     
        
    }
}

using Newtonsoft.Json;

namespace TestProject_2.model
{
    public class Test
    {
        public string Name { get; set; }
        
        [JsonConverter(typeof(QuestionConverter))]
        public List<Question> Questions { get; set; }

        public Test(string name)
        {
            Name = name;
            Questions = new List<Question>();
        }

        public void AddQuestion(Question question)
        {
            Questions.Add(question);
        }
    }
}

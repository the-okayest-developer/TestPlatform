using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace TestProject_2.model
{
    public class QuestionConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return typeof(Question).IsAssignableFrom(objectType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JArray array = JArray.Load(reader);
            List<Question> questions = new List<Question>();

            foreach (var item in array)
            {
                QuestionType type = item["QuestionType"].ToObject<QuestionType>();

                Question question;
                switch (type)
                {
                    case QuestionType.SINGLE_OPTION:
                        question = new SingleChoiceQuestion();
                        break;
                    case QuestionType.MULTIPLE_OPTION:
                        question = new MultipleChoiceQuestion();
                        break;
                    case QuestionType.HALF_CORRECT:
                        question = new HalfCorrectChoiceQuestion();
                        break;
                    case QuestionType.OPEN:
                        question = new OpenEndedQuestion();
                        break;
                    case QuestionType.CHRONOLOGY:
                        question = new ChronologyQuestion();
                        break;
                    case QuestionType.MATCHING:
                        question = new MatchingQuestion();
                        break;
                    default:
                        throw new NotSupportedException($"Question type '{type}' is not supported");
                }

                serializer.Populate(item.CreateReader(), question);
                questions.Add(question);
            }

            return questions;
        }
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value is Question)
            {
                JObject jo = JObject.FromObject(value, serializer);
                jo.WriteTo(writer);
            }
            else if (value is IEnumerable<Question> questionList)
            {
                JArray array = JArray.FromObject(questionList, serializer);
                array.WriteTo(writer);
            }
            else
            {
                throw new JsonSerializationException("Unexpected value type: " + value.GetType());
            }
        }
    }
}

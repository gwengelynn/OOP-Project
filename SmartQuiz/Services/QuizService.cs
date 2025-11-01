namespace SmartQuiz.Services
{
    public class QuizService
    {
        public List<Quiz> Quizzes { get; set; } = new();

        public void AddQuiz(Quiz quiz)
        {
            Quizzes.Add(quiz);
        }

        public void UpdateQuiz(Quiz quiz)
        {
            var existing = Quizzes.FirstOrDefault(q => q.Id == quiz.Id);
            if (existing != null)
            {
                existing.Title = quiz.Title;
                existing.Description = quiz.Description;
                existing.Questions = quiz.Questions;
            }
            else
            {
                Quizzes.Add(quiz);
            }
        }

        public Quiz? GetQuizById(int id)
        {
            return Quizzes.FirstOrDefault(q => q.Id == id);
        }

        public void DeleteQuiz(int id)
        {
            var quiz = Quizzes.FirstOrDefault(q => q.Id == id);
            if (quiz != null)
                Quizzes.Remove(quiz);
        }
    }

    // ✅ Make sure these two classes exist:
    public class Quiz
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public List<Question> Questions { get; set; } = new();
    }

    public class Question
    {
        public int Id { get; set; }
        public string QuestionText { get; set; } = "";
        public string Type { get; set; } = "Multiple Choice"; // Default type
        public List<string> Options { get; set; } = new();
        public int CorrectAnswerIndex { get; set; } = -1;
        public string CorrectAnswer { get; set; } = "";
    }
}

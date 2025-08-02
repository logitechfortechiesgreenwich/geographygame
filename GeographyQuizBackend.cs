using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Geography_game
{
    // Abstract base class for all questions
    public abstract class Question
    {
        public string Text { get; set; }
        public string Category { get; set; }
        public string? ImagePath { get; set; }

        protected Question(string text, string category, string? imagePath = null)
        {
            Text = text;
            Category = category;
            ImagePath = imagePath;
        }

        public abstract bool CheckAnswer(string userAnswer);
        public abstract string GetCorrectAnswer();
    }

    // Multiple-choice question class
    public class MultipleChoiceQuestion : Question
    {
        public List<string> Options { get; set; }
        public string CorrectAnswer { get; set; }

        public MultipleChoiceQuestion(string text, string category, List<string> options, string correctAnswer, string? imagePath = null)
            : base(text, category, imagePath)
        {
            if (options.Count != 4)
                throw new ArgumentException("Multiple-choice questions must have exactly 4 options.");
            Options = options;
            CorrectAnswer = correctAnswer;
        }

        public override bool CheckAnswer(string userAnswer)
        {
            return userAnswer.Trim().Equals(CorrectAnswer.Trim(), StringComparison.OrdinalIgnoreCase);
        }

        public override string GetCorrectAnswer()
        {
            return CorrectAnswer;
        }
    }

    // Open-ended question class
    public class OpenEndedQuestion : Question
    {
        public List<string> CorrectAnswers { get; set; }

        public OpenEndedQuestion(string text, string category, List<string> correctAnswers, string? imagePath = null)
            : base(text, category, imagePath)
        {
            if (correctAnswers.Count == 0)
                throw new ArgumentException("Open-ended questions must have at least one correct answer.");
            CorrectAnswers = correctAnswers;
        }

        public override bool CheckAnswer(string userAnswer)
        {
            return CorrectAnswers.Exists(ans => ans.Trim().Equals(userAnswer.Trim(), StringComparison.OrdinalIgnoreCase));
        }

        public override string GetCorrectAnswer()
        {
            return string.Join(" or ", CorrectAnswers);
        }
    }

    // True/False question class
    public class TrueFalseQuestion : Question
    {
        public bool CorrectAnswer { get; set; }

        public TrueFalseQuestion(string text, string category, bool correctAnswer, string? imagePath = null)
            : base(text, category, imagePath)
        {
            CorrectAnswer = correctAnswer;
        }

        public override bool CheckAnswer(string userAnswer)
        {
            // Accept "True"/"False" or "true"/"false" or "1"/"0"
            if (bool.TryParse(userAnswer, out bool result))
                return result == CorrectAnswer;
            if (userAnswer.Trim() == "1" && CorrectAnswer) return true;
            if (userAnswer.Trim() == "0" && !CorrectAnswer) return true;
            return false;
        }

        public override string GetCorrectAnswer()
        {
            return CorrectAnswer ? "True" : "False";
        }
    }
}
using System;

namespace AnalyticsTestClient
{
    internal class Question
    {
        private readonly string _question;
        public Question(string question)
        {
            _question = question;
        }

        public string Ask()
        {
            Console.Write(_question);
            var result = Console.ReadLine();
            Console.WriteLine();

            return result;
        }

        public string AskOnNewLine()
        {
            Console.WriteLine(_question);
            var result = Console.ReadLine();
            Console.WriteLine();

            return result;
        }
    }
}
using System;

namespace BigFiles
{
    internal class Program
    {
        private static void Main()
        {
            Console.WriteLine("file path");

            var dictionary = WordProcessor.GetAllWords(Console.ReadLine());

            Console.WriteLine("file to write words");

            WordProcessor.WriteWords(dictionary, Console.ReadLine());
        }
    }
}
using System;
using System.IO;

namespace BigFiles
{
    internal class Program
    {
        private static void Main()
        {
            Console.WriteLine("file path");

            var filePath = Console.ReadLine();
            if(!File.Exists(filePath))
            {
                Console.WriteLine("file does not exist");
                return;
            }

            var dictionary = WordProcessor.GetAllWords();

            Console.WriteLine("file to write words");

            filePath = Console.ReadLine();
            if (!File.Exists(filePath))
            {
                Console.WriteLine("file does not exist");
                return;
            }

            WordProcessor.WriteWords(dictionary, filePath);

            Console.ReadKey();
        }
    }
}
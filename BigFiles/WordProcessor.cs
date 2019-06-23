using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BigFiles
{
    public static class WordProcessor
    {
        private const string SplitPattern = "[^a-zA-Z0-9]+";

        //async? (main not async)
        public static CustomDictionary GetAllWords(string filePath = "D:\\text.txt")
        {
            var dictionary = new CustomDictionary();

            using (var fs = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (var bs = new BufferedStream(fs))
            using (var sr = new StreamReader(bs))
            {
                string line;
                var lineNumber = 1;

                while ((line = sr.ReadLine()) != null)
                {
                    var words = GetWordsFromLineRegex(line);
                    
                    foreach (var word in words)
                    {
                        dictionary.Add(word, lineNumber);
                    }

                    lineNumber++;
                }
            }

            return dictionary;
        }

        public static void WriteWords(CustomDictionary dictionary, string externalFilePath = "D:\\processedFile.txt")
        {
            dictionary.WriteAllWords(externalFilePath);
        }

        public static IEnumerable<string> GetWordsFromLineRegex(string line)
        {
            return Regex.Split(line, SplitPattern).Select(x => x.ToLower());
        }
    }
}

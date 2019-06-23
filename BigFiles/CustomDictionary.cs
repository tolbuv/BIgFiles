using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace BigFiles
{
    //use indexes instead of linq? (b perf, w code rdblty)
    public class CustomDictionary
    {
        private const string CharacterSet = "1234567890ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";

        public List<Section> Sections { get; set; }

        public CustomDictionary()
        {
            Sections = new List<Section>
            {
                new Section(CharacterSet.First(), CharacterSet.Last())
            };
        }

        public void Add(string key, int line)
        {
            var section = Sections.First(x => x.IsCharacterCovered(key.First()));

            if (!section.IsMaxLength()) section.Add(key, line);
            else
            {
                var midIndex = (CharacterSet.IndexOf(section.FirstRangeCharacter)
                                             + CharacterSet.IndexOf(section.LastRangeCharacter))
                                             / 2;

                var firstSection = new Section(section.FirstRangeCharacter, CharacterSet[midIndex]);
                var secondSection = new Section(CharacterSet[midIndex + 1], section.LastRangeCharacter);

                for (var i = 0; i < section.Entries.Length; i++)
                {
                    if(section.Entries[i] == null) break;

                    if (firstSection.IsCharacterCovered(section.Entries[i].Word.First()))
                        firstSection.AddEntry(section.Entries[i]);
                    else
                        secondSection.AddEntry(section.Entries[i]);
                }


                if (firstSection.IsCharacterCovered(key.First()))
                    firstSection.Add(key, line);
                else
                    secondSection.Add(key, line);

                Sections.Add(firstSection);
                Sections.Add(secondSection);

                Sections.Remove(section);
            }
        }

        public void WriteAllWords(string filePath)
        {
            foreach (var section in Sections.OrderBy(x => x.FirstRangeCharacter))
            {
                section.WriteSection(filePath);
            }
        }

        public class Section
        {
            #region Private constants and variables

            private const int DefaultCapacity = 10000;

            private const int Multiplier = 10;

            private const int MaxArrayLength = 0x7FEFFFFD;

            private int _count;

            private int _capacity;

            #endregion

            #region Public properties

            public char FirstRangeCharacter { get; set; }

            public char LastRangeCharacter { get; set; }

            public Entry[] Entries { get; set; }

            #endregion

            #region Constructors

            public Section(char firstCharacter, char lastCharacter)
                : this(firstCharacter, lastCharacter, DefaultCapacity)
            { }

            public Section(char firstCharacter, char lastCharacter, int capacity)
            {
                FirstRangeCharacter = firstCharacter;
                LastRangeCharacter = lastCharacter;

                _count = 0;
                _capacity = capacity;

                Entries = new Entry[capacity];
            }

            #endregion

            #region Public methods

            public void AddEntry(Entry entry)
            {
                if (_count == _capacity) Resize(GetNewCapacity(_capacity));

                Entries[_count] = entry ?? throw new ArgumentNullException(nameof(entry));
                _count++;
            }

            public void Add(string word, int lineNumber)
            {
                if (word == null || lineNumber == default(int))
                    throw new ArgumentNullException();
                if (!IsCharacterCovered(word.First()))
                    throw new ArgumentOutOfRangeException(nameof(word));
                if (IsMaxLength())
                    throw new InvalidOperationException();

                var index = Array.FindIndex(Entries, 0, _count, x => x.Word == word);

                if (index < 0)
                {
                    if (_count == _capacity) Resize(GetNewCapacity(_capacity));

                    Entries[_count] = new Entry
                    {
                        Word = word,
                        LineNumbers = new Collection<int> { lineNumber }
                    };

                    _count++;
                }
                else
                {
                    Entries[index].LineNumbers.Add(lineNumber);
                }
            }

            public void WriteSection(string filePath)
            {
                File.AppendAllLines(
                    filePath,
                    Entries.Take(_count).OrderBy(x => x.Word)
                        .Select(x => $"{x.Word.ToLower()} {string.Join(" ", x.LineNumbers)}")
                    );
            }

            public bool IsCharacterCovered(char character)
                => (character >= FirstRangeCharacter && character <= LastRangeCharacter);

            public bool IsMaxLength()
                => _count == MaxArrayLength;

            #endregion

            #region Private methods

            private void Resize(int newCapacity)
            {
                if (newCapacity <= Entries.Length) return;

                var newEntries = new Entry[newCapacity];
                Array.Copy(Entries, 0, newEntries, 0, _count);
                Entries = newEntries; 

                _capacity = newCapacity;
            }

            private static int GetNewCapacity(int oldCapacity)
            {
                var newSize = Multiplier * oldCapacity;

                return newSize > MaxArrayLength && MaxArrayLength > oldCapacity
                    ? MaxArrayLength
                    : newSize;
            }

            #endregion

            public class Entry
            {
                public string Word { get; set; }

                public Collection<int> LineNumbers { get; set; }
            }
        }
    }
}

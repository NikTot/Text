using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace FTS.Services
{
    public class ClearTextForOmniDesk
    {
        /// <summary>
        /// Список стоп слов русского языка.
        /// </summary>
        public HashSet<string> russianStopWords;

        /// <summary>
        /// Список стоп слов английского языка.
        /// </summary>
        public HashSet<string> englishStopWords;

        public string CleanUp(string text, List<string> filters)
        {
            var result = "";

            foreach (var item in filters)
            {
                text = Regex.Replace(text ?? "Нет данных", item, string.Empty);
            }

            text = text.ToLower();

            //Разбиваем текст на массив слов
            string[] words = text.Split(new[] { ' ', ',', '.', '!', '?', '\n', '\r', '-' }, StringSplitOptions.RemoveEmptyEntries);
            var pathToRus = AppDomain.CurrentDomain.BaseDirectory + "LibDataWorkItems/RusStopWordsForOmnidesk.txt";
            var pathToEng = AppDomain.CurrentDomain.BaseDirectory + "LibDataWorkItems/EngStopWordsForOmnidesk.txt";

            russianStopWords = new HashSet<string>(File.ReadAllLines(pathToRus)
                .Where(l => !l.StartsWith("#"))
                .Select(l => l.Trim())
                .ToArray());

            englishStopWords = new HashSet<string>(File.ReadAllLines(pathToEng)
                .Where(l => !l.StartsWith("#"))
                .Select(l => l.Trim())
                .ToArray());

            var index = 0;
            foreach (var word in words)
            {
                string _word = word;
                if (_word == "1c" || _word == "1с")
                {
                    _word = "odins";
                }
                _word = Regex.Replace(_word, @"[^а-яa-z]", string.Empty);
                if (russianStopWords.Contains(_word.Trim()))
                    _word = string.Empty;
                if (englishStopWords.Contains(_word.Trim()))
                    _word = string.Empty;

                if (_word == "odins")
                {
                    _word = "1c";
                }

                if (index != words.Length-1)
                {
                    result += String.IsNullOrEmpty(_word) ? _word : _word + ' ';
                }
                else
                {
                    result += String.IsNullOrEmpty(_word) ? _word : _word;
                }

                index++;
            }

            //Убираем лишние пробелы
            result = Regex.Replace(result, @"  +", " ");

            return result;
        }
    }
}

using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace TimeLine.Core
{
    /// <summary>
    /// Handles parsing user text input.
    /// </summary>
    public class CommandParser
    {
        readonly HashSet<string> mainWords = new HashSet<string>() 
        { 
            "timer", "t", 
            "stopwatch", "s", 
            "alarm", "a", 
            "mute", "exit" 
        };
        readonly HashSet<string> operationWords = new HashSet<string>() 
        { 
            "add", "stop", "info",
            "start", "reset", "pause", "close", 
            "clear", "skip" 
        };

        private List<string> wordList;
        private string wordToRemove = "";

        private string mainCommand = "";
        private string operationCommand = "";
        private string hours = "";
        private string minutes = "";
        private string seconds = "";


        /// <summary>
        /// Defines mainCommand, operation and time from input string.
        /// </summary>
        /// <param name="text">String from user</param>
        /// <returns></returns>
        public ParsedCommandData Parse(string text)
        {
            CreateWordList(text);

            ParseProgram();
            if (mainCommand == "exit" || mainCommand == "mute")
                return new ParsedCommandData(mainCommand);
            RemoveWordFromList();
            ChangeOneLetterMainCommand();


            ParseOperation();
            if (operationCommand == "stop" || operationCommand == "pause" || operationCommand == "close")
                return new ParsedCommandData(mainCommand, operationCommand);
            RemoveWordFromList();

            GetTime();

            return new ParsedCommandData(mainCommand, operationCommand, hours, minutes, seconds);
        }

        private void ChangeOneLetterMainCommand()
        {
            if (mainCommand == "t")
                mainCommand = "timer";
            else if (mainCommand == "s")
                mainCommand = "stopwatch";
            else if (mainCommand == "a")
                mainCommand = "alarm";
        }

        private void CreateWordList(string text)
        {
            if (text == null)
                text = "";

            text = text.Trim().ToLower();
            text = Regex.Replace(text, @"\s+", " ");
            wordList = text.Split(' ').ToList();
        }

        private void ParseProgram()
        {
            foreach (var word in wordList)
            {
                if (mainWords.Contains(word))
                {
                    mainCommand = word;
                    wordToRemove = word;
                    break;
                }
            }
        }

        private void ParseOperation()
        {
            foreach (var word in wordList)
            {
                if (operationWords.Contains(word))
                {
                    operationCommand = word;
                    wordToRemove = word;
                    break;
                }
            }
        }

        private void RemoveWordFromList()
        {
            wordList.Remove(wordToRemove);
            wordToRemove = "";
        }

        private void GetTime()
        {
            wordList.Reverse();

            Regex regex = new Regex(@"-?\d*");

            int i = 0;

            foreach (var item in wordList)
            {
                string trimmedItem = item.Trim();
                if (new Regex(@"\d+s", RegexOptions.IgnoreCase).IsMatch(trimmedItem))
                {
                    seconds = regex.Match(trimmedItem).Value;
                    i = 1;
                }
                else if (new Regex(@"\d+m", RegexOptions.IgnoreCase).IsMatch(trimmedItem))
                {
                    minutes = regex.Match(trimmedItem).Value;
                    i = 2;
                }
                else if (new Regex(@"\d+h", RegexOptions.IgnoreCase).IsMatch(trimmedItem))
                {
                    hours = regex.Match(trimmedItem).Value;
                    i = 3;
                }                
                else if (new Regex(@"\d+", RegexOptions.IgnoreCase).IsMatch(trimmedItem))
                {
                    if (i == 0)
                    {
                        seconds = regex.Match(trimmedItem).Value;
                        i = 1;
                    }
                    else if (i == 1)
                    {
                        minutes = regex.Match(trimmedItem).Value;
                        i = 2;
                    }
                    else if (i == 2)
                    {
                        hours = regex.Match(trimmedItem).Value;
                        i = 3;
                    }
                }
            }
        }

    }
}

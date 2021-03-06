﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Input;
using MortuusLogger;
using TimeLine.Models;
using WpfScreenHelper;

namespace TimeLine
{
    public class RunCommandViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public static int suggestionButtonHeight = 42;

        #region Bindable Properties

        public int Left { get; set; }
        public int Top { get; set; }

        public int WindowWidth { get; set; } = 500;
        public int BaseHeight { get; set; } = 70;

        private int suggestionsHeight;
        public int SuggestionsHeight
        {
            get { return suggestionsHeight; }
            set { 
                if (value == 0) {
                    SuggestionsClosing = true;
                    suggestionsHeight = value;
                }                    
                else {
                    SuggestionsClosing = false;
                    suggestionsHeight = value;
                }
            }
        }


        public bool Closing { get; set; }
        public bool SuggestionsClosing { get; set; }

        public string GhostText { get; set; } = "Type a command..";
        public bool GhostTextIsVisible { get; set; } = true;

        private string input;
        public string Input
        {
            get { return input; }
            set { input = value; SetGhostTextVisibility(); GetSuggestions(); }
        }

        public ObservableCollection<Suggestion> Suggestions { get; set; }


        public ICommand ConfirmCommand { get; private set; }
        public ICommand EraseCommand { get; private set; }

        #endregion



        public RunCommandViewModel() {
            ConfirmCommand = new RelayCommand(act => { ConfirmAndClose(); });
            EraseCommand = new RelayCommand(act => { Input = string.Empty; });


            SetWindowPosition();
            Logger.Log($"CommandWindow Position: x:{Left} : y:{Top}", LogLevel.DEBUG);

            GetSuggestions();
        }



        #region Position

        public void SetWindowPosition() {
            Point position = MouseHelper.MousePosition;
            Screen currentScreen = Screen.FromPoint(position);

            Left = GetPositionX(position, currentScreen);
            Top = GetPositionY(position, currentScreen);
        }

        private int GetPositionX(Point position, Screen currentScreen) {

            // If distance between bounds and mouse position is less than half of the window: position stays around bounds.
            if (Math.Abs(currentScreen.Bounds.Left - position.X) < WindowWidth / 2)
                position.X = currentScreen.Bounds.Left + (WindowWidth / 2) * 1.1;
            else if (Math.Abs(currentScreen.Bounds.Right - position.X) < WindowWidth / 2)
                position.X = currentScreen.Bounds.Right - (WindowWidth / 2) * 1.1;


            return (int)position.X - WindowWidth / 2;
        }

        private int GetPositionY(Point position, Screen currentScreen) {

            if (Math.Abs(currentScreen.Bounds.Top - position.Y) < BaseHeight * 2)
                position.Y = currentScreen.Bounds.Top + BaseHeight * 2;
            else if (Math.Abs(currentScreen.Bounds.Bottom - position.Y) < BaseHeight * 4)
                position.Y = currentScreen.Bounds.Bottom - BaseHeight * 4 * 1.1;

            return (int)position.Y - BaseHeight / 2;
        }

        #endregion


        #region Window Actions        

        /// <summary>
        /// Replaces typed letters with Suggestion word.
        /// </summary>
        /// <param name="header"></param>
        public void AppendSuggestion(string header) {

            if (string.IsNullOrEmpty(Input)) {
                Input = header;
            }
            else if (Input[Input.Length - 1] == ' ') {
                Input += header;
            }
            else {
                var commandWords = new List<string>(Input.Trim().ToLower().Split(" "));
                var last = commandWords.FindLast(word => word.Length != 0);

                foreach (var word in commandWords) 
                    if (header.ToLower().StartsWith(word))
                        Input = Input.Replace(word, header);                    
            }            

            if (Input.Length != 0 && Input[Input.Length - 1] != ' ') {
                Input += " ";
            }
        } 


        private void ConfirmAndClose() {
            App.Manager.ToggleRunCommandView();
            App.Manager.ParseInput(Input);
        }

        private void SetGhostTextVisibility() {
            if (string.IsNullOrEmpty(Input))
                GhostTextIsVisible = true;
            else
                GhostTextIsVisible = false;
        }


        #endregion


        #region Suggestions

        HashSet<string> baseSuggestions = new HashSet<string>() { "Timer", "Stopwatch", "Alarm" };

        HashSet<string> timerRunningSuggestions = new HashSet<string>() { "Add", "Stop", "Info" };
        HashSet<string> timerTimeSuggestions = new HashSet<string>() { "3m", "5m", "10m", "15m", "25m", "1h" };

        HashSet<string> stopwatchSuggestions = new HashSet<string>() { "Start", "Reset", "Close" };
        HashSet<string> stopwatchRunningSuggestions = new HashSet<string>() { "Pause", "Reset", "Close" };

        HashSet<string> alarmSuggestions = new HashSet<string>() { "Skip", "Clear", "List" };

        private void GetSuggestions() {

            string lowercaseInput = string.IsNullOrWhiteSpace(Input) ? "" : Input.ToLower();

            HashSet<string> suggestionsSet = new HashSet<string>();

            string lastWord = " ";

            suggestionsSet = DesideSuggestions(lowercaseInput, ref lastWord);

            Suggestions = new ObservableCollection<Suggestion>();


            if (string.IsNullOrWhiteSpace(Input) || Input[Input.Length - 1] == ' ') {
                foreach (var word in suggestionsSet) {
                    Suggestions.Add(new Suggestion(word));
                }
            }
            else {
                foreach (var word in suggestionsSet) {
                    if (word.ToLower().StartsWith(lastWord))
                        Suggestions.Add(new Suggestion(word));
                }
            }


            SetSuggestionsCornerRadius();

            if (Suggestions.Count == 0)
                SuggestionsClosing = true;
            else
                SuggestionsClosing = false;

            SuggestionsHeight = Suggestions.Count > 0 ? (Suggestions.Count * suggestionButtonHeight) + 2 : suggestionsHeight;

        }

        private HashSet<string> DesideSuggestions(string lowercaseInput, ref string lastWord) {            
            HashSet<string> suggestionsSet;

            if (string.IsNullOrWhiteSpace(Input)) {
                suggestionsSet = baseSuggestions;
            }
            else {
                lastWord = new List<string>(lowercaseInput.Split(" ")).FindLast(word => word.Length != 0);

                if (lowercaseInput.Contains("timer ") || lowercaseInput.Contains("t "))
                    suggestionsSet = App.Manager.TimerIsRunning ? timerRunningSuggestions : timerTimeSuggestions;
                else if (lowercaseInput.Contains("stopwatch ") || lowercaseInput.Contains("s "))
                    suggestionsSet = App.Manager.StopwatchRunning ? stopwatchRunningSuggestions : stopwatchSuggestions;
                else if (lowercaseInput.Contains("alarm ") || lowercaseInput.Contains("a "))
                    suggestionsSet = alarmSuggestions;
                else
                    suggestionsSet = baseSuggestions;
            }

            return suggestionsSet;
        }

        private void SetSuggestionsCornerRadius() {
            if (Suggestions.Count == 1)
                Suggestions[0].CornerRadius = new CornerRadius(6, 6, 6, 6);
            else if (Suggestions.Count > 1) {
                Suggestions[0].CornerRadius = new CornerRadius(6, 6, 0, 0);
                Suggestions[Suggestions.Count - 1].CornerRadius = new CornerRadius(0, 0, 6, 6);                
            }
        }

        #endregion
    }
}

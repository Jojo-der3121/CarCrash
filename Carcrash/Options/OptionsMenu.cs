using System;
using System.Collections.Generic;
using System.IO;
using Carcrash.Game;
using Carcrash.Options;
using Newtonsoft.Json;

namespace Carcrash
{
    class OptionsMenu
    {
        public Settings _settings = new Settings();
        public readonly string FilePath = @"C:\Users\jbb\source\repos\Carcrash\Carcrash\Options\CarCrashSettings.json";
        private readonly Menu _menu = new Menu();
        private readonly GameLoop _gameLoop = new GameLoop();




        public void Configurate(Settings settings)
        {
            _settings = settings;
            DrawOptionMenu();
            SelectWhatToConfigurate();

        }

        private void DrawOptionMenu()
        {
            Console.WriteLine(" ╔═─────────────────═╗");
            Console.WriteLine(" │┌─┐   │        ┌─┐ │");
            Console.WriteLine(" ││ │┌─┐┼ .┌─┐├─┐└─┐ │");
            Console.WriteLine(" │└─┘├─┘│ │└─┘│ │└─┘ │");
            Console.WriteLine(" │   │               │");
            Console.WriteLine(" ╚═─────────────────═╝");
            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine("   >>   Difficulty  <<");
            Console.WriteLine("");
            Console.WriteLine("      Player Amount");
            Console.WriteLine("");
            Console.WriteLine("         Sound");
            Console.WriteLine("");
            Console.WriteLine("         Color");
            Console.WriteLine("");
            Console.WriteLine("         Back");

        }

        private void SelectWhatToConfigurate()
        {
            var selection = _menu.SelectionProcess(9, 17, 3, 20);
            switch (selection)
            {
                case 9:
                    EditDifficulty();
                    break;
                case 11:
                    EditPlayerAmount();
                    break;
                case 13:
                    EditSound();
                    break;
                case 15:
                    EditColor();
                    break;
                case 17:
                    Console.Clear();
                    LeaderBoard.Serialize(_settings, FilePath);
                    _settings = Deserialize(FilePath);
                    _menu.Start(_settings);
                    break;
            }
        }

        private void DrawTable( string WhatIsEdited, string option1, string option2, string option3)
        {
            var frame = "";
            for (var i = 0; i < option3.Length; i++)
            {
                frame = String.Concat(frame, "─");
            }
            var tableList = new List<string>
            {
                "╚═────"+frame+"────═╝",
                "│                  │",
                "│                  │",
                "│                  │",
                "│     "+option3+"     │",
                "│                  │",
                "│     "+option2+"     │",
                "│                  │",
                "│   >>"+option1+"<<   │",
                "│                  │",
                "│Edit "+WhatIsEdited+"│",
                "╔═────"+frame+"────═╗"
            };
            _gameLoop.Draw(35, 19, tableList);
        }

        private void EditDifficulty()
        {
            DrawTable( "Difficulty:  ", "  Hard  ", " Medium ", "  Easy  ");
            var selection = _menu.SelectionProcess(11, 15, 39, 49);
            _settings.DifficultyLevel = selection switch
            {
                11 => 3,
                13 => 2,
                15 => 1,
                _ => _settings.DifficultyLevel
            };
            Console.Clear();
            Configurate(_settings);

        }

        private void EditPlayerAmount()
        {
            DrawTable( "Player Amount", "   1P   ", "   2p   ", "        ");
            var selection = _menu.SelectionProcess(11, 13, 39, 49);
            _settings.PlayMode = selection switch
            {
                11 => PlayMode.SinglePlayer,
                13 => PlayMode.MultiPlayer,
                _ => _settings.PlayMode
            };
            Console.Clear();
            Configurate(_settings);


        }
        private void EditSound()
        {
            DrawTable( "Sound:       ", "  Loud  ", "  Soft  ", "  Mute  ");
            var selection = _menu.SelectionProcess(11, 15, 39, 49);
            _settings.Sound = selection switch
            {
                11 => 2,
                13 => 1,
                15 => 0,
                _ => _settings.Sound
            };
            Console.Clear();
            Configurate(_settings);
        }

        private void EditColor()
        {
            DrawTable( "Color :      ",  " DarkRed", "DarkCyan", "  White ");
            var selection = _menu.SelectionProcess(11, 15, 39, 49);
            _settings.Color = selection switch
            {
                11 => ConsoleColor.DarkRed,
                13 => ConsoleColor.DarkCyan,
                15 => ConsoleColor.White,
                _ => _settings.Color
            };
            Console.Clear();
            Console.ForegroundColor = _settings.Color;
            Configurate(_settings);
        }

        public Settings Deserialize(string filePath)
        {
            var content = File.ReadAllText(filePath);
            return JsonConvert.DeserializeObject <Settings>(content);
        }
    }
}

﻿using System;
using System.Collections.Generic;
using System.IO;
using Carcrash.Game;
using Carcrash.Options;
using Newtonsoft.Json;

namespace Carcrash
{
    class OptionsMenu
    {
        private Settings _settings = new Settings();
        public readonly string FilePath;
        private GameLoop _gameLoop;

        public OptionsMenu()
        {
            FilePath = GetFilePath();
        }

        private string GetFilePath()
        {
            var directory = Directory.GetCurrentDirectory().Split("Carcrash");
            var path = "";
            for (var i = 0; i < directory.Length - 1; i++)
            {
                path += directory[i];
                path += "Carcrash";
            }
            return path += "\\Options\\CarCrashSettings.json";
        }


        public void Configurate(Settings settings)
        {
            _settings = settings;
            _gameLoop = new GameLoop(_settings);
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
            Console.WriteLine("   >>    GameMode   <<");
            Console.WriteLine("");
            Console.WriteLine("        Joining IP");
            Console.WriteLine("");
            Console.WriteLine("        Difficulty");
            Console.WriteLine("");
            Console.WriteLine("          Color");
            Console.WriteLine("");
            Console.WriteLine("          Sound");
            Console.WriteLine("");
            Console.WriteLine("          Songs");
            Console.WriteLine("");
            Console.WriteLine("          Back");

        }

        private void SelectWhatToConfigurate()
        {
            var menu = new Menu();
            var selection = menu.SelectionProcess(9, 21, 3, 20);
            switch (selection)
            {
                case 9:
                    EditPlayerAmount(menu);
                    break;
                case 11:
                    EditIpToConnectTo(menu);
                    break;
                case 13:
                    EditDifficulty(menu);
                    break;
                case 15:
                    EditColor(menu);
                    break;
                case 17:
                    EditSound(menu);
                    break;
                case 19:
                    EditSongSelection(menu);
                    break;
                case 21:
                    Console.Clear();
                    LeaderBoard.Serialize(_settings, FilePath);
                    _settings = Deserialize(FilePath);
                    menu.Start(_settings);
                    break;
            }
        }

        private void DrawTable(string WhatIsEdited, string option1, string option2, string option3)
        {
            var frame = "";
            var whiteSpace = "";
            for (var i = 0; i < option3.Length; i++)
            {
                frame = String.Concat(frame, "─");
                whiteSpace = String.Concat(whiteSpace, " ");
            }

            var titleWhiteSpace = whiteSpace.Remove(whiteSpace.Length - WhatIsEdited.Length + 5);
            var tableList = new List<string>
            {
                "╚═────"+frame+"────═╝",
                "│     "+whiteSpace+"     │",
                "│     "+whiteSpace+"     │",
                "│     "+whiteSpace+"     │",
                "│     "+option3+"     │",
                "│     "+whiteSpace+"     │",
                "│     "+option2+"     │",
                "│     "+whiteSpace+"     │",
                "│   >>"+option1+"<<   │",
                "│     "+whiteSpace+"     │",
                "│Edit "+WhatIsEdited+titleWhiteSpace+"│",
                "╔═────"+frame+"────═╗"
            };
            _gameLoop.Draw(35, 19, tableList);
        }

        private void EditIpToConnectTo(Menu menu)
        {
            Console.SetCursorPosition(40,10);
            Console.WriteLine("Please Enter the Ip-Address! :D");
            Console.SetCursorPosition(39,11);
            Console.WriteLine("═══════════════════════════════════");
            Console.SetCursorPosition(45,14);
            _settings.Ip = Console.ReadLine();
            Console.Clear();
            Configurate(_settings);
        }

        private void EditDifficulty(Menu menu)
        {
            DrawTable("Difficulty:  ", "  Hard  ", " Medium ", "  Easy  ");
            var selection = menu.SelectionProcess(11, 15, 39, 49);
            _settings.DifficultyLevel = selection switch
            {
                11 => 0,
                13 => 1,
                15 => 5,
                _ => _settings.DifficultyLevel
            };
            Console.Clear();
            Configurate(_settings);

        }

        private void EditPlayerAmount(Menu menu)
        {
            DrawTable("Player Amount", "1P(local) ", "2P(online)", "          ");
            var selection = menu.SelectionProcess(11, 13, 39, 51);
            _settings.PlayMode = selection switch
            {
                11 => PlayMode.SinglePlayer,
                13 => PlayMode.MultiPlayer,
                _ => _settings.PlayMode
            };
            Console.Clear();
            Configurate(_settings);
        }

        private void EditSound(Menu menu)
        {
            DrawTable("Sound:       ", "  High  ", "  Deep  ", "  Mute  ");
            var selection = menu.SelectionProcess(11, 15, 39, 49);
            _settings.Sound = selection switch
            {
                11 => 750,
                13 => 350,
                15 => 0,
                _ => _settings.Sound
            };
            Console.Clear();
            Configurate(_settings);
        }

        private void EditSongSelection(Menu menu)
        {
            DrawTable("Song:        ", "  Alle meine Entchen   ", "Super Mario Bros. Theme", "        Tetris         ");
            var selection = menu.SelectionProcess(11, 15, 39, 39 + 25);
            _settings.WhichSong = selection switch
            {
                11 => 1,
                13 => 2,
                15 => 3,
                _ => _settings.WhichSong
            };
            Console.Clear();
            Configurate(_settings);
        }

        private void EditColor(Menu menu)
        {
            DrawTable("Color :      ", " DarkRed", "DarkCyan", "  White ");
            var selection = menu.SelectionProcess(11, 15, 39, 49);
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
            return JsonConvert.DeserializeObject<Settings>(content);
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Media;
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
            var directory = Directory.GetCurrentDirectory();
            return directory += "\\CarCrashSettings.json";
        }


        public void Configurate(Settings settings)
        {
            _settings = settings;
            _gameLoop = new GameLoop(_settings);
            DrawOptionMenu();
            SelectWhatToConfigure();
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
            Console.WriteLine("          Songs");
            Console.WriteLine("");
            Console.WriteLine("          Sound");
            Console.WriteLine("");
            Console.WriteLine("          Back");

        }

        private void SelectWhatToConfigure()
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
                    EditSongSelection(menu);
                    break;
                case 19:
                    EditSound(menu);
                    break;
                case 21:
                    Console.Clear();
                    LeaderBoard.Serialize(_settings, FilePath);
                    _settings = Deserialize(FilePath);
                    menu.Start(_settings);
                    break;
            }
        }

        private void DrawTable(string whatIsEdited, string option1, string option2, string option3)
        {
            var frame = "";
            var whiteSpace = "";
            for (var i = 0; i < option3.Length; i++)
            {
                frame = String.Concat(frame, "─");
                whiteSpace = String.Concat(whiteSpace, " ");
            }

            var titleWhiteSpace = whiteSpace.Remove(whiteSpace.Length - whatIsEdited.Length + 5);
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
                "│Edit "+whatIsEdited+titleWhiteSpace+"│",
                "╔═────"+frame+"────═╗"
            };
            _gameLoop.Draw(35, 19, tableList);
        }

        private void EditIpToConnectTo(Menu menu)
        {
            Console.SetCursorPosition(40, 10);
            Console.WriteLine("Please Enter the Ip-Address! :D");
            Console.SetCursorPosition(39, 11);
            Console.WriteLine("═══════════════════════════════════");
            Console.SetCursorPosition(45, 14);
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
                15 => 7,
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
            if (_settings.Sound != 0)
            {
                menu.Player.SoundLocation = _settings.WhichSong;
                menu.Player.PlayLooping();
            }
            else
            {
                menu.Player.Stop();
            }
            Console.Clear();
            Configurate(_settings);
        }

        private void EditSongSelection(Menu menu)
        {
            DrawTable("Song:        ", "                        Tetris                         ", "                      Africa-toto                      ", "Just try it come on whats the worst that can happen? =)");
            var selection = menu.SelectionProcess(11, 15, 39, 39 + 57);
            _settings.WhichSong = selection switch
            {
                11 => "Tetris.wav",
                13 => "africa-toto.wav",
                15 => "Rick_Astley_-_Never_Gonna_Give_You_Up_Qoret.com (online-audio-converter.com).wav",
                _ => _settings.WhichSong
            };
            menu.Player.SoundLocation = _settings.WhichSong;
            menu.Player.PlayLooping();
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

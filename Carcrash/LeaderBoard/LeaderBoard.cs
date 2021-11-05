using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using System.Linq;
using Carcrash.Options;

namespace Carcrash
{
    class LeaderBoard
    {
        private string FilePath;
        private List<LeaderBoardEntry> _leaderBoardEntries = new List<LeaderBoardEntry>();
        private readonly List<string> _tableDesign = new List<string>();
        private Settings _settings = new Settings();

        private string GetFilePath()
        {
            var whichDifficulty = "";
            var directory = Directory.GetCurrentDirectory().Split("Carcrash");
            var path = "";
            switch (_settings.DifficultyLevel)
            {
                case 0:
                    whichDifficulty = "Hard";
                    break;
                case 1:
                    whichDifficulty = "Medium";
                    break;
                case 7:
                    whichDifficulty = "Easy";
                    break;
            }
            for (var i = 0; i < directory.Length - 1; i++)
            {
                path += directory[i];
                path += "Carcrash";
            }
            return path += "\\LeaderBoard\\CarCrashLeaderBoard" + whichDifficulty + ".json";
        }

        public void CreateLeaderBoard(double score, Settings settings)
        {
            _settings = settings;
            var loop = new GameLoop(_settings);
            var menu = new Menu();

            DrawHeadLine();
            FilePath = GetFilePath();
            CreateTable();
            loop.Draw(45, 20, _tableDesign);
            _leaderBoardEntries.AddRange(Deserialize(FilePath));
            _leaderBoardEntries = SortLeaderBoardEntryList(_leaderBoardEntries);
            FillTable(_leaderBoardEntries);
            _leaderBoardEntries.Add(NewLeaderBoardEntryInput(score));
            _leaderBoardEntries = SortLeaderBoardEntryList(_leaderBoardEntries);
            loop.Draw(45, 20, _tableDesign);
            FillTable(_leaderBoardEntries);
            Serialize(_leaderBoardEntries, FilePath);
            menu.PressEnterToContinue("main menu", 23, 47);
            menu.Start(_settings);
        }

        private LeaderBoardEntry NewLeaderBoardEntryInput(double score)
        {
            var leaderBoardEntry = new LeaderBoardEntry();

            Console.SetCursorPosition(6, 6);
            Console.WriteLine("Your Score:" + score);
            Console.SetCursorPosition(6, 7);
            Console.WriteLine("please Enter Your Name! UwU");
            Console.SetCursorPosition(6, 9);
            leaderBoardEntry.Name = Console.ReadLine();
            leaderBoardEntry.Score = score;
            return leaderBoardEntry;
        }
        private void DrawHeadLine()
        {
            var headLine = new List<string>
            {
                "════════════════└─┘════════════│═════════",
                "└──└──└─┴└─┘││ │└─┤  │  └─└─┴└─┤└── │ └─┘",
                "│  ┌─┐┌─┐┌─┤.├─┐┌─┐  ├─┘│ ┌─┐   ┌─┐ ┌─└─┐",
                "│          │         ┌─┐│             ┌─┐"
            };
            var loop = new GameLoop(_settings);
            loop.Draw(2, 4, headLine);
        }

        private void CreateTable()
        {
            _tableDesign.Add("╚═════════════════════════════════════╩══════════════════════════════════╝");
            _tableDesign.Add("║                                     ║                                  ║");
            _tableDesign.Add("╠═════════════════════════════════════╬══════════════════════════════════╣");
            _tableDesign.Add("║                                     ║                                  ║");
            _tableDesign.Add("╠═════════════════════════════════════╬══════════════════════════════════╣");
            _tableDesign.Add("║                                     ║                                  ║");
            _tableDesign.Add("╠═════════════════════════════════════╬══════════════════════════════════╣");
            _tableDesign.Add("║                                     ║                                  ║");
            _tableDesign.Add("╠═════════════════════════════════════╬══════════════════════════════════╣");
            _tableDesign.Add("║                                     ║                                  ║");
            _tableDesign.Add("╠═════════════════════════════════════╬══════════════════════════════════╣");
            _tableDesign.Add("║                                     ║                                  ║");
            _tableDesign.Add("╠═════════════════════════════════════╬══════════════════════════════════╣");
            _tableDesign.Add("║                                     ║                                  ║");
            _tableDesign.Add("╠═════════════════════════════════════╬══════════════════════════════════╣");
            _tableDesign.Add("║                                     ║                                  ║");
            _tableDesign.Add("╠═════════════════════════════════════╬══════════════════════════════════╣");
            _tableDesign.Add("║                                     ║                                  ║");
            _tableDesign.Add("╠═════════════════════════════════════╬══════════════════════════════════╣");
            _tableDesign.Add("║ Name:                               ║ Score:                           ║");
            _tableDesign.Add("╔═════════════════════════════════════╦══════════════════════════════════╗");
        }

        private void FillTable(List<LeaderBoardEntry> leaderBoardEntryList)
        {
            var y = 3;
            var howManyEntriesWillBeDrawn = 9;
            if (leaderBoardEntryList.Count < 9)
            {
                howManyEntriesWillBeDrawn = leaderBoardEntryList.Count;
            }
            for (var i = 0; i < howManyEntriesWillBeDrawn; i++)
            {
                var leaderBoardEntry = leaderBoardEntryList[i];
                Console.SetCursorPosition(46, y);
                var name = leaderBoardEntry.Name;
                if (name.Length > 36)
                {
                    name = name.Truncate(33);
                }
                Console.WriteLine(name);
                Console.SetCursorPosition(46 + 38, y);
                Console.WriteLine(leaderBoardEntry.Score);
                y += 2;
            }
        }

        private List<LeaderBoardEntry> SortLeaderBoardEntryList(List<LeaderBoardEntry> leaderBoardEntryList)
        {
            var sortedList = leaderBoardEntryList.OrderBy(o => o.Score).ToList();
            sortedList.Reverse();
            return sortedList;
        }

        public static void Serialize(object obj, string filePath)
        {
            var jsonString = JsonConvert.SerializeObject(obj);
            File.WriteAllText(filePath, jsonString);
        }

        private static IEnumerable<LeaderBoardEntry> Deserialize(string filePath)
        {
            var content = File.ReadAllText(filePath);
            return JsonConvert.DeserializeObject<IEnumerable<LeaderBoardEntry>>(content);
        }

    }

}

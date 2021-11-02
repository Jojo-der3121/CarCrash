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
        private Settings _settings;

        public LeaderBoard()
        {
            FilePath = GetFilePath();
        }

        private string GetFilePath()
        {
            var directory = Directory.GetCurrentDirectory().Split("Carcrash");
            return directory[0] += "Carcrash\\Carcrash\\LeaderBoard\\CarCrashLeaderBoard.json";
        }

        public void CreateLeaderBoard(double score,Settings settings)
        {
            _settings = settings;
            CreateTable();
            _leaderBoardEntries.AddRange(Deserialize(FilePath));
            _leaderBoardEntries= SortLeaderBoardEntryList(_leaderBoardEntries);
            FillTable(_leaderBoardEntries);
            var leaderBoardEntry = new LeaderBoardEntry();
            Console.SetCursorPosition(2, 3);
            Console.WriteLine("Your Score:"+score);
            Console.SetCursorPosition(2, 4 );
            Console.WriteLine("please Enter Your Name! UwU");
            Console.SetCursorPosition(2, 5 );
            leaderBoardEntry.Name = Console.ReadLine();
            leaderBoardEntry.Score = score;
            _leaderBoardEntries.Add(leaderBoardEntry);
            _leaderBoardEntries = SortLeaderBoardEntryList(_leaderBoardEntries);
            CreateTable();
            FillTable(_leaderBoardEntries);
            Serialize(_leaderBoardEntries,FilePath);
            var menu = new Menu();
            menu.PressEnterToContinue("main menu");
            menu.Start(_settings);
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

            var gameLoop = new GameLoop(_settings);
            gameLoop.Draw(35, 20, _tableDesign);
        }

        private void FillTable(List<LeaderBoardEntry> leaderBoardEntryList)
        {
            var y = 3;
            var howManyEntriesWillBeDrawn = 9;
            if (leaderBoardEntryList.Count < 9)
            {
                howManyEntriesWillBeDrawn = leaderBoardEntryList.Count;
            }
            for (var i= 0 ; i< howManyEntriesWillBeDrawn ;i++)
            {
                var leaderBoardEntry = leaderBoardEntryList[i];
                Console.SetCursorPosition(36, y);
                var name = leaderBoardEntry.Name;
                if (name.Length > 36)
                {
                    name = name.Truncate(33);
                }
                Console.WriteLine(name);
                Console.SetCursorPosition(36 + 38, y);
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

        public static void Serialize(object obj,string filePath)
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

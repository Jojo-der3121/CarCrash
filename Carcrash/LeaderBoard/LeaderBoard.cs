using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using Newtonsoft.Json;
using System.Linq;
using Carcrash.Options;

namespace Carcrash
{
    class LeaderBoard
    {
        private const string FilePath = @"C:\Users\jbb\source\repos\Carcrash\Carcrash\LeaderBoard\CarCrashLeaderBoard.json";
        private List<LeaderBoardEntry> _leaderBoardEntries = new List<LeaderBoardEntry>();
        private readonly List<string> _tableDesign = new List<string>();

        public void CreateLeaderBoard(List<double> scoreList,Settings settings)
        {
            CreateTable();
            _leaderBoardEntries.AddRange(Deserialize(FilePath));
            _leaderBoardEntries= SortLeaderBoardEntryList(_leaderBoardEntries);
            FillTable(_leaderBoardEntries);
            var leaderBoardEntry = new LeaderBoardEntry();
            for (int i = 1; i <= scoreList.Count; i++)
            {
                var scoreListIndex = i - 1;
                Console.SetCursorPosition(2, 3+scoreListIndex*3);
            Console.WriteLine("Score of Car"+i+":"+scoreList[scoreListIndex]);
            Console.SetCursorPosition(2, 4 +scoreListIndex*3);
            Console.WriteLine("please Enter Your Name! UwU");
            Console.SetCursorPosition(2, 5);
            leaderBoardEntry.Name = Console.ReadLine();
            leaderBoardEntry.Score = scoreList[scoreListIndex];
               _leaderBoardEntries.Add(leaderBoardEntry);
            }
            _leaderBoardEntries = SortLeaderBoardEntryList(_leaderBoardEntries);
            CreateTable();
            FillTable(_leaderBoardEntries);
            Serialize(_leaderBoardEntries,FilePath);
            var menu = new Menu();
            menu.PressEnterToContinue("main menu");
            menu.Start(settings);
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

            var gameLoop = new GameLoop();
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

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using Carcrash.Game;
using Carcrash.Options;

namespace Carcrash
{
    class GameLoop
    {

        private readonly List<string> _groundList;
        private readonly Road _road = new Road();
        private readonly Car _car1;
        private readonly Cars _enemyCar1 = new Cars(74);
        private readonly Settings _settings;
        private readonly List<string> _efficientDrawFrameList = new List<string>();
        private const int ScoreDivider = 25;
        private const int RightRoadLeftBoarder = 45;
        private const int RightRoadRightBoarder = 45 + 19;
        private const int ExtraScore = 9;
        private const int NoDeviation = 0;
        private const int RightRoadMiddleOfLeftLane = 53;
        private const int RightRoadMiddleOfRightLane = 72;


        public GameLoop(Settings settings)
        {
            _settings = settings;
            Console.CursorVisible = false;
            _car1 = new Car(74, 0, _settings.DifficultyLevel);
            _groundList = FillGroundList();
        }

        public void Tutorial()
        {

            Console.SetCursorPosition(35, 5);
            Console.WriteLine("Please use :");
            var tutorial = new List<string>
            {
                "╚═════╩═════╩═════╝",
                "║ ║ ║ │ ╙─╜ │ ╙─┘ ║",
                "║ ╟─╢ │ ╙─╖ │ ║ │ ║",
                "║ ╔═╗ │ ╓─╖ │ ╓─┐ ║",
                "╔════╩╦─────╬═════╗",
                "     ║ ║/\\║ ║",
                "     ║ ║  ║ ║",
                "     ║      ║",
                "     ╔══════╗"
            };
            Draw(45, 18, tutorial);
            Console.SetCursorPosition(55, 21);
            Console.WriteLine("to move.");
            Thread.Sleep(3000);
            Console.Clear();
            SinglePlayerLoop();
        }

        private void SinglePlayerLoop()
        {
            while (true)
            {
                EfficientDraw();
                GiveScore();
                if (CalculateCollision(_car1.ObjectSizeAndLocation, _enemyCar1.ObjectSizeAndLocation))
                {
                    break;
                }
                if (_car1.Score > ScoreDivider * _settings.DifficultyLevel)
                {
                    _enemyCar1.Movement(NoDeviation);
                }
                _road.Movement();
                _car1.Steer();
                Thread.Sleep(13);
            }
            if (_settings.Sound != 0)
            {
                Console.Beep(_settings.Sound, 1350);
            }
            Die(_car1.Score / ScoreDivider);
        }

        private void GiveScore()
        {
            if (_car1.ObjectSizeAndLocation.Left > RightRoadLeftBoarder - 5 && _car1.ObjectSizeAndLocation.Left < RightRoadLeftBoarder + 35)
            {
                _car1.Score++;
            }
            if (_car1.ObjectSizeAndLocation.Left < RightRoadRightBoarder - 2 && _car1.ObjectSizeAndLocation.Left > RightRoadLeftBoarder - 5)
            {
                _car1.Score += ExtraScore;
            }
        }

        private List<string> FillGroundList()
        {
            var groundListModel = new List<string>();
            for (var i = 0; i < 32; i++)
            {
                groundListModel.Add("                                            ║                 ║║                 ║                                       ");
            }
            groundListModel[26] = groundListModel[26].Insert(95, "Score:").Remove(120);
            return groundListModel;
        }

        public bool CalculateCollision(ObjectSizeAndLocation objectSizeAndLocationA, ObjectSizeAndLocation objectSizeAndLocationB)
        {
            var locationsObjectA = CreateLocationList(objectSizeAndLocationA.CollisionDimensions, objectSizeAndLocationA.Top, objectSizeAndLocationA.Left);
            var locationsObjectB = CreateLocationList(objectSizeAndLocationB.CollisionDimensions, objectSizeAndLocationB.Top, objectSizeAndLocationB.Left);

            foreach (var locationA in locationsObjectA)
            {
                foreach (var locationB in locationsObjectB)
                {
                    if (locationA == locationB)
                    {
                        return true;
                    }
                }
            }

            return false;

        }

        private List<string> CreateLocationList(List<int> dimensionList, int y, int x)
        {
            var locationList = new List<string>();
            var cacheList = new List<string>();
            for (var i = 0; i < dimensionList[0]; i++)
            {
                cacheList.Add(Convert.ToString(y + i));
                for (var e = 0; e < dimensionList[1]; e++)
                {
                    locationList.Add(cacheList[i] + ":" + Convert.ToString(x + e));
                }

            }
            return locationList;
        }

        public void Draw(int x, int y, List<string> whatToDraw)
        {
            foreach (var element in whatToDraw)
            {
                if (y >= 0 && y <= 28)
                {
                    Console.SetCursorPosition(x, y);
                    Console.Write(element);
                }
                y -= 1;
            }

        }

        private void EfficientDraw()
        {
            _efficientDrawFrameList.Clear();
            _efficientDrawFrameList.AddRange(_groundList);
            AddToFrame(_road.Top, RightRoadMiddleOfLeftLane, _road.Design);
            AddToFrame(_road.Top, RightRoadMiddleOfRightLane, _road.Design);
            AddToFrame(_car1.ObjectSizeAndLocation.Top, _car1.ObjectSizeAndLocation.Left, _car1.Design);
            if (_car1.Score > ScoreDivider * _settings.DifficultyLevel)
            {
                AddToFrame(_enemyCar1.ObjectSizeAndLocation.Top, _enemyCar1.ObjectSizeAndLocation.Left, _enemyCar1.Design);
            }
            AddToFrame(27, 95, GetScoreDisplayList());
            _efficientDrawFrameList.Reverse();
            Draw(0, _efficientDrawFrameList.Count - 1, _efficientDrawFrameList);
        }

        private List<string> GetScoreDisplayList()
        {
            var scoreDisplayList = new List<string>
            {
                Convert.ToString(_car1.Score / ScoreDivider, CultureInfo.InvariantCulture)
            };
            return scoreDisplayList;
        }

        private void AddToFrame(int top, int left, List<string> whatIsAdded)
        {
            for (var i = 0; i < whatIsAdded.Count; i++)
            {
                if (top - i <= _groundList.Count - 1 && top - i >= 0)
                {
                    var cacheString = _efficientDrawFrameList[top - i];
                    _efficientDrawFrameList[top - i] = _efficientDrawFrameList[top - i].Insert(left, whatIsAdded[i]);
                    _efficientDrawFrameList[top - i] = _efficientDrawFrameList[top - i].Substring(0, left + whatIsAdded[i].Length);
                    cacheString = cacheString.Substring(left + whatIsAdded[i].Length);
                    _efficientDrawFrameList[top - i] += cacheString;
                }
            }
        }

        public void Die(double score)
        {
            Console.Clear();
            var deathMessage = new List<string>()
            {
                "╚═════════════════════════════════════════════════╝",
                "║   sincerly ~Jojo                                ║",
                "║                                                 ║",
                "║ in the meantime =)                              ║",
                "║ You will be redirected to the Leader Board      ║",
                "║                                                 ║",
                "║ We will hold a minute of silence in your honor. ║",
                "║ You are surely gonna be missed..                ║",
                "║                                                 ║",
                "║  You have died,                                 ║",
                "║                                                 ║",
                "╔═════════════════════════════════════════════════╗"
            };
            Draw(35, 20, deathMessage);
            Thread.Sleep(1500);
            var menu = new Menu();
            menu.PressEnterToContinue("leader boards", 23, 40);
            var leaderBoard = new LeaderBoard();
            Console.Clear();
            leaderBoard.CreateLeaderBoard(score, _settings);
        }
    }
}

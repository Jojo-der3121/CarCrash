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
                    PlayExplosionAnimation(_car1.ObjectSizeAndLocation.Top, _car1.ObjectSizeAndLocation.Left, FillAnimationList(),_settings.Sound);
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
            Console.Beep(1750,1000);
            menu.PressEnterToContinue("leader boards", 23, 40);
            var leaderBoard = new LeaderBoard();
            Console.Clear();
            leaderBoard.CreateLeaderBoard(score, _settings);
        }

        private List<List<string>> FillAnimationList()
        {
            var frame1 = new List<string>
            {
                "\\ | /",
                "-(@)-",
                "/-|-\\"
            };
            frame1.Reverse();
            var frame2 = new List<string>
            {
                " /-\\",
                "(|&|)",
                "\\---/"
            };
            frame2.Reverse();
            var frame3 = new List<string>
            {
                "|-^-|",
                "\\(φ)/",
                " |´|",
                "/-=-\\"
            };
            frame3.Reverse();
            var frame4 = new List<string>
            {
                " _^|^_",
                "/(=^=)\\",
                "<:═|-;>",
                "  |;|",
                ".-═=═-.",
                " /| ,\\",
                "- - - -"
            };
            frame4.Reverse();
            var frame5 = new List<string>
            {
                "   -~^~-",
                " ( ^   ^ )",
                "/≡ -   - ≡\\",
                "<- .° ° ,->",
                " -=═_ _═=-",
                "   \\   /",
                "    | |",
                ".-═=≡ ≡=═-.",
                "   -\\ /-",
                "   /| ;\\",
                "-_-_-_-_-_-"
            };
            frame5.Reverse();
            var frame6 = new List<string>
            {
                "    _~~~`~_-\\, _",
                "   /  (     )- \\",
                " ((             |",
                " |  (  °   ))    )",
                "\\.  ((        ), /",
                "  ;\\\\~. - _ - / )`",
                "      \\|| | /",
                "       | ^ |",
                " ,-═=≡&≡ φ ≡&≡=═-\\",
                " ` - =≡| ≡ |═ -  ´",
                "       /   \\",
                "  - _-~ ~ ~_~_°",
                " _°-_-_-_-_-_-;_-."

            };
            frame6.Reverse();
            var frame7 = new List<string>
            {
                "   _~*;-^_",
                "/ /^    |°\\ \\",
                " ((    c) ))",
                "  - -|- - -",
                " (( \\  |/ ))",
                "     | /",
                "     | | | ",
                " .-═=≡ ≡=═-.",
                "    -\\ /-",
                "     | |",
                "    /| ;\\",
                " -_-_-_-_-_-"
            };
            frame7.Reverse();
            var frame8 = new List<string>
            {
                "  /^~*;-_°\\",
                "( (       ) )",
                "  - -|- -/-",
                "    /  |\\",
                "    (  ;) ",
                "     |",
                "      \\",
                "     /φ",
                "    | |",
                "    |||\\",
                " .- ═ ═ -.",
                "   /´ ,\\",
                "  - - - -"
            };
            frame8.Reverse();
            var frame9 = new List<string>
            {
                "  _~ _-",
                "  /^ ~ )\\",
                "   (  ;)",
                "   \\|",
                "     \\",
                "     /",
                "   `",
                "",
                "   ´",
                "  /",
                "  φ\\",
                "  | |",
                " /-=-\\ "
            };
            frame9.Reverse();
            var frame10 = new List<string>
            {
                "  `",
                "  /",
                "  ,|",
                ".| ,\\"

            };
            frame10.Reverse();
            var frame11 = new List<string>
            {
                "  .",
                "  /",
                "_|\\"
            };
            frame11.Reverse();
            var frame12 = new List<string>
            {
                "_;"
            };
            var frame13 = new List<string>
            {
                "\\"
            };
            var animation = new List<List<string>>
            {
                frame1,
                frame2,
                frame3,
                frame4,
                frame5,
                frame6,
                frame7,
                frame8,
                frame9,
                frame10,
                frame11,
                frame12,
                frame13
            };
            return animation;

        }

        public void PlayExplosionAnimation(int top, int left, List<List<string>> frameList, int sound)
        {
            if (sound != 0)
            {
                var menu = new Menu();
                menu.Player.SoundLocation = "mixkit-arcade-game-explosion-echo-1698-[AudioTrimmer (Joined by Happy Scribe) (1) (online-audio-converter.com).wav";
                menu.Player.Play();
            }
            var timingList = new List<int>
            {
                1450,
                150,
                200,
                275,
                300,
                1000,
                500,
                100,
                500,
                600,
                400,
                100,
                200
            };
            Thread.Sleep(175);
            for (var i = 0; i < frameList.Count; i++)
            {
                var frame = frameList[i];
                Draw(left + 3 - frame[frame.Count - 1].Length / 2, top - 4, frame);
                Thread.Sleep(timingList[i]);
                Console.Clear();
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using Carcrash.Options;

namespace Carcrash.Game.OnlineGame
{
    class Client
    {
        private readonly List<string> _groundList;
        private List<List<string>> animationFrameList = new List<List<string>>();
        private bool _enemyDiedLast = false;
        private int DeathLeft = 0;
        private readonly Road _road = new Road();
        private readonly Car _car1;
        private readonly Car _hostCar = new Car(31, 1, 1);
        private readonly Cars _enemyCar1 = new Cars(74);
        private readonly Cars _enemyCar2 = new Cars(74 - 43);
        private const int ScoreDivider = 25;
        private const int RightRoadLeftBoarder = 45;
        private const int RightRoadRightBoarder = 45 + 19;
        private const int LeftRoadLeftBoarder = 2;
        private const int LeftRoadRightBoarder = 2 + 19;
        private const int ExtraScore = 9;
        private const int LeftRoadMiddleOfLeftLane = 10;
        private const int LeftRoadMiddleOfRightLane = 29;
        private const int RightRoadMiddleOfLeftLane = 53;
        private const int RightRoadMiddleOfRightLane = 72;
        private readonly Settings _settings;
        private TcpClient _client;
        private Stream _stream;
        private StreamReader _streamR;
        private StreamWriter _streamW;
        private readonly GameLoop _loop;
        private readonly List<string> _efficientDrawFrameList = new List<string>();

        public Client(Settings settings)
        {
            _settings = settings;
            _groundList = FillGroundList();
            animationFrameList = FillAnimationList();
            _loop = new GameLoop(_settings);
            _car1 = new Car(74, 2, _settings.DifficultyLevel);
        }

        public void ConnectToServer()
        {
            _client = new TcpClient(_settings.Ip, 307);
            _stream = _client.GetStream();
            _streamW = new StreamWriter(_stream);
            _streamW.AutoFlush = true;
            _streamR = new StreamReader(_stream);
            var thread = new Thread(GetServerAnswer);
            thread.Start();
            PlayGame();
        }

        private void PlayGame()
        {
            try
            {
                var hostCarDeathDesignIndex = 0;
                var car1DeathDesignIndex = 0;
                while (true)
                {
                    EfficientDraw();
                    Thread.Sleep(10);
                    if (!_car1.Dead)
                    {
                        CheckIfDeadAndGiveScore();
                        if (_car1.Dead && _hostCar.Dead)
                        {
                            _streamW.WriteLine("Play Explosion");
                            _loop.PlayExplosionAnimation(_car1.ObjectSizeAndLocation.Top, _car1.ObjectSizeAndLocation.Left, animationFrameList,_settings.Sound);
                        }
                        _car1.Steer();
                    }
                    else if(car1DeathDesignIndex<74)
                    {
                        Thread.Sleep(10);
                        if (car1DeathDesignIndex == 0)
                        {
                            DeathLeft = _car1.ObjectSizeAndLocation.Left;
                        }
                        _car1.Design = GiveRightAnimationFrame(car1DeathDesignIndex);
                        _car1.ObjectSizeAndLocation.Left = DeathLeft - _car1.Design[_car1.Design.Count - 1].Length / 2;
                        car1DeathDesignIndex++;
                    }
                    else
                    {
                        Thread.Sleep(10);
                    }
                    if (_enemyDiedLast)
                    {
                        _loop.PlayExplosionAnimation(_hostCar.ObjectSizeAndLocation.Top, _hostCar.ObjectSizeAndLocation.Left, animationFrameList, _settings.Sound);
                    }
                    if (_hostCar.Dead && hostCarDeathDesignIndex < 74)
                    {
                        if (hostCarDeathDesignIndex == 0)
                        {
                            DeathLeft = _hostCar.ObjectSizeAndLocation.Left;
                        }
                        _hostCar.Design = GiveRightAnimationFrame(hostCarDeathDesignIndex);
                        _hostCar.ObjectSizeAndLocation.Left = DeathLeft - _hostCar.Design[_hostCar.Design.Count - 1].Length / 2;
                        hostCarDeathDesignIndex++;
                    }
                    _streamW.WriteLine("clientCar Left:" + _car1.ObjectSizeAndLocation.Left);
                    _streamW.WriteLine("clientCar Top:" + _car1.ObjectSizeAndLocation.Top);
                    _road.Movement();
                    _streamW.WriteLine("clientCar deadStatus:" + _car1.Dead);
                    _streamW.WriteLine("clientCar Score:" + _car1.Score);
                    if (_car1.Dead && _hostCar.Dead)
                    {
                        break;
                    }
                }
                if (_settings.Sound != 0)
                {
                    Console.Beep(_settings.Sound, 1350);
                }
                _loop.Die(_car1.Score / ScoreDivider);
            }
            catch (Exception)
            {
                Console.Clear();
                Console.SetCursorPosition(45, 15);
                Console.WriteLine("The Connection has been Lost...=/");
                Thread.Sleep(1500);
                Console.Clear();
                var menu = new Menu();
                menu.Start(_settings);
            }

        }

        private void GetServerAnswer()
        {
            try
            {
                while (true)
                {
                    var serverInformation = _streamR.ReadLine();
                    var whichServerInformation = GetInformationValue(serverInformation);
                    switch (whichServerInformation[0])
                    {
                        case "hostCar Left":
                            _hostCar.ObjectSizeAndLocation.Left = Convert.ToInt32(whichServerInformation[1]);
                            break;
                        case "hostCar Top":
                            _hostCar.ObjectSizeAndLocation.Top = Convert.ToInt32(whichServerInformation[1]);
                            break;
                        case "hostCar deadStatus":
                            _hostCar.Dead = Convert.ToBoolean(whichServerInformation[1]);
                            break;
                        case "hostCar Score":
                            _hostCar.Score = Convert.ToDouble(whichServerInformation[1]);
                            break;
                        case "enemyCar1Left":
                            _enemyCar1.ObjectSizeAndLocation.Left = Convert.ToInt32(whichServerInformation[1]);
                            break;
                        case "enemyCar1Top":
                            _enemyCar1.ObjectSizeAndLocation.Top = Convert.ToInt32(whichServerInformation[1]);
                            break;
                        case "enemyCar2Left":
                            _enemyCar2.ObjectSizeAndLocation.Left = Convert.ToInt32(whichServerInformation[1]);
                            break;
                        case "enemyCar2Top":
                            _enemyCar2.ObjectSizeAndLocation.Top = Convert.ToInt32(whichServerInformation[1]);
                            break;
                        case "Play Explosion":
                            _enemyDiedLast = true;
                            break;
                    }
                }
            }
            catch (Exception)
            {
                // ignored
            }
        }

        private string[] GetInformationValue(string serverInformation)
        {
            var informationArray = serverInformation.Split(":");
            return informationArray;
        }

        private bool CheckForCollisions(ObjectSizeAndLocation objectSizeAndLocationA)
        {
            var allEnemyCollisionBoarders = new List<ObjectSizeAndLocation>
            {
                _enemyCar1.ObjectSizeAndLocation,
                _enemyCar2.ObjectSizeAndLocation
            };
            foreach (var collisionBoarderB in allEnemyCollisionBoarders)
            {
                if (_loop.CalculateCollision(objectSizeAndLocationA, collisionBoarderB))
                {
                    return true;
                }
            }
            return false;
        }

        private List<string> FillGroundList()
        {
            var groundListModel = new List<string>();
            for (var i = 0; i < 32; i++)
            {
                groundListModel.Add(" ║                 ║║                 ║     ║                 ║║                 ║                                       ");
            }
            groundListModel[26] = groundListModel[26].Insert(95, "Car2Score:").Remove(120);
            groundListModel[23] = groundListModel[23].Insert(95, "Car1Score:").Remove(120);
            return groundListModel;
        }

        private void CheckIfDeadAndGiveScore()
        {
            if (_car1.ObjectSizeAndLocation.Left < LeftRoadRightBoarder - 2 && _car1.ObjectSizeAndLocation.Left > LeftRoadLeftBoarder - 5 && !_car1.Dead || _car1.ObjectSizeAndLocation.Left < RightRoadRightBoarder - 2 && _car1.ObjectSizeAndLocation.Left > RightRoadLeftBoarder - 5 && !_car1.Dead)
            {
                _car1.Score += ExtraScore;
            }
            if (CheckForCollisions(_car1.ObjectSizeAndLocation))
            {
                _car1.Dead = true;
            }
            if (!_car1.Dead && _car1.ObjectSizeAndLocation.Left > LeftRoadLeftBoarder - 5 && _car1.ObjectSizeAndLocation.Left < RightRoadLeftBoarder + 35)
            {
                _car1.Score++;
            }
        }

        private void EfficientDraw()
        {
            _efficientDrawFrameList.Clear();
            _efficientDrawFrameList.AddRange(_groundList);
            AddToFrame(_road.Top, LeftRoadMiddleOfLeftLane, _road.Design);
            AddToFrame(_road.Top, LeftRoadMiddleOfRightLane, _road.Design);
            AddToFrame(_road.Top, RightRoadMiddleOfLeftLane, _road.Design);
            AddToFrame(_road.Top, RightRoadMiddleOfRightLane, _road.Design);
            AddToFrame(_car1.ObjectSizeAndLocation.Top, _car1.ObjectSizeAndLocation.Left, _car1.Design);
            AddToFrame(_hostCar.ObjectSizeAndLocation.Top, _hostCar.ObjectSizeAndLocation.Left, _hostCar.Design);
            if (_car1.Score > ScoreDivider || _hostCar.Score > ScoreDivider)
            {
                AddToFrame(_enemyCar1.ObjectSizeAndLocation.Top, _enemyCar1.ObjectSizeAndLocation.Left, _enemyCar1.Design);
                AddToFrame(_enemyCar2.ObjectSizeAndLocation.Top, _enemyCar2.ObjectSizeAndLocation.Left, _enemyCar2.Design);
            }
            AddToFrame(27, 95, GetScoreDisplayList()); _efficientDrawFrameList.Reverse();
            _loop.Draw(0, _efficientDrawFrameList.Count - 1, _efficientDrawFrameList);
        }

        private List<string> GetScoreDisplayList()
        {
            var scoreDisplayList = new List<string>
            {
                Convert.ToString(_car1.Score / ScoreDivider, CultureInfo.InvariantCulture),
                "",
                "",
                Convert.ToString(_hostCar.Score / ScoreDivider, CultureInfo.InvariantCulture)
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
            var frame14 = new List<string>
            {
                "                  ",
                "                  ",
                "                  ",
                "                  ",
                "                  ",
                "                  ",
                "                  ",
                "                  ",
                "                  ",
                "                  ",
                "                  ",
                "                  "
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

        private List<string> GiveRightAnimationFrame(int durationOfDeath)
        {


            if (durationOfDeath >= 0 && durationOfDeath < 6)
            {
                return animationFrameList[0];
            }

            if (durationOfDeath >= 6 && durationOfDeath < 8)
            {
                return animationFrameList[1];
            }

            if (durationOfDeath >= 8 && durationOfDeath < 12)
            {
                return animationFrameList[2];
            }

            if (durationOfDeath >= 12 && durationOfDeath < 16)
            {
                return animationFrameList[3];
            }

            if (durationOfDeath >= 16 && durationOfDeath < 21)
            {
                return animationFrameList[4];
            }

            if (durationOfDeath >= 21 && durationOfDeath < 38)
            {
                return animationFrameList[5];
            }

            if (durationOfDeath >= 38 && durationOfDeath < 46)
            {
                return animationFrameList[6];
            }

            if (durationOfDeath >= 46 && durationOfDeath < 47)
            {
                return animationFrameList[7];
            }

            if (durationOfDeath >= 47 && durationOfDeath < 56)
            {
                return animationFrameList[8];
            }

            if (durationOfDeath >= 56 && durationOfDeath < 66)
            {
                return animationFrameList[9];
            }

            if (durationOfDeath >= 66 && durationOfDeath < 72)
            {
                return animationFrameList[10];
            }

            if (durationOfDeath >= 72 && durationOfDeath < 74)
            {
                return animationFrameList[11];
            }
            if (durationOfDeath >= 74)
            {
                return animationFrameList[12];
            }
            return null;
        }
    }
}

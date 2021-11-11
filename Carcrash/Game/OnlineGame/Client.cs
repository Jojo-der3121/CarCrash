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
        private bool _enemyDiedLast = false;
        private int _deadLeft = 0;
        private readonly Road _road = new Road();
        private Explosion _explosion;
        private Car _car1;
        private Car _hostCar = new Car(31, 1, 1);
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
        private int _deathDesignIndex = 0;
        private readonly Host _host;
        private bool _enemyCar1DesignWasChanged = false;
        private bool _enemyCar2DesignWasChanged= false;

        public Client(Settings settings)
        {
            _settings = settings;
            _explosion = new Explosion();
            _loop = new GameLoop(_settings);
            _host = new Host(_settings);
            _groundList = _host.FillGroundList();
            _car1 = new Car(74, 2, _settings.DifficultyLevel);
        }

        public void ConnectToServer()
        {
            DrawConnectingScreen();
            try
            {
                _client = new TcpClient(_settings.Ip, 307);
            }
            catch (Exception)
            {
                Console.Clear();
                Console.SetCursorPosition(45, 13);
                Console.Write("UNABLE TO CONNECT TO SERVER .. =(");
                Console.SetCursorPosition(39, 15);
                Console.Write("(you will be send back to the main menu.. =/)");
                Thread.Sleep(1500);
                Console.Clear();
                var menu = new Menu();
                menu.Start(_settings);
            }
            _stream = _client.GetStream();
            _streamW = new StreamWriter(_stream);
            _streamW.AutoFlush = true;
            _streamR = new StreamReader(_stream);
            var thread = new Thread(GetServerAnswer);
            thread.Start();
            PlayAndTestGame();
        }

        private void DrawConnectingScreen()
        {
            var loadingScreenList = new List<string>
            {
                "┌─┐               │",
                "│  ┌─┐├─┐├─┐┌─┐┌─ ┼.├─┐┌─┐",
                "└─┘└─┘│ ││ │└──└─ │││ │└─┤ . . .",
                "                       └─┘"
            };
            loadingScreenList.Reverse();
            _loop.Draw(80, 25, loadingScreenList);
        }

        private void PlayAndTestGame()
        {
            try
            {
                ClientGameLoop();
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
            _loop.Die(_car1.Score / ScoreDivider);
        }

        private void ClientGameLoop()
        {
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
                        _explosion.PlayExplosionAnimation(_car1.ObjectSizeAndLocation.Top, _car1.ObjectSizeAndLocation.Left, _settings);
                        break;
                    }
                }
                else
                {
                    Thread.Sleep(10);
                    _car1 = ChangeToDeadCar(_car1);
                }
                if (_enemyDiedLast || _hostCar.Dead && _car1.Dead)
                {
                    _explosion.PlayExplosionAnimation(_hostCar.ObjectSizeAndLocation.Top, _hostCar.ObjectSizeAndLocation.Left, _settings);
                    break;
                }
                if (_hostCar.Dead && _deathDesignIndex < 74)
                {
                    _hostCar = ChangeToDeadCar(_hostCar);
                }
                _road.Movement();
                _car1.Steer();
                GiveHostClientCarInfo();
            }
        }

        private void GiveHostClientCarInfo()
        {
            _streamW.WriteLine("clientCar Left:" + _car1.ObjectSizeAndLocation.Left);
            _streamW.WriteLine("clientCar Top:" + _car1.ObjectSizeAndLocation.Top);
            _streamW.WriteLine("clientCar deadStatus:" + _car1.Dead);
            _streamW.WriteLine("clientCar Score:" + _car1.Score);
        }

        private Car ChangeToDeadCar(Car car)
        {
            if (_deathDesignIndex == 0)
            {
                _deadLeft = car.ObjectSizeAndLocation.Left;
            }
            car.Design = _explosion.GiveRightAnimationFrame(_deathDesignIndex);
            car.ObjectSizeAndLocation.Left = _deadLeft - car.Design[car.Design.Count - 1].Length / 2;
            _deathDesignIndex++;
            return car;
        }

        private void GetServerAnswer()
        {
            try
            {
                while (true)
                {
                    var serverInformation = _streamR.ReadLine();
                    var whichServerInformation = _host.GetInformationValue(serverInformation);
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

        private bool CheckForCollisions(ObjectSizeAndLocation objectSizeAndLocationA)
        {
            var allEnemyCollisionBoarders = new List<ObjectSizeAndLocation>
            {
                _enemyCar1.ObjectSizeAndLocation,
                _enemyCar2.ObjectSizeAndLocation
            };
            
                if (_loop.CalculateCollision(objectSizeAndLocationA, allEnemyCollisionBoarders))
                {
                    return true;
                }
            
            return false;
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
                ChangeEnemyCarDesign();
                AddToFrame(_enemyCar1.ObjectSizeAndLocation.Top, _enemyCar1.ObjectSizeAndLocation.Left, _enemyCar1.Design);
                AddToFrame(_enemyCar2.ObjectSizeAndLocation.Top, _enemyCar2.ObjectSizeAndLocation.Left, _enemyCar2.Design);
            }
            AddToFrame(27, 95, GetScoreDisplayList()); _efficientDrawFrameList.Reverse();
            _loop.Draw(0, _efficientDrawFrameList.Count - 1, _efficientDrawFrameList);
        }

        private void ChangeEnemyCarDesign()
        {
            if (_enemyCar1.ObjectSizeAndLocation.Left < 60)
            {
                _enemyCar1.ChangeDesign();
                _enemyCar1DesignWasChanged = true;
            }
            else if (_enemyCar1DesignWasChanged)
            {
                _enemyCar1.Design = _enemyCar1.AutoModel();
            }
            if (_enemyCar2.ObjectSizeAndLocation.Left < 60 - 42)
            {
                _enemyCar2.ChangeDesign();
                _enemyCar2DesignWasChanged = true;
            }
            else if (_enemyCar2DesignWasChanged)
            {
                _enemyCar2.Design = _enemyCar2.AutoModel();
            }
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

    }
}

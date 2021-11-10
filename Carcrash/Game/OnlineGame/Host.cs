using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Carcrash.Options;

namespace Carcrash.Game.OnlineGame
{
    class Host
    {
        private readonly List<string> _groundList;
        private readonly List<List<string>> _animationFrameList;
        private bool _enemyDiedLast = false;
        private int _deadLeft = 0;
        private readonly Road _road = new Road();
        private readonly Car _car1;
        private readonly Car _clientCar = new Car(74, 2, 1);
        private readonly Cars _enemyCar1 = new Cars(74);
        private readonly Cars _enemyCar2 = new Cars(74 - 43);
        private const int ScoreDivider = 25;
        private const int RightRoadLeftBoarder = 45;
        private const int RightRoadRightBoarder = 45 + 19;
        private const int LeftRoadLeftBoarder = 2;
        private const int LeftRoadRightBoarder = 2 + 19;
        private const int ExtraScore = 9;
        private const int NoDeviation = 0;
        private const int LeftRoadMiddleOfLeftLane = 10;
        private const int LeftRoadMiddleOfRightLane = 29;
        private const int RightRoadMiddleOfLeftLane = 53;
        private const int RightRoadMiddleOfRightLane = 72;
        private readonly Settings _settings;
        private TcpListener _listener;
        private TcpClient _client;
        private Stream _stream;
        private StreamReader _streamR;
        private StreamWriter _streamW;
        private readonly GameLoop _loop;
        private readonly List<string> _efficientDrawFrameList = new List<string>();


        public Host(Settings settings)
        {
            _settings = settings;
            _groundList = FillGroundList();
            _loop = new GameLoop(_settings);
            _car1 = new Car(31, 1, _settings.DifficultyLevel);
            _animationFrameList = _loop.FillAnimationList();
        }

        public void BootServer()
        {
            _listener = new TcpListener(307);
            _listener.Start();
            DrawLoadingScreen();
            AnimateLoadingScreen();
            _client = _listener.AcceptTcpClient();
            _stream = _client.GetStream();
            _listener.Stop();
            _streamW = new StreamWriter(_stream);
            _streamW.AutoFlush = true;
            _streamR = new StreamReader(_stream);
            var thread = new Thread(GetClientAnswer);
            thread.Start();
            PlayGame();
        }

        private void AnimateLoadingScreen()
        {
            while (_listener.Pending() == false)
            {
                Thread.Sleep(400);
                for (var i = 0; i < 3; i++)
                {
                    Console.SetCursorPosition(101 + i * 2, 24);
                    Console.Write(".");
                    if (_listener.Pending())
                    {
                        break;
                    }
                    Thread.Sleep(400);
                }
                Console.SetCursorPosition(101, 24);
                Console.WriteLine("        ");
                if (Console.KeyAvailable)
                {
                    var input = Console.ReadKey();
                    if (input.Key == ConsoleKey.Backspace)
                    {
                        _listener.Stop();
                        Console.Clear();
                        var menu = new Menu();
                        menu.Start(_settings);
                    }
                }
            }
        }

        private void DrawLoadingScreen()
        {
            var loadingScreenList = new List<string>
            {
                "                └─┘",
                "└──└─┘└─┴└─┘││ │└─┤",
                "│  ┌─┐┌─┐┌─┤.├─┐┌─┐",
                "│          │"
            };
            _loop.Draw(80, 25, loadingScreenList);
        }

        private void PlayGame()
        {
            try
            {
                var clientCarDeathDesignIndex = 0;
                var car1DeathDesignIndex = 0;
                while (true)
                {
                    EfficientDraw();
                    Thread.Sleep(10);
                    if (!_car1.Dead)
                    {
                        CheckIfDeadAndGiveScore();
                        if (_car1.Dead && _clientCar.Dead)
                        {
                            _streamW.WriteLine("Play Explosion");
                            _loop.PlayExplosionAnimation(_car1.ObjectSizeAndLocation.Top, _car1.ObjectSizeAndLocation.Left, _animationFrameList, _settings.Sound);
                            break;
                        }
                        _car1.Steer();
                    }
                    else if (car1DeathDesignIndex < 74)
                    {
                        Thread.Sleep(10);
                        if (car1DeathDesignIndex == 0)
                        {
                            _deadLeft = _car1.ObjectSizeAndLocation.Left;
                        }
                        _car1.Design = GiveRightAnimationFrame(car1DeathDesignIndex);
                        _car1.ObjectSizeAndLocation.Left = _deadLeft - _car1.Design[_car1.Design.Count - 1].Length / 2;
                        car1DeathDesignIndex++;
                    }
                    else
                    {
                        Thread.Sleep(10);
                    }
                    if (_enemyDiedLast)
                    {
                        _loop.PlayExplosionAnimation(_clientCar.ObjectSizeAndLocation.Top, _clientCar.ObjectSizeAndLocation.Left, _animationFrameList, _settings.Sound);
                        break;
                    }
                    if (_clientCar.Dead && clientCarDeathDesignIndex < 74)
                    {
                        if (clientCarDeathDesignIndex == 0)
                        {
                            _deadLeft = _clientCar.ObjectSizeAndLocation.Left;
                        }
                        _clientCar.Design = GiveRightAnimationFrame(clientCarDeathDesignIndex);
                        _clientCar.ObjectSizeAndLocation.Left = _deadLeft - _clientCar.Design[_clientCar.Design.Count - 1].Length / 2;
                        clientCarDeathDesignIndex++;
                    }
                    _streamW.WriteLine("hostCar Left:" + _car1.ObjectSizeAndLocation.Left);
                    _streamW.WriteLine("hostCar Top:" + _car1.ObjectSizeAndLocation.Top);
                    _streamW.WriteLine("hostCar deadStatus:" + _car1.Dead);
                    _streamW.WriteLine("hostCar Score:" + _car1.Score);
                    _road.Movement();
                    if (_car1.Score > ScoreDivider || _clientCar.Score > ScoreDivider)
                    {
                        _enemyCar1.Movement(NoDeviation);
                        _enemyCar2.Movement(42);
                        _streamW.WriteLine("enemyCar1Left:" + _enemyCar1.ObjectSizeAndLocation.Left);
                        _streamW.WriteLine("enemyCar1Top:" + _enemyCar1.ObjectSizeAndLocation.Top);
                        _streamW.WriteLine("enemyCar2Left:" + _enemyCar2.ObjectSizeAndLocation.Left);
                        _streamW.WriteLine("enemyCar2Top:" + _enemyCar2.ObjectSizeAndLocation.Top);
                    }

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

        private void GetClientAnswer()
        {
            try
            {
                while (true)
                {
                    var clientInformation = _streamR.ReadLine();
                    var whichClientInformation = GetInformationValue(clientInformation);
                    switch (whichClientInformation[0])
                    {
                        case "clientCar Left":
                            _clientCar.ObjectSizeAndLocation.Left = Convert.ToInt32(whichClientInformation[1]);
                            break;
                        case "clientCar Top":
                            _clientCar.ObjectSizeAndLocation.Top = Convert.ToInt32(whichClientInformation[1]);
                            break;
                        case "clientCar deadStatus":
                            _clientCar.Dead = Convert.ToBoolean(whichClientInformation[1]);
                            break;
                        case "clientCar Score":
                            _clientCar.Score = Convert.ToDouble(whichClientInformation[1]);
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

        private string[] GetInformationValue(string clientInformation)
        {
            var informationArray = clientInformation.Split(":");
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

        private void EfficientDraw()
        {
            _efficientDrawFrameList.Clear();
            _efficientDrawFrameList.AddRange(_groundList);
            AddToFrame(_road.Top, LeftRoadMiddleOfLeftLane, _road.Design);
            AddToFrame(_road.Top, LeftRoadMiddleOfRightLane, _road.Design);
            AddToFrame(_road.Top, RightRoadMiddleOfLeftLane, _road.Design);
            AddToFrame(_road.Top, RightRoadMiddleOfRightLane, _road.Design);
            AddToFrame(_car1.ObjectSizeAndLocation.Top, _car1.ObjectSizeAndLocation.Left, _car1.Design);
            AddToFrame(_clientCar.ObjectSizeAndLocation.Top, _clientCar.ObjectSizeAndLocation.Left, _clientCar.Design);
            if (_car1.Score > ScoreDivider || _clientCar.Score > ScoreDivider)
            {
                AddToFrame(_enemyCar1.ObjectSizeAndLocation.Top, _enemyCar1.ObjectSizeAndLocation.Left, _enemyCar1.Design);
                AddToFrame(_enemyCar2.ObjectSizeAndLocation.Top, _enemyCar2.ObjectSizeAndLocation.Left, _enemyCar2.Design);
            }
            AddToFrame(27, 95, GetScoreDisplayList());
            _efficientDrawFrameList.Reverse();
            _loop.Draw(0, _efficientDrawFrameList.Count - 1, _efficientDrawFrameList);
        }

        private List<string> GetScoreDisplayList()
        {
            var scoreDisplayList = new List<string>
            {
                Convert.ToString(_car1.Score / ScoreDivider, CultureInfo.InvariantCulture),
                "",
                "",
                Convert.ToString(_clientCar.Score / ScoreDivider, CultureInfo.InvariantCulture)
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

        private List<string> GiveRightAnimationFrame(int durationOfDeath)
        {

            if (durationOfDeath >= 0 && durationOfDeath < 6)
            {
                return _animationFrameList[0];
            }

            if (durationOfDeath >= 6 && durationOfDeath < 8)
            {
                return _animationFrameList[1];
            }

            if (durationOfDeath >= 8 && durationOfDeath < 12)
            {
                return _animationFrameList[2];
            }

            if (durationOfDeath >= 12 && durationOfDeath < 16)
            {
                return _animationFrameList[3];
            }

            if (durationOfDeath >= 16 && durationOfDeath < 21)
            {
                return _animationFrameList[4];
            }

            if (durationOfDeath >= 21 && durationOfDeath < 38)
            {
                return _animationFrameList[5];
            }

            if (durationOfDeath >= 38 && durationOfDeath < 46)
            {
                return _animationFrameList[6];
            }

            if (durationOfDeath >= 46 && durationOfDeath < 47)
            {
                return _animationFrameList[7];
            }

            if (durationOfDeath >= 47 && durationOfDeath < 56)
            {
                return _animationFrameList[8];
            }

            if (durationOfDeath >= 56 && durationOfDeath < 66)
            {
                return _animationFrameList[9];
            }

            if (durationOfDeath >= 66 && durationOfDeath < 72)
            {
                return _animationFrameList[10];
            }

            if (durationOfDeath >= 72 && durationOfDeath < 74)
            {
                return _animationFrameList[11];
            }
            if (durationOfDeath >= 74)
            {
                return _animationFrameList[12];
            }
            return null;
        }

    }
}

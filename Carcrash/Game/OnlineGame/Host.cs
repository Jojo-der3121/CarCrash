using System;
using System.Collections.Generic;
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
        private readonly Road _road = new Road();
        private readonly Car _car1;
        private readonly Car _clientCar = new Car(74, 2, 1);
        private readonly Cars _enemyCar1 = new Cars(74);
        private readonly Cars _enemyCar2 = new Cars(74 - 43);
        private const int ScoreDivider = 25;
        private const int ScoreBoardLeft = 95;
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


        public Host(Settings settings)
        {
            _settings = settings;
            _groundList = FillGroundList();
            _loop = new GameLoop(_settings);
            _car1 = new Car(31, 1, _settings.DifficultyLevel);
        }

        public void BootServer()
        {
            _listener = new TcpListener(307);
            _listener.Start();
            DrawLoadingScreen();
            AnimateLoadingScreen();
            _client = _listener.AcceptTcpClient();
            _stream = _client.GetStream();
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
                while (true)
                {
                    DrawGroundAndRoad();
                    _loop.Draw(_clientCar.ObjectSizeAndLocation.Left, _clientCar.ObjectSizeAndLocation.Top, _clientCar.Design);
                    _car1.Steer();
                    _streamW.WriteLine("hostCar Left:" + _car1.ObjectSizeAndLocation.Left);
                    _streamW.WriteLine("hostCar Top:" + _car1.ObjectSizeAndLocation.Top);
                    _loop.Draw(_car1.ObjectSizeAndLocation.Left, _car1.ObjectSizeAndLocation.Top, _car1.Design);
                    DrawScores();

                    if (_car1.Score > ScoreDivider + 250 || _clientCar.Score > ScoreDivider + 250)
                    {
                        DrawAndSteerEnemyCars();
                        _streamW.WriteLine("enemyCar1Left:" + _enemyCar1.ObjectSizeAndLocation.Left);
                        _streamW.WriteLine("enemyCar1Top:" + _enemyCar1.ObjectSizeAndLocation.Top);
                        _streamW.WriteLine("enemyCar2Left:" + _enemyCar2.ObjectSizeAndLocation.Left);
                        _streamW.WriteLine("enemyCar2Top:" + _enemyCar2.ObjectSizeAndLocation.Top);
                    }
                    Thread.Sleep(10);

                    _road.Movement();

                    if (!_car1.Dead)
                    {
                        CheckIfDeadAndGiveScore();
                    }
                    else
                    {
                        Thread.Sleep(10);
                    }
                    _streamW.WriteLine("hostCar deadStatus:" + _car1.Dead);
                    _streamW.WriteLine("hostCar Score:" + _car1.Score);
                    if (_car1.Dead && _clientCar.Dead)
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

        private void DrawGroundAndRoad()
        {
            _loop.Draw(0, _groundList.Count - 1, _groundList);
            _loop.Draw(LeftRoadMiddleOfLeftLane, _road._top, _road.Design);
            _loop.Draw(LeftRoadMiddleOfRightLane, _road._top, _road.Design);
            _loop.Draw(RightRoadMiddleOfLeftLane, _road._top, _road.Design);
            _loop.Draw(RightRoadMiddleOfRightLane, _road._top, _road.Design);
            _loop.Draw(_car1.ObjectSizeAndLocation.Left, _car1.ObjectSizeAndLocation.Top, _car1.Design);

        }

        private void DrawScores()
        {
            Console.SetCursorPosition(ScoreBoardLeft, 27);
            Console.Write(_clientCar.Score / ScoreDivider);
            Console.SetCursorPosition(ScoreBoardLeft, 24);
            Console.Write(_car1.Score / ScoreDivider);
        }

        private void DrawAndSteerEnemyCars()
        {
            _loop.Draw(_enemyCar1.ObjectSizeAndLocation.Left, _enemyCar1.ObjectSizeAndLocation.Top, _enemyCar1.Design);
            _loop.Draw(_enemyCar2.ObjectSizeAndLocation.Left, _enemyCar2.ObjectSizeAndLocation.Top, _enemyCar2.Design);
            _enemyCar1.Movement(NoDeviation);
            _enemyCar2.Movement(42);
        }

        private void CheckIfDeadAndGiveScore()
        {
            if (_car1.ObjectSizeAndLocation.Left < LeftRoadRightBoarder -2 && _car1.ObjectSizeAndLocation.Left > LeftRoadLeftBoarder -5 && !_car1.Dead || _car1.ObjectSizeAndLocation.Left < RightRoadRightBoarder -2 && _car1.ObjectSizeAndLocation.Left > RightRoadLeftBoarder -5 && !_car1.Dead)
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
            groundListModel[5] = groundListModel[5].Insert(95, "Car2Score:").Remove(120);
            groundListModel[8] = groundListModel[8].Insert(95, "Car1Score:").Remove(120);
            return groundListModel;



        }
        //die drei
        //private List<string> CreateLocationList(List<int> dimensionList, int y, int x)
        //{
        //    var locationList = new List<string>();
        //    var cacheList = new List<string>();
        //    for (var i = 0; i < dimensionList[0]; i++)
        //    {
        //        cacheList.Add(Convert.ToString(y + i));
        //        for (var e = 0; e < dimensionList[1]; e++)
        //        {
        //            locationList.Add(cacheList[i] + ":" + Convert.ToString(x + e));
        //        }

        //    }
        //    return locationList;
        //}

        //private bool CalculateCollision(ObjectSizeAndLocation objectSizeAndLocationA, ObjectSizeAndLocation objectSizeAndLocationB)
        //{
        //    var locationsObjectA = CreateLocationList(objectSizeAndLocationA.CollisionDimensions, objectSizeAndLocationA.Top, objectSizeAndLocationA.Left);
        //    var locationsObjectB = CreateLocationList(objectSizeAndLocationB.CollisionDimensions, objectSizeAndLocationB.Top, objectSizeAndLocationB.Left);

        //    foreach (var locationA in locationsObjectA)
        //    {
        //        foreach (var locationB in locationsObjectB)
        //        {
        //            if (locationA == locationB)
        //            {
        //                return true;
        //            }
        //        }
        //    }

        //    return false;

        //}

        //private void Die(double score)
        //{
        //    Console.Clear();
        //    var deathMessage = new List<string>()
        //    {
        //        "╚═════════════════════════════════════════════════╝",
        //        "║   sincerly ~Jojo                                ║",
        //        "║                                                 ║",
        //        "║ in the meantime =)                              ║",
        //        "║ You will be redirected to the Leader Board      ║",
        //        "║                                                 ║",
        //        "║ We will hold a minute of silence in your honor. ║",
        //        "║ You are surely gonna be missed..                ║",
        //        "║                                                 ║",
        //        "║  You have died,                                 ║",
        //        "║                                                 ║",
        //        "╔═════════════════════════════════════════════════╗"
        //    };
        //    _loop.Draw(35, 20, deathMessage);
        //    Thread.Sleep(1500);
        //    var menu = new Menu();
        //    menu.PressEnterToContinue("leader boards",23,40);
        //    var leaderBoard = new LeaderBoard();
        //    Console.Clear();
        //    leaderBoard.CreateLeaderBoard(score, _settings);
        //}
    }
}

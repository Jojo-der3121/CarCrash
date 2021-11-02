﻿using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using Carcrash.Options;

namespace Carcrash.Game.OnlineGame
{
    class Client
    {
        private readonly List<string> _groundList;
        private readonly Road _road = new Road();
        private readonly Car _car1 = new Car(74, 2);
        private readonly Car _hostCar = new Car(31, 1);
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
        private Settings _settings;
        private TcpClient client;
        private Stream stream;
        private StreamReader streamR;
        private StreamWriter streamW;
        private GameLoop _loop;

        public Client(Settings settings)
        {
            _settings = settings;
            _groundList = FillGroundList();
            _loop = new GameLoop(_settings);
        }

        public void ConnectToServer()
        {
            client = new TcpClient("localhost", 307);
            stream = client.GetStream();
            streamW = new StreamWriter(stream);
            streamW.AutoFlush = true;
            streamR = new StreamReader(stream);
            var thread = new Thread(GetServerAnswer);
            thread.Start();
            PlayGame();
        }

        private void PlayGame()
        {
            try
            {
                while (true)
                {
                    DrawGroundAndRoad();
                    _loop.Draw(_hostCar.ObjectSizeAndLocation.Left, _hostCar.ObjectSizeAndLocation.Top, _hostCar.Design);
                    _loop.Draw(_car1.ObjectSizeAndLocation.Left, _car1.ObjectSizeAndLocation.Top, _car1.Design);

                    _hostCar.Score = DrawScoresAndGetClientScore();
                    if (_car1.Score > ScoreDivider|| _hostCar.Score > ScoreDivider)
                    {
                        streamW.WriteLine("give enemyCar1Left");
                        _enemyCar1.ObjectSizeAndLocation.Left = Convert.ToInt32(streamR.ReadLine());
                        streamW.WriteLine("give enemyCar1Top");
                        _enemyCar1.ObjectSizeAndLocation.Top = Convert.ToInt32(streamR.ReadLine());
                        streamW.WriteLine("give enemyCar2Left");
                        _enemyCar2.ObjectSizeAndLocation.Left = Convert.ToInt32(streamR.ReadLine());
                        streamW.WriteLine("give enemyCar2Top");
                        _enemyCar2.ObjectSizeAndLocation.Top = Convert.ToInt32(streamR.ReadLine());
                        _loop.Draw(_enemyCar1.ObjectSizeAndLocation.Left, _enemyCar1.ObjectSizeAndLocation.Top, _enemyCar1.Design);
                        _loop.Draw(_enemyCar2.ObjectSizeAndLocation.Left, _enemyCar2.ObjectSizeAndLocation.Top, _enemyCar2.Design);
                    }
                    _car1.Steer();
                    // host steer
                    streamW.WriteLine("give Left");
                    _hostCar.ObjectSizeAndLocation.Left = Convert.ToInt32(streamR.ReadLine());
                    streamW.WriteLine("give Top");
                    _hostCar.ObjectSizeAndLocation.Top = Convert.ToInt32(streamR.ReadLine());
                    _road.Movement();
                    if (!_car1._dead)
                    {
                        CheckIfDeadAndGiveScore();
                    }
                    streamW.WriteLine("check if dead");
                    _hostCar._dead = Convert.ToBoolean(streamR.ReadLine());// check if host is dead
                    if (_car1._dead && _hostCar._dead)
                    {
                        break;
                    }
                }
                Die(_car1.Score/ScoreDivider);
            }
            catch (Exception)
            {
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
                    switch (streamR.ReadLine())
                    {
                        case "give Left":
                            streamW.WriteLine(_car1.ObjectSizeAndLocation.Left);
                            break;
                        case "give Top":
                            streamW.WriteLine(_car1.ObjectSizeAndLocation.Top);
                            break;
                        case "check if dead":
                            streamW.WriteLine(_car1._dead);
                            break;
                        case "give Score":
                            streamW.WriteLine(_car1.Score);
                            break;
                    }
                }
            }
            catch (Exception)
            {

            }
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
                if (CalculateCollision(objectSizeAndLocationA, collisionBoarderB))
                {
                    return true;
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

        private bool CalculateCollision(ObjectSizeAndLocation objectSizeAndLocationA, ObjectSizeAndLocation objectSizeAndLocationB)
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

        private void Die(double score)
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
            _loop.Draw(35, 20, deathMessage);
            Thread.Sleep(1500);
            var menu = new Menu();
            menu.PressEnterToContinue("leader boards");
            var leaderBoard = new LeaderBoard();
            Console.Clear();
            leaderBoard.CreateLeaderBoard(score, _settings);
        }

        private List<string> FillGroundList()
        {
            var groundListModel = new List<string>();
            for (var i = 0; i < 32; i++)
            {
                groundListModel.Add(" ║                 ║║                 ║     ║                 ║║                 ║                                       ");
            }
            groundListModel[5] = groundListModel[5].Insert(95, "Car2Score:").Remove(118);
            groundListModel[8] = groundListModel[8].Insert(95, "Car1Score:").Remove(118);
            return groundListModel;
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

        private double DrawScoresAndGetClientScore()
        {
            Console.SetCursorPosition(ScoreBoardLeft, 27);
            //client get score
            streamW.WriteLine("give Score");
            var clientScore = Convert.ToDouble(streamR.ReadLine());
            Console.Write(clientScore / ScoreDivider);
            Console.SetCursorPosition(ScoreBoardLeft, 24);
            Console.Write(_car1.Score / ScoreDivider);

            return clientScore;
        }

        private void CheckIfDeadAndGiveScore()
        {
            if (_car1.ObjectSizeAndLocation.Left < LeftRoadRightBoarder && _car1.ObjectSizeAndLocation.Left > LeftRoadLeftBoarder && !_car1._dead || _car1.ObjectSizeAndLocation.Left < RightRoadRightBoarder && _car1.ObjectSizeAndLocation.Left > RightRoadLeftBoarder && !_car1._dead)
            {
                _car1.Score += ExtraScore;
            }
            if (CheckForCollisions(_car1.ObjectSizeAndLocation))
            {
                _car1._dead = true;
            }
            if (!_car1._dead)
            {
                _car1.Score++;
            }
        }
    }
}

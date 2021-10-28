using System;
using System.Collections.Generic;
using System.Threading;
using Carcrash.Game;
using Carcrash.Options;

namespace Carcrash
{
    class GameLoop
    {

        private List<string> _groundList = new List<string>();
        private readonly List<string> _bufferList = new List<string>();
        private readonly Road _road = new Road();
        private readonly Car _car1 = new Car(74, 0);
        private readonly Car _car2 = new Car(74, 2);
        private readonly Cars _enemyCar1 = new Cars(74);
        private readonly Cars _enemyCar2 = new Cars(74 - 38);
        private Settings _settings = new Settings();

        public void Game(Settings settings)
        {
            _settings = settings;
            Console.CursorVisible = false;
            _groundList = FillGroundList(settings.PlayMode);
            GameMode();
        }


        private void GameMode()
        {
            if (_settings.PlayMode == PlayMode.SinglePlayer)
            {
                SinglePlayerLoop(_settings);
            }
            MultiPlayerLoop(_settings);

        }

        private void MultiPlayerLoop(Settings settings)
        {
            var car1Dead = false;
            var car2Dead = false;
            _car1._left = 31;
            _car1.Design = _car1.AutoModel(1);
            while (true)
            {
                Draw(0, 30, _groundList);
                Draw(10, _road._top, _road.Design);
                Draw(29, _road._top, _road.Design);
                Draw(53, _road._top, _road.Design);
                Draw(72, _road._top, _road.Design);
                Draw(_car1._left, _car1._top, _car1.Design);
                Draw(_car2._left, _car2._top, _car2.Design);
                Console.SetCursorPosition(95, 26);
                Console.Write(_car2.Score / 25);
                Console.SetCursorPosition(95, 23);
                Console.Write(_car1.Score / 25);
                if (_car1.Score > 300 || _car2.Score > 300)
                {
                    Draw(_enemyCar1._left, _enemyCar1._top, _enemyCar1.Design);
                    Draw(_enemyCar2._left, _enemyCar2._top, _enemyCar2.Design);
                    _enemyCar1.Movement(0);
                    _enemyCar2.Movement(38);
                }
                _car1.ApplyPlayerInput(1);
                _car2.ApplyPlayerInput(2);
                _road.Movement();
                if (_car1._left < 38 && _car1._left > 27) // fix this 
                {
                    _car1.Score += 9;
                }
                if (_car2._left < 38 && _car2._left > 27)
                {
                    _car2.Score += 9;
                }
                if (CalculateCollision(_car1.CollisionBoarder, _enemyCar1.CollisionBoarder))
                {
                    car1Dead = true;
                }

                if (CalculateCollision(_car2.CollisionBoarder, _enemyCar1.CollisionBoarder))
                {
                    car2Dead = true;
                }
                if (!car1Dead)
                {
                    _car1.Score += 1;
                }
                if (!car2Dead)
                {
                    _car2.Score += 1;
                }
                if (car1Dead && car2Dead)
                {
                    break;
                }
            }
            var scoreList = new List<double>
            {
                _car1.Score/25,
                _car2.Score/25
            };
            Die(scoreList);
        }

        private void SinglePlayerLoop(Settings settings)
        {
            Console.CursorVisible = false;
            while (true)
            {
                Draw(0, 31, _groundList);
                Console.SetCursorPosition(95, 27);
                Console.Write(_car1.Score / 25);
                Draw(53, _road._top, _road.Design);
                Draw(72, _road._top, _road.Design);
                _road.Movement();
                if (_car1.Score > 25)
                {
                    _enemyCar1.Movement(0);
                    Draw(_enemyCar1._left, _enemyCar1._top, _enemyCar1.Design);
                }
                Draw(_car1._left, _car1._top, _car1.Design);
                _car1.ApplyPlayerInput(1);
                _car1.Score += 1;
                if (_car1._left < 38 && _car1._left > 27)
                {
                    _car1.Score += 9;
                }
                if (CalculateCollision(_car1.CollisionBoarder, _enemyCar1.CollisionBoarder))
                {
                    break;
                }

            }
            var scoreList = new List<double>
            {
                _car1.Score/25
            };
            Die(scoreList);
        }

        private bool CheckForCollisions(CollisionBoarder collisionBoarderA, List<CollisionBoarder> allEnemyCollisionBoarders)
        {
            foreach (var collisionBoarderB in allEnemyCollisionBoarders)
            {
                if (CalculateCollision(collisionBoarderA, collisionBoarderB))
                {
                    return true;
                }
            }
            return false;
        }

        private List<string> FillGroundList(PlayMode playMode)
        {
            var groundListModel = new List<string>();
            switch (playMode)
            {
                case PlayMode.SinglePlayer:
                    for (var i = 0; i < 31; i++)
                    {
                        groundListModel.Add("                                            ║                 ║║                 ║                                       ");
                    }
                    groundListModel[5] = groundListModel[7].Insert(95, "Score:").Remove(118);
                    return groundListModel;
                case PlayMode.MultiPlayer:
                    for (var i = 0; i < 31; i++)
                    {
                        groundListModel.Add(" ║                 ║║                 ║     ║                 ║║                 ║                                       ");
                    }
                    groundListModel[5] = groundListModel[5].Insert(95, "Car2Score:").Remove(118);
                    groundListModel[8] = groundListModel[8].Insert(95, "Car1Score:").Remove(118);
                    return groundListModel;
                default: throw new ArgumentException(nameof(PlayMode));
            }
            
        }

        private bool CalculateCollision(CollisionBoarder collisionBoarderA, CollisionBoarder collisionBoarderB)
        {
            var locationsObjectA = CreateLocationList(collisionBoarderA.CollisionDimensions, collisionBoarderA.Top, collisionBoarderA.Left);
            var locationsObjectB = CreateLocationList(collisionBoarderB.CollisionDimensions, collisionBoarderB.Top, collisionBoarderB.Left);

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
                    Console.WriteLine(element);
                }
                y -= 1;
            }

        }

        private void Die(List<double> scoreList)
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
            menu.PressEnterToContinue("leader boards");
            var leaderBoard = new LeaderBoard();
            Console.Clear();
            leaderBoard.CreateLeaderBoard(scoreList, _settings);
        }
    }
}

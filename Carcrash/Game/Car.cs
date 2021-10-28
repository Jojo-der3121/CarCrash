using System;
using System.Collections.Generic;
using System.Threading;
using Carcrash.Game;

namespace Carcrash
{
    class Car
    {
        public int _left;
        public int _top;
        public List<int> CollisionDimensions;
        public CollisionBoarder CollisionBoarder = new CollisionBoarder();
        public double Score = 0;
        public int DesignIndex;

        public List<string> Design;

        public Car(int left, int designIndex)
        {
            _top = 26;
            _left = left;
            DesignIndex = designIndex;
            Design = AutoModel(designIndex);
            CollisionDimensions = FillCollisionDimensions();
            CollisionBoarder.Left = _left;
            CollisionBoarder.Top = _top;
            CollisionBoarder.CollisionDimensions= CollisionDimensions;
        }

        private List<int> FillCollisionDimensions()
        {
            var cacheList = new List<int>();
            cacheList.Add(6);
            cacheList.Add(5);
            return cacheList;
        }

        public List<string> AutoModel(int designIndex)
        {
            var cacheList = new List<string>();
            var autoModel = new List<string>
            {
                "└════┘",
                "║└──┘║",
                "├┌──┐┤",
                "├────┤",
                "╟────╢",
                "┌═──═┐",
            };
            switch (designIndex)
            {
                case 0:
                    return autoModel;
                case 1:
                    cacheList.Add("  p1  ");
                    cacheList.AddRange(autoModel);
                    return cacheList;
                case 2:
                    cacheList.Add("  p2  ");
                    cacheList.AddRange(autoModel);
                    return cacheList;
            }
            return null;
        }

        public void WichPlayerIsPlaying(int index)
        {
            if (index == 1)
            {
                PlayerOneSteer();
            }
            else
            {
                PlayerTwoSteer();
            }

        }

        private void PlayerOneSteer()
        {
            if (Console.KeyAvailable)
            {
                var key = new ConsoleKeyInfo();
                while (Console.KeyAvailable)
                    key = Console.ReadKey(true); // skips previous input chars

                Thread.Sleep(10);

                switch (key.Key)
                {
                    case ConsoleKey.W:
                        if (_top > 0)
                        {
                            _top--;
                        }

                        break;
                    case ConsoleKey.S:
                        if (_top < 29)
                        {
                            _top++;
                        }

                        break;
                    case ConsoleKey.A:
                        if (_left > 3)
                        {
                            _left -= 3;
                        }

                        break;
                    case ConsoleKey.D:
                        if (_left < 110)
                        {
                            _left += 3;
                        }

                        break;
                }

            }
            else
            {
                Thread.Sleep(10);
            }
        }

        private void PlayerTwoSteer()
        {
            if (Console.KeyAvailable)
            {
                var key = new ConsoleKeyInfo();
                while (Console.KeyAvailable)
                    key = Console.ReadKey(true);

                Thread.Sleep(10);

                switch (key.Key)
                {
                    case ConsoleKey.UpArrow:
                        if (_top > 0)
                        {
                            _top--;
                        }

                        break;
                    case ConsoleKey.DownArrow:
                        if (_top < 29)
                        {
                            _top++;
                        }

                        break;
                    case ConsoleKey.LeftArrow:
                        if (_left > 3)
                        {
                            _left -= 3;
                        }

                        break;
                    case ConsoleKey.RightArrow:
                        if (_left < 110)
                        {
                            _left += 3;
                        }
                        break;
                }

            }
            else
            {
                Thread.Sleep(10);
            }

        }
    }
}

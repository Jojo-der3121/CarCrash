using System;
using System.Collections.Generic;
using System.Threading;
using Carcrash.Game;

namespace Carcrash
{
    class Car
    {
       
        public List<int> ObjectDimensions;
        public ObjectSizeAndLocation ObjectSizeAndLocation = new ObjectSizeAndLocation();
        public double Score = 0;

        public List<string> Design;

        public Car(int left, int designIndex)
        {
            Design = AutoModel(designIndex);
            ObjectDimensions = FillCollisionDimensions();
            ObjectSizeAndLocation.Left = left;
            ObjectSizeAndLocation.Top = 26;
            ObjectSizeAndLocation.CollisionDimensions= ObjectDimensions;
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

        public void ApplyPlayerInput(int index)
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
                        if (ObjectSizeAndLocation.Top > 0)
                        {
                            ObjectSizeAndLocation.Top--;
                        }

                        break;
                    case ConsoleKey.S:
                        if (ObjectSizeAndLocation.Top < 29)
                        {
                            ObjectSizeAndLocation.Top++;
                        }

                        break;
                    case ConsoleKey.A:
                        if (ObjectSizeAndLocation.Left > 3)
                        {
                            ObjectSizeAndLocation.Left -= 3;
                        }

                        break;
                    case ConsoleKey.D:
                        if (ObjectSizeAndLocation.Left < 110)
                        {
                            ObjectSizeAndLocation.Left += 3;
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
                        if (ObjectSizeAndLocation.Top > 0)
                        {
                            ObjectSizeAndLocation.Top--;
                        }

                        break;
                    case ConsoleKey.DownArrow:
                        if (ObjectSizeAndLocation.Top < 29)
                        {
                            ObjectSizeAndLocation.Top++;
                        }

                        break;
                    case ConsoleKey.LeftArrow:
                        if (ObjectSizeAndLocation.Left > 3)
                        {
                            ObjectSizeAndLocation.Left -= 3;
                        }

                        break;
                    case ConsoleKey.RightArrow:
                        if (ObjectSizeAndLocation.Left < 110)
                        {
                            ObjectSizeAndLocation.Left += 3;
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

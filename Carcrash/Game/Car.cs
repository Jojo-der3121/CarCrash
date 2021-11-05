using System;
using System.Collections.Generic;
using Carcrash.Game;

namespace Carcrash
{
    class Car
    {
        public readonly ObjectSizeAndLocation ObjectSizeAndLocation = new ObjectSizeAndLocation();
        public double Score = 0;
        public bool Dead = false;
        private readonly int _difficulty;
        public readonly List<string> Design;

        public Car(int left, int designIndex, int difficulty)
        {
            _difficulty = difficulty;
            Design = AutoModel(designIndex);
            var objectDimensions = FillCollisionDimensions();
            ObjectSizeAndLocation.Left = left;
            ObjectSizeAndLocation.Top = 26;
            ObjectSizeAndLocation.CollisionDimensions = objectDimensions;
        }

        private List<int> FillCollisionDimensions()
        {
            var cacheList = new List<int>
            {
                Design[0].Length,
                Design.Count
            };
            return cacheList;
        }

        private List<string> AutoModel(int designIndex)
        {
            var autoModel = new List<string>
            {
                "┌═──═┐",
                "╟────╢",
                "├────┤",
                "├┌──┐┤",
                "║└──┘║",
                "└════┘",
            };
            switch (designIndex)
            {
                case 1:
                    autoModel.Add("  p1  ");
                    break;
                case 2:
                    autoModel.Add("  p2  ");
                    break;
            }

            autoModel.Reverse();
            return autoModel;
        }


        public void Steer()
        {
            if (!Dead)
            {
                if (Console.KeyAvailable)
                {
                    var key = new ConsoleKeyInfo();
                    while (Console.KeyAvailable)
                        key = Console.ReadKey(true);
                    switch (key.Key)
                    {
                        case ConsoleKey.W:
                        case ConsoleKey.UpArrow:
                            if (ObjectSizeAndLocation.Top > 0)
                            {
                                ObjectSizeAndLocation.Top--;
                            }
                            break;
                        case ConsoleKey.S:
                        case ConsoleKey.DownArrow:
                            if (ObjectSizeAndLocation.Top < 29)
                            {
                                ObjectSizeAndLocation.Top++;
                            }
                            break;
                        case ConsoleKey.A:
                        case ConsoleKey.LeftArrow:
                            ObjectSizeAndLocation.Left -= 2 + _difficulty;
                            if (ObjectSizeAndLocation.Left < 0)
                            {
                                ObjectSizeAndLocation.Left += Math.Abs(ObjectSizeAndLocation.Left);
                            }
                            break;
                        case ConsoleKey.D:
                        case ConsoleKey.RightArrow:
                            ObjectSizeAndLocation.Left += 2 + _difficulty;
                            if (ObjectSizeAndLocation.Left > 114)
                            {
                                ObjectSizeAndLocation.Left = 114;
                            }
                            break;
                    }
                }
            }
            else if (ObjectSizeAndLocation.Top < 37)
            {
                ObjectSizeAndLocation.Top++;
            }
        }
    }
}

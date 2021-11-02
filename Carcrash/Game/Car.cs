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
        public bool _dead = false;

        public List<string> Design;

        public Car(int left, int designIndex)
        {
            Design = AutoModel(designIndex);
            ObjectDimensions = FillCollisionDimensions();
            ObjectSizeAndLocation.Left = left;
            ObjectSizeAndLocation.Top = 26;
            ObjectSizeAndLocation.CollisionDimensions = ObjectDimensions;
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
            if (!_dead)
            {
                if (Console.KeyAvailable)
                {
                    var key = new ConsoleKeyInfo();
                    while (Console.KeyAvailable)
                        key = Console.ReadKey(true);

                    Thread.Sleep(10);

                    if (key.Key == ConsoleKey.W && ObjectSizeAndLocation.Top > 0)
                    {
                        ObjectSizeAndLocation.Top--;
                    }
                    else if (key.Key == ConsoleKey.S && ObjectSizeAndLocation.Top < 29)
                    {
                        ObjectSizeAndLocation.Top++;
                    }
                    else if (key.Key == ConsoleKey.A && ObjectSizeAndLocation.Left > 3)
                    {
                        ObjectSizeAndLocation.Left -= 3;
                    }
                    else if (key.Key == ConsoleKey.D && ObjectSizeAndLocation.Left < 110)
                    {
                        ObjectSizeAndLocation.Left += 3;
                    }
                }
                else
                {
                    Thread.Sleep(10);
                }
            }
            else if (ObjectSizeAndLocation.Top < 37)
            {
                ObjectSizeAndLocation.Top++;
            }
        }
    }
}

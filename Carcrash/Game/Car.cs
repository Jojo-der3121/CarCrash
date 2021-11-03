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
        private int _difficulty;
        public List<string> Design;

        public Car(int left, int designIndex, int difficulty)
        {
            _difficulty = difficulty;
            Design = AutoModel(designIndex);
            ObjectDimensions = FillCollisionDimensions();
            ObjectSizeAndLocation.Left = left;
            ObjectSizeAndLocation.Top = 26;
            ObjectSizeAndLocation.CollisionDimensions = ObjectDimensions;
        }

        private List<int> FillCollisionDimensions()
        {
            var cacheList = new List<int>();
            cacheList.Add(Design[0].Length);
            cacheList.Add(Design.Count);
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
            if (!_dead)
            {
                if (Console.KeyAvailable)
                {
                    var key = new ConsoleKeyInfo();
                    while (Console.KeyAvailable)
                        key = Console.ReadKey(true);

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
                        ObjectSizeAndLocation.Left -= 4 + _difficulty;
                    }
                    else if (key.Key == ConsoleKey.D && ObjectSizeAndLocation.Left < 110)
                    {
                        ObjectSizeAndLocation.Left += 4 + _difficulty;
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

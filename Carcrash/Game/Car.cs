using System;
using System.Collections.Generic;
using System.Threading;
using Carcrash.Enums;
using Carcrash.Game;

namespace Carcrash
{
    class Car
    {

        public List<int> ObjectDimensions;
        public ObjectSizeAndLocation ObjectSizeAndLocation = new ObjectSizeAndLocation();
        public double Score = 0;
        public bool _dead = false;
        private ControlKeys controlKeys = new ControlKeys();

        public List<string> Design;

        public Car(int left, int designIndex, Controls controls)
        {
            Design = AutoModel(designIndex);
            ObjectDimensions = FillCollisionDimensions();
            ObjectSizeAndLocation.Left = left;
            ObjectSizeAndLocation.Top = 26;
            ObjectSizeAndLocation.CollisionDimensions = ObjectDimensions;
            GetControlKeys(controls);
        }

        private void

            GetControlKeys(Controls controls)
        {
            if (controls == Controls.WSAD)
            {
                controlKeys.Up = ConsoleKey.W;
                controlKeys.Down = ConsoleKey.S;
                controlKeys.Left = ConsoleKey.A;
                controlKeys.Right = ConsoleKey.D;
            }
            else
            {
                controlKeys.Up = ConsoleKey.UpArrow;
                controlKeys.Down = ConsoleKey.DownArrow;
                controlKeys.Left = ConsoleKey.LeftArrow;
                controlKeys.Right = ConsoleKey.RightArrow;
            }
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

                    if (key.Key == controlKeys.Up)
                    {
                        if (ObjectSizeAndLocation.Top > 0)
                        {
                            ObjectSizeAndLocation.Top--;
                        }
                    }
                    else if (key.Key == controlKeys.Down)
                    {
                        if (ObjectSizeAndLocation.Top < 29)
                        {
                            ObjectSizeAndLocation.Top++;
                        }
                    }
                    else if (key.Key == controlKeys.Left)
                    {
                        if (ObjectSizeAndLocation.Left > 3)
                        {
                            ObjectSizeAndLocation.Left -= 3;
                        }
                    }
                    else if (key.Key == controlKeys.Right)
                    {
                        if (ObjectSizeAndLocation.Left < 110)
                        {
                            ObjectSizeAndLocation.Left += 3;
                        }
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

        //public void PlayerTwoSteer()
        //{
        //    if (!_dead)
        //    {
        //        if (Console.KeyAvailable)
        //        {
        //            var key = new ConsoleKeyInfo();
        //            while (Console.KeyAvailable)
        //                key = Console.ReadKey(true);

        //            Thread.Sleep(10);

        //            switch (key.Key)
        //            {
        //                case ConsoleKey.UpArrow:
        //                    if (ObjectSizeAndLocation.Top > 0)
        //                    {
        //                        ObjectSizeAndLocation.Top--;
        //                    }

        //                    break;
        //                case ConsoleKey.DownArrow:
        //                    if (ObjectSizeAndLocation.Top < 29)
        //                    {
        //                        ObjectSizeAndLocation.Top++;
        //                    }

        //                    break;
        //                case ConsoleKey.LeftArrow:
        //                    if (ObjectSizeAndLocation.Left > 3)
        //                    {
        //                        ObjectSizeAndLocation.Left -= 3;
        //                    }

        //                    break;
        //                case ConsoleKey.RightArrow:
        //                    if (ObjectSizeAndLocation.Left < 110)
        //                    {
        //                        ObjectSizeAndLocation.Left += 3;
        //                    }
        //                    break;
        //            }

        //        }
        //        else
        //        {
        //            Thread.Sleep(10);
        //        }
        //    }
        //    else if (ObjectSizeAndLocation.Top < 37)
        //    {

        //        ObjectSizeAndLocation.Top++;
        //    }

        //}
    }
}

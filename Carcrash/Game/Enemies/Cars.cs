using System;
using System.Collections.Generic;
using Carcrash.Game;

namespace Carcrash
{
    class Cars
    {
        public List<string> Design;
        public readonly ObjectSizeAndLocation ObjectSizeAndLocation = new ObjectSizeAndLocation();
        private readonly Random _random = new Random();
        private int _movementSpeed = 1;
       

        public Cars(int left)
        {
            Design = AutoModel();
            var objectDimensions = FillCollisionDimensions();
            ObjectSizeAndLocation.Left = left;
            ObjectSizeAndLocation.Top = 0;
            ObjectSizeAndLocation.CollisionDimensions = objectDimensions;
            ObjectSizeAndLocation.Height = 0;
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
        public List<string> AutoModel()
        {
            var autoModel = new List<string>
            {
                "└════┘",
                "║└──┘║",
                "├┌──┐┤",
                "├────┤",
                "╟────╢",
                "┌═──═┐"
            };

            return autoModel;
        }
        public void Movement(int deviation)
        {
            ObjectSizeAndLocation.Top += _movementSpeed;
            if (ObjectSizeAndLocation.Top < 34) return;
            var leftCache = _random.Next(1, 5);
            switch (leftCache)
            {
                case 1:
                    ObjectSizeAndLocation.Left = 46 - deviation;
                    ChangeDesign();
                    _movementSpeed = 2;
                    break;
                case 2:
                    ObjectSizeAndLocation.Left = 55 - deviation;
                    ChangeDesign();
                    _movementSpeed = 2;
                    break;
                case 3:
                    ObjectSizeAndLocation.Left = 65 - deviation;
                    Design.Clear();
                    Design = AutoModel();
                    _movementSpeed = 1;
                    break;
                case 4:
                    ObjectSizeAndLocation.Left = 74 - deviation;
                    Design.Clear();
                    Design = AutoModel();
                    _movementSpeed = 1;
                    break;
            }

            ObjectSizeAndLocation.Top = 0;
        }

        public void ChangeDesign()
        {
            Design.Clear();
            Design.Add("└═──═┘");
            Design.Add("╟────╢");
            Design.Add("├────┤");
            Design.Add("├└──┘┤");
            Design.Add("║┌──┐║");
            Design.Add("┌════┐");
        }
    }
}

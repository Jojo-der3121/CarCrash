using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using Carcrash.Game;

namespace Carcrash
{
    class Cars
    {

        public int _left;
        public int _top;
        public List<string> Design;
        public List<int> CollisionDimensions;
        public CollisionBoarder CollisionBoarder = new CollisionBoarder();
        private Random random = new Random();
        private int _movementSpeed = 1;

        public Cars( int left)
        {
            _left = left;
            _top = 0;
            Design = AutoModel();
            CollisionDimensions = FillCollisionDimensions();
            CollisionDimensions = FillCollisionDimensions();
            CollisionBoarder.Left = _left;
            CollisionBoarder.Top = _top;
            CollisionBoarder.CollisionDimensions = CollisionDimensions;
        }

        private List<int> FillCollisionDimensions()
        {
            var cacheList = new List<int>();
            cacheList.Add(6);
            cacheList.Add(5);
            return cacheList;
        }
        private List<string> AutoModel()
        {
            var autoModel = new List<string>();
            
                autoModel.Add("└════┘");
                autoModel.Add("║└──┘║");
                autoModel.Add("├┌──┐┤");
                autoModel.Add("├────┤");
                autoModel.Add("╟────╢");
                autoModel.Add("┌═──═┐");
                return autoModel;
        }
        public void Movement(int deviation)
        {
            _top += _movementSpeed;
            if (_top == 34)
            {
               var leftCache = random.Next(1,5);
               switch (leftCache)
               {
                   case 1:
                       _left = 46-deviation;
                       ChangeDesign();
                       _movementSpeed = 2;
                       break;
                   case 2:
                       _left = 55-deviation;
                       ChangeDesign();
                       _movementSpeed = 2;
                       break;
                   case 3:
                       _left = 64-deviation;
                       Design.Clear();
                       Design = AutoModel();
                       _movementSpeed = 1;
                        break;
                   case 4:
                       _left = 73-deviation;
                       Design.Clear();
                       Design = AutoModel();
                       _movementSpeed = 1;
                        break;
               }

               _top = 0;
            }
        }

        private void ChangeDesign()
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

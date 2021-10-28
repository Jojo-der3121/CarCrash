using System;
using System.Collections.Generic;
using Carcrash.Game;

namespace Carcrash
{
    class Cars
    {

       
        public List<string> Design;
        public List<int> ObjectDimensions;
        public ObjectSizeAndLocation ObjectSizeAndLocation = new ObjectSizeAndLocation();
        private Random random = new Random();
        private int _movementSpeed = 1;

        public Cars( int left)
        {
             
           
            Design = AutoModel();
            ObjectDimensions = FillCollisionDimensions();
            ObjectDimensions = FillCollisionDimensions();
            ObjectSizeAndLocation.Left = left;
            ObjectSizeAndLocation.Top = 0;
            ObjectSizeAndLocation.CollisionDimensions = ObjectDimensions;
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
            ObjectSizeAndLocation.Top += _movementSpeed;
            if (ObjectSizeAndLocation.Top == 34)
            {
               var leftCache = random.Next(1,5);
               switch (leftCache)
               {
                   case 1:
                       ObjectSizeAndLocation.Left = 46-deviation;
                       ChangeDesign();
                       _movementSpeed = 2;
                       break;
                   case 2:
                       ObjectSizeAndLocation.Left = 55-deviation;
                       ChangeDesign();
                       _movementSpeed = 2;
                       break;
                   case 3:
                       ObjectSizeAndLocation.Left = 64-deviation;
                       Design.Clear();
                       Design = AutoModel();
                       _movementSpeed = 1;
                        break;
                   case 4:
                       ObjectSizeAndLocation.Left = 73-deviation;
                       Design.Clear();
                       Design = AutoModel();
                       _movementSpeed = 1;
                        break;
               }

               ObjectSizeAndLocation.Top = 0;
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

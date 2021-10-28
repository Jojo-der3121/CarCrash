using System;
using System.Collections.Generic;
using System.Text;

namespace Carcrash
{
    class Road
    {
        public int _top = 30;
        public List<string> Design;

        public Road()
        {
            Design = RoadModel();
        }

        private List<string> RoadModel()
        {
            var roadModel = new List<string>();
            for (var i = 0; i < 20; i++)
            {
                roadModel.Add("║");
                roadModel.Add("║");
                roadModel.Add(" ");
            }
            return roadModel;
        }

        public void Movement()
        {
            _top++;
            if (_top == 60)
            {
                _top = 30;
            }
        }

    }
}

using System.Collections.Generic;

namespace Carcrash
{
    class Road
    {
        public int Top = 30;
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
            Top++;
            if (Top == 60)
            {
                Top = 30;
            }
        }

    }
}

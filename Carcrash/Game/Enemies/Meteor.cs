using System;
using System.Collections.Generic;
using System.Text;
using Carcrash.Options;

namespace Carcrash.Game.Enemies
{
    class Meteor
    {
        public List<string> DestinationDesign;
        public List<string> MeteorDesign = new List<string>();
        public int[] DestinationCoordinates = new int[2];
        private Explosion _explosion;
        public readonly ObjectSizeAndLocation MeteorSizeAndLocation = new ObjectSizeAndLocation();
        private int _deathCounter = 0;
        private int _deadLeft = 0;

        public Meteor()
        {
            _explosion = new Explosion();
            DestinationDesign = GetDestinationDesign();
            MeteorDesign = GetMeteorDesign();
            DestinationCoordinates = GetDestinationCoordinates();
            var objectDimensions = FillCollisionDimensions();
            MeteorSizeAndLocation.Left = DestinationCoordinates[0] + DestinationCoordinates[1];
            MeteorSizeAndLocation.Top = 0;
            MeteorSizeAndLocation.Height = DestinationCoordinates[0] ;
            MeteorSizeAndLocation.CollisionDimensions = objectDimensions;
        }

        private List<string> GetMeteorDesign()
        {
            var design = new List<string>
            {
                "          ´",
                "        /",
                "    / / /",
                " //   ;",
                "; ╔-╗ /",
                "│ ╚-╝ /",
                "╚═───/"
            };
            design.Reverse();
            return design;
        }

        private List<int> FillCollisionDimensions()
        {
            var collisionDimensions = new List<int>
            {
                DestinationDesign[0].Length,
                DestinationDesign.Count
            };
            return collisionDimensions;
        }

        private int[] GetDestinationCoordinates()
        {
            var coordinates = new int[2];
            var randomTop = new Random();
            var randomLeft = new Random();
            coordinates[0] = randomTop.Next(1, 30);
            coordinates[1] = randomLeft.Next(1, 120);
            return coordinates;
        }

        private List<string> GetDestinationDesign()
        {
            var design = new List<string>
            {
                "╔ | ╗",
                "- . -",
                "╚ | ╝"
            };
            design.Reverse();
            return design;
        }

        public void Move()
        {
            if (MeteorSizeAndLocation.Top != DestinationCoordinates[0])
            {
                MeteorSizeAndLocation.Height--;
                MeteorSizeAndLocation.Top++;
                MeteorSizeAndLocation.Left--;
            }
            else if (_deathCounter <= 85)
            {
                if (_deathCounter == 0)
                {
                    _deadLeft = MeteorSizeAndLocation.Left;
                }
                MeteorDesign = _explosion.GiveRightAnimationFrame(_deathCounter);
                MeteorSizeAndLocation.Left = _deadLeft +2 - MeteorDesign[MeteorDesign.Count - 1].Length / 2;
                _deathCounter++;
            }
            else
            {
                _deathCounter = 0;
                MeteorDesign = GetMeteorDesign();
                DestinationCoordinates = GetDestinationCoordinates();
                MeteorSizeAndLocation.Height = DestinationCoordinates[0] ;
                MeteorSizeAndLocation.Left = DestinationCoordinates[0]+ DestinationCoordinates[1];
                MeteorSizeAndLocation.Top = 0;
            }
        }
    }
}

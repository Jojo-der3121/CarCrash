using System;
using Carcrash.Game;

namespace Carcrash.Options
{
    class Settings
    {
        public int DifficultyLevel { get; set; }
        public ConsoleColor Color { get; set; }
        public int Sound { get; set; }
        public PlayMode PlayMode { get; set; }
        public int WhichSong { get; set; }
        public string Ip { get; set; }
    }
}

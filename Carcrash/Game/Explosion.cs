using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Carcrash.Options;

namespace Carcrash.Game
{
    class Explosion
    {
        private List<List<string>> _animationFrameList;
        private Settings _settings;

        public Explosion(Settings settings)
        {
            _settings = settings;
            _animationFrameList = FillAnimationList();
        }

        private List<List<string>> FillAnimationList()
        {
            var frame1 = new List<string>
            {
                "\\ | /",
                "-(@)-",
                "/-|-\\"
            };
            frame1.Reverse();
            var frame2 = new List<string>
            {
                " /-\\",
                "(|&|)",
                "\\---/"
            };
            frame2.Reverse();
            var frame3 = new List<string>
            {
                "|-^-|",
                "\\(φ)/",
                " |´|",
                "/-=-\\"
            };
            frame3.Reverse();
            var frame4 = new List<string>
            {
                " _^|^_",
                "/(=^=)\\",
                "<:═|-;>",
                "  |;|",
                ".-═=═-.",
                " /| ,\\",
                "- - - -"
            };
            frame4.Reverse();
            var frame5 = new List<string>
            {
                "   -~^~-",
                " ( ^   ^ )",
                "/≡ -   - ≡\\",
                "<- .° ° ,->",
                " -=═_ _═=-",
                "   \\   /",
                "    | |",
                ".-═=≡ ≡=═-.",
                "   -\\ /-",
                "   /| ;\\",
                "-_-_-_-_-_-"
            };
            frame5.Reverse();
            var frame6 = new List<string>
            {
                "    _~~~`~_-\\, _",
                "   /  (     )- \\",
                " ((             |",
                " |  ( ° OWO))    )",
                "\\.  ((        ), /",
                "  ;\\\\~. - _ - / )`",
                "      \\ |^| /",
                "       ||  |",
                " ,-═=≡&≡ φ ≡&≡=═-\\",
                " ` - =≡| ≡ |═ -  ´",
                "       /  |\\",
                "  - _-~ ~ ~_~_°",
                " _°-_-_-_-_-_-;_-."

            };
            frame6.Reverse();
            var frame7 = new List<string>
            {
                "   _~*;-^_",
                "/ /^    |°\\ \\",
                " ((    c) ))",
                "  - -|- - -",
                " (( \\  |/ ))",
                "     | /",
                "     | | | ",
                " .-═=≡ ≡=═-.",
                "    -\\ /-",
                "     | |",
                "    /| ;\\",
                " -_-_-_-_-_-"
            };
            frame7.Reverse();
            var frame8 = new List<string>
            {
                "  /^~*;-_°\\",
                "( (       ) )",
                "  - -|- -/-",
                "    /  |\\",
                "    (  ;) ",
                "     |",
                "      \\",
                "     /φ",
                "    | |",
                "    |||\\",
                " .- ═ ═ -.",
                "   /´ ,\\",
                "  - - - -"
            };
            frame8.Reverse();
            var frame9 = new List<string>
            {
                "  _~ _-",
                "  /^ ~ )\\",
                "   (  ;)",
                "   \\|",
                "     \\",
                "     /",
                "   `",
                "",
                "   ´",
                "  /",
                "  φ\\",
                "  | |",
                " /-=-\\ "
            };
            frame9.Reverse();
            var frame10 = new List<string>
            {
                "  `",
                "  /",
                "  ,|",
                ".| ,\\"

            };
            frame10.Reverse();
            var frame11 = new List<string>
            {
                "  .",
                "  /",
                "_|\\"
            };
            frame11.Reverse();
            var frame12 = new List<string>
            {
                "_;"
            };
            var frame13 = new List<string>
            {
                "\\"
            };
            var animation = new List<List<string>>
            {
                frame1,
                frame2,
                frame3,
                frame4,
                frame5,
                frame6,
                frame7,
                frame8,
                frame9,
                frame10,
                frame11,
                frame12,
                frame13
            };
            return animation;
        }

        public void PlayExplosionAnimation(int top, int left, int sound)
        {
            if (sound != 0)
            {
                var menu = new Menu();
                menu.Player.SoundLocation = "mixkit-arcade-game-explosion-echo-1698-[AudioTrimmer (Joined by Happy Scribe) (1) (online-audio-converter.com).wav";
                menu.Player.Play();
            }
            var timingList = new List<int>
            {
                1450,
                150,
                200,
                275,
                300,
                1000,
                500,
                100,
                500,
                600,
                400,
                100,
                200
            };
            Thread.Sleep(175);
            for (var i = 0; i < _animationFrameList.Count; i++)
            {
                var frame = _animationFrameList[i];
                var loop = new GameLoop(_settings);
                loop.Draw(left + 3 - frame[frame.Count - 1].Length / 2, top - 4, frame);
                Thread.Sleep(timingList[i]);
                Console.Clear();
            }
        }

        public List<string> GiveRightAnimationFrame(int durationOfDeath)
        {
            if (durationOfDeath >= 0 && durationOfDeath < 6)
            {
                return _animationFrameList[0];
            }
            if (durationOfDeath >= 6 && durationOfDeath < 8)
            {
                return _animationFrameList[1];
            }
            if (durationOfDeath >= 8 && durationOfDeath < 12)
            {
                return _animationFrameList[2];
            }
            if (durationOfDeath >= 12 && durationOfDeath < 16)
            {
                return _animationFrameList[3];
            }
            if (durationOfDeath >= 16 && durationOfDeath < 21)
            {
                return _animationFrameList[4];
            }
            if (durationOfDeath >= 21 && durationOfDeath < 38)
            {
                return _animationFrameList[5];
            }
            if (durationOfDeath >= 38 && durationOfDeath < 46)
            {
                return _animationFrameList[6];
            }
            if (durationOfDeath >= 46 && durationOfDeath < 47)
            {
                return _animationFrameList[7];
            }
            if (durationOfDeath >= 47 && durationOfDeath < 56)
            {
                return _animationFrameList[8];
            }
            if (durationOfDeath >= 56 && durationOfDeath < 66)
            {
                return _animationFrameList[9];
            }
            if (durationOfDeath >= 66 && durationOfDeath < 72)
            {
                return _animationFrameList[10];
            }
            if (durationOfDeath >= 72 && durationOfDeath < 74)
            {
                return _animationFrameList[11];
            }
            return _animationFrameList[12];
        }
    }
}

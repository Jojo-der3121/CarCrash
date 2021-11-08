using System;
using System.Threading;
using Carcrash.Game;


namespace Carcrash
{
    class BackGroundMusic
    {
        public void PlaySong()
        {
            while (true)
            {
                try
                {
                    var menu = new Menu();
                    var settings = menu._settings;
                    //if (settings.PlayMode != PlayMode.MultiPlayer)
                    //{
                        if (settings.Sound != 0)
                        {
                            switch (settings.WhichSong)
                            {
                                case 1:
                                    Song1();
                                    break;
                                case 2:
                                    Song2();
                                    break;
                                case 3:
                                    Song3();
                                    break;
                            }
                        }
                    //}
                    //else
                    //{
                    //    Mute();
                    //}


                }
                catch
                {
                    Thread.Sleep(15);
                }
            }
        }

        private void Song1()
        {
            const int cSmall = 75 * 2;
            const int c = 100 * 2;
            const int d = 150 * 2;
            const int eSmall = 175 * 2;
            const int e = 200 * 2;
            const int f = 250 * 2;
            const int gSmall = 275 * 2;
            const int a = 350 * 2;
            const int pause = 1000;
            const int miniPause = 7;

            Thread.Sleep(pause);
            Console.Beep(c, pause);
            Thread.Sleep(miniPause);
            Console.Beep(d, pause);
            Thread.Sleep(miniPause);
            Console.Beep(e, pause);
            Thread.Sleep(miniPause);
            Console.Beep(f, pause);
            Thread.Sleep(miniPause);

            Console.Beep(gSmall, pause);
            Thread.Sleep(2 * miniPause);
            Console.Beep(gSmall, pause);
            Thread.Sleep(2 * miniPause);

            Console.Beep(a, pause);
            Thread.Sleep(miniPause);
            Console.Beep(a, pause);
            Thread.Sleep(miniPause);
            Console.Beep(a, pause);
            Thread.Sleep(miniPause);
            Console.Beep(a, pause);
            Thread.Sleep(miniPause);

            Console.Beep(gSmall, pause);
            Thread.Sleep(3 * miniPause);

            Console.Beep(a, pause);
            Thread.Sleep(miniPause);
            Console.Beep(a, pause);
            Thread.Sleep(miniPause);
            Console.Beep(a, pause);
            Thread.Sleep(miniPause);
            Console.Beep(a, pause);
            Thread.Sleep(miniPause);

            Console.Beep(gSmall, pause);
            Thread.Sleep(3 * miniPause);

            Console.Beep(f, pause);
            Thread.Sleep(miniPause);
            Console.Beep(f, pause);
            Thread.Sleep(miniPause);
            Console.Beep(f, pause);
            Thread.Sleep(miniPause);
            Console.Beep(f, pause);
            Thread.Sleep(miniPause);


            Console.Beep(eSmall, pause);
            Thread.Sleep(2 * miniPause);
            Console.Beep(eSmall, pause);
            Thread.Sleep(2 * miniPause);

            Console.Beep(d, pause);
            Thread.Sleep(miniPause);
            Console.Beep(d, pause);
            Thread.Sleep(miniPause);
            Console.Beep(d, pause);
            Thread.Sleep(miniPause);
            Console.Beep(d, pause);
            Thread.Sleep(miniPause);

            Console.Beep(cSmall, pause);
            Thread.Sleep(3 * miniPause);
            Thread.Sleep(pause);
        }

        private void Song2()
        {
            Console.Beep(659, 125);
            Console.Beep(659, 125);
            Thread.Sleep(125);
            Console.Beep(659, 125);
            Thread.Sleep(167);
            Console.Beep(523, 125);
            Console.Beep(659, 125);
            Thread.Sleep(125);
            Console.Beep(784, 125);
            Thread.Sleep(375);
            Console.Beep(392, 125);
            Thread.Sleep(375);
            Console.Beep(523, 125);
            Thread.Sleep(250);
            Console.Beep(392, 125);
            Thread.Sleep(250);
            Console.Beep(330, 125);
            Thread.Sleep(250);
            Console.Beep(440, 125);
            Thread.Sleep(125);
            Console.Beep(494, 125);
            Thread.Sleep(125);
            Console.Beep(466, 125);
            Thread.Sleep(42);
            Console.Beep(440, 125);
            Thread.Sleep(125);
            Console.Beep(392, 125);
            Thread.Sleep(125);
            Console.Beep(659, 125);
            Thread.Sleep(125);
            Console.Beep(784, 125);
            Thread.Sleep(125);
            Console.Beep(880, 125);
            Thread.Sleep(125);
            Console.Beep(698, 125);
            Console.Beep(784, 125);
            Thread.Sleep(125);
            Console.Beep(659, 125);
            Thread.Sleep(125);
            Console.Beep(523, 125);
            Thread.Sleep(125);
            Console.Beep(587, 125);
            Console.Beep(494, 125);
            Thread.Sleep(125);
            Console.Beep(523, 125);
            Thread.Sleep(250);
            Console.Beep(392, 125);
            Thread.Sleep(250);
            Console.Beep(330, 125);
            Thread.Sleep(250);
            Console.Beep(440, 125);
            Thread.Sleep(125);
            Console.Beep(494, 125);
            Thread.Sleep(125);
            Console.Beep(466, 125);
            Thread.Sleep(42);
            Console.Beep(440, 125);
            Thread.Sleep(125);
            Console.Beep(392, 125);
            Thread.Sleep(125);
            Console.Beep(659, 125);
            Thread.Sleep(125);
            Console.Beep(784, 125);
            Thread.Sleep(125);
            Console.Beep(880, 125);
            Thread.Sleep(125);
            Console.Beep(698, 125);
            Console.Beep(784, 125);
            Thread.Sleep(125);
            Console.Beep(659, 125);
            Thread.Sleep(125);
            Console.Beep(523, 125);
            Thread.Sleep(125);
            Console.Beep(587, 125);
            Console.Beep(494, 125);
            Thread.Sleep(375);
            Console.Beep(784, 125);
            Console.Beep(740, 125);
            Console.Beep(698, 125);
            Thread.Sleep(42);
            Console.Beep(622, 125);
            Thread.Sleep(125);
            Console.Beep(659, 125);
            Thread.Sleep(167);
            Console.Beep(415, 125);
            Console.Beep(440, 125);
            Console.Beep(523, 125);
            Thread.Sleep(125);
            Console.Beep(440, 125);
            Console.Beep(523, 125);
            Console.Beep(587, 125);
            Thread.Sleep(250);
            Console.Beep(784, 125);
            Console.Beep(740, 125);
            Console.Beep(698, 125);
            Thread.Sleep(42);
            Console.Beep(622, 125);
            Thread.Sleep(125);
            Console.Beep(659, 125);
            Thread.Sleep(167);
            Console.Beep(698, 125);
            Thread.Sleep(125);
            Console.Beep(698, 125);
            Console.Beep(698, 125);
            Thread.Sleep(625);
            Console.Beep(784, 125);
            Console.Beep(740, 125);
            Console.Beep(698, 125);
            Thread.Sleep(42);
            Console.Beep(622, 125);
            Thread.Sleep(125);
            Console.Beep(659, 125);
            Thread.Sleep(167);
            Console.Beep(415, 125);
            Console.Beep(440, 125);
            Console.Beep(523, 125);
            Thread.Sleep(125);
            Console.Beep(440, 125);
            Console.Beep(523, 125);
            Console.Beep(587, 125);
            Thread.Sleep(250);
            Console.Beep(622, 125);
            Thread.Sleep(250);
            Console.Beep(587, 125);
            Thread.Sleep(250);
            Console.Beep(523, 125);
            Thread.Sleep(1125);
        }

        private void Song3()
        {
            Console.Beep(1320, 500); Console.Beep(990, 250); Console.Beep(1056, 250); Console.Beep(1188, 250); Console.Beep(1320, 125); Console.Beep(1188, 125); Console.Beep(1056, 250); Console.Beep(990, 250); Console.Beep(880, 500); Console.Beep(880, 250); Console.Beep(1056, 250); Console.Beep(1320, 500); Console.Beep(1188, 250); Console.Beep(1056, 250); Console.Beep(990, 750); Console.Beep(1056, 250); Console.Beep(1188, 500); Console.Beep(1320, 500); Console.Beep(1056, 500); Console.Beep(880, 500); Console.Beep(880, 500); System.Threading.Thread.Sleep(250); Console.Beep(1188, 500); Console.Beep(1408, 250); Console.Beep(1760, 500); Console.Beep(1584, 250); Console.Beep(1408, 250); Console.Beep(1320, 750); Console.Beep(1056, 250); Console.Beep(1320, 500); Console.Beep(1188, 250); Console.Beep(1056, 250); Console.Beep(990, 500); Console.Beep(990, 250); Console.Beep(1056, 250); Console.Beep(1188, 500); Console.Beep(1320, 500); Console.Beep(1056, 500); Console.Beep(880, 500); Console.Beep(880, 500); System.Threading.Thread.Sleep(500); PlaySong();
        }

        private void Mute()
        {
            Thread.Sleep(10000);
        }

    }
}

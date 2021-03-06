using System;
using Carcrash.Game;
using Carcrash.Options;
using System.Media;

namespace Carcrash
{
    class Menu
    {
        private readonly OptionsMenu _optionsMenu = new OptionsMenu();
        private Settings _settings;
        public readonly SoundPlayer Player = new SoundPlayer();

        public Menu()
        {
            _settings = _optionsMenu.Deserialize(_optionsMenu.FilePath);
        }

        static void Main()
        {
            var menu = new Menu();
            Console.ForegroundColor = menu._settings.Color;
            menu.Player.SoundLocation = menu._settings.WhichSong;
            if (menu._settings.Sound != 0)
            {
                menu.Player.PlayLooping();
            }
            menu.Start(menu._settings);
        }

        public void Start(Settings settings)
        {
            _settings = settings;
            Console.CursorVisible = false;
            DrawMenuScreen();
            ExecuteSelection(SelectionProcess(15, 19, 4, 17), settings);
        }

        private static void DrawMenuScreen()
        {
            Console.WriteLine(" ╔══════════════════════════════════════════════════════════════════════╗");
            Console.WriteLine(" ║                                                                      ║");
            Console.WriteLine(" ║  #####                      #####                           ##       ║");
            Console.WriteLine(" ║ ##   ##                    ##   ##                          ##       ║");
            Console.WriteLine(" ║ ##   ##                    ##   ##                          ##       ║");
            Console.WriteLine(" ║ ##                         ##                        #####  ##       ║");
            Console.WriteLine(" ║ ##        #####   ## ##    ##       ## ##  #####    ##    # ######   ║");
            Console.WriteLine(" ║ ##   ##  ##   ##  ###      ##   ##  ###   ##   ##     ##    ##   ##  ║");
            Console.WriteLine(" ║ ##   ##  ##  ###  ##       ##   ##  ##    ##  ###  #    ##  ##   ##  ║");
            Console.WriteLine(" ║  #####    ### ##  ##        #####   ##     ### ##   #####   ##   ##  ║ ");
            Console.WriteLine(" ║                                                                      ║");
            Console.WriteLine(" ╚══════════════════════════════════════════════════════════════════════╝");
            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine("    >> play Game <<");
            Console.WriteLine("");
            Console.WriteLine("        Options");
            Console.WriteLine("");
            Console.WriteLine("         Exit");

        }

        public int SelectionProcess(int lowerBound, int upperBound, int leftBound, int rightBound)
        {
            var top = lowerBound;
            while (true)
            {
                var key = Console.ReadKey(true);

                int formerTop;
                switch (key.Key)
                {
                    case ConsoleKey.W:
                    case ConsoleKey.UpArrow:
                        formerTop = top;
                        if (top == lowerBound)
                        {
                            top = upperBound;
                        }
                        else
                        {
                            top -= 2;
                        }
                        DrawSelection(top, formerTop, leftBound, rightBound);
                        break;
                    case ConsoleKey.S:
                    case ConsoleKey.DownArrow:
                        formerTop = top;
                        if (top == upperBound)
                        {
                            top = lowerBound;
                        }
                        else
                        {
                            top += 2;
                        }
                        DrawSelection(top, formerTop, leftBound, rightBound);
                        break;
                    case ConsoleKey.Enter:
                    case ConsoleKey.Spacebar:
                        return top;

                }
            }
        }

        private static void DrawSelection(int selection, int formerSelection, int leftBound, int rightBound)
        {
            Console.SetCursorPosition(leftBound, formerSelection);
            Console.Write("  ");
            Console.SetCursorPosition(rightBound, formerSelection);
            Console.Write("  ");

            Console.SetCursorPosition(leftBound, selection);
            Console.Write(">>");
            Console.SetCursorPosition(rightBound, selection);
            Console.Write("<<");

        }

        private void ExecuteSelection(int selection, Settings settings)
        {
            var options = new OptionsMenu();
            Console.Clear();
            switch (selection)
            {
                case 15:
                    OnlineOrLocal(settings);
                    break;
                case 17:
                    options.Configurate(settings);
                    break;
                case 19:
                    LeaderBoard.Serialize(settings, options.FilePath);
                    Environment.Exit(0);
                    break;

            }
        }

        public void PressEnterToContinue(string destination, int top, int left)
        {
            Console.CursorVisible = false;
            Console.SetCursorPosition(left, top);
            Console.WriteLine("Press Enter to go to the " + destination + " OwO");
            while (Console.ReadKey(true).Key != ConsoleKey.Enter)
            {
                if (Console.KeyAvailable)
                {
                    Console.ReadKey(true);
                }
            }
            Console.Clear();
        }

        private void OnlineOrLocal(Settings settings)
        {
            var networkMenu = new NetworkMenu(settings);
            var loop = new GameLoop(settings);
            if (settings.PlayMode == PlayMode.SinglePlayer)
            {
                loop.Tutorial();
            }
            else
            {
                networkMenu.NetworkSelectionMenu();
            }
        }
    }
}

using System;
using Carcrash.Options;

namespace Carcrash
{
    class Menu
    {

        static void Main()
        {
            var menu = new Menu();
            var optionsMenu = new OptionsMenu();
            var settings = optionsMenu.Deserialize(optionsMenu.FilePath);
            Console.ForegroundColor = settings.Color;
            menu.Start(settings);
        }

        public void Start(Settings settings)
        {
            Console.CursorVisible = false;
            DrawMenuScreen();
            var selection = SelectionProcess(15,19, 4,17);
            ExecuteSelection(selection,settings);
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

        public int SelectionProcess(int lowerBound, int upperBound, int leftBound, int rightBound )
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
                        DrawSelection(top, formerTop,leftBound,rightBound);
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
                        DrawSelection(top, formerTop, leftBound,rightBound);
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

        private void ExecuteSelection(int selection,Settings settings)
        {
            var loop = new GameLoop();
            var options = new OptionsMenu();
            Console.Clear();
            switch (selection)
            {
                case 15:
                    loop.Game(settings);
                    break;
                case 17:
                    options.Configurate(settings);
                    break;
                case 19:
                    LeaderBoard.Serialize(settings,options.FilePath);
                    Environment.Exit(0);
                    break;
            }
        }

        public void PressEnterToContinue(string destination)
        {
            Console.SetCursorPosition(40, 23);
            Console.WriteLine("Press Enter to go to the " + destination + " OwO");
            while (Console.ReadKey(true).Key != ConsoleKey.Enter)
            {
                if (Console.KeyAvailable)
                {
                    Console.ReadKey();
                }
            }
            Console.Clear();
        }

    }
}

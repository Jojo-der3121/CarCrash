using System;
using System.Collections.Generic;
using System.Text;
using Carcrash.Game.OnlineGame;
using Carcrash.Options;

namespace Carcrash
{
    class NetworkMenu
    {
        private List<string> NetworkMenuHost;
        private List<string> NetworkMenuJoin;
        private List<string> NetworkMenuSelection;
        private List<string> eraseList;
        private readonly Settings _settings;
        private readonly GameLoop loop;

        public NetworkMenu(Settings settings)
        {
            _settings = settings;
            loop  = new GameLoop(_settings);
        }

        public void NetworkSelectionMenu()
        {
            DrawNetworkMenu();
            Console.SetCursorPosition(35, 25);
            Console.Write("Press \"BackSpace\" to go back to the main Menu. ^^");
            var selection = SelectionProcess(32, 71);
            if (selection == 0)
            {
                Console.Clear();
                var menu = new Menu();
                menu.Start(_settings);
            }
            ExecuteSelection(selection);
        }

        private void DrawNetworkMenu()
        {
            NetworkMenuHost = new List<string>
            {
                "╚═─────────═╝",
                "││ │└─┘└─┘│ │",
                "│├─┤┌─┐└─┐┼ │",
                "││ │   ┌─┐│ │",
                "╔═─────────═╗"
            };
            NetworkMenuJoin = new List<string>
            {

                "╚═─────────═╝",
                "│└─┘└─┘││ │ │",
                "│  │┌─┐.├─┐ │",
                "│ ─┐        │",
                "╔═─────────═╗"
            };
            NetworkMenuSelection = new List<string>
            {
                "╚═───────────═╝",
                "│             │",
                "│             │",
                "│             │",
                "│             │",
                "│             │",
                "╔═───────────═╗"
            };

            eraseList = new List<string>
            {
                "               ",
                "               ",
                "               ",
                "               ",
                "               ",
                "               ",
                "               "
            };
            loop.Draw(32, 16, NetworkMenuSelection);
            loop.Draw(33, 15, NetworkMenuJoin);
            loop.Draw(72, 15, NetworkMenuHost);

        }


        private int SelectionProcess(int leftBound, int rightBound)
        {
            var left = leftBound;
            while (true)
            {
                var key = Console.ReadKey(true);
                switch (key.Key)
                {
                    case ConsoleKey.A:
                    case ConsoleKey.LeftArrow:
                    case ConsoleKey.D:
                    case ConsoleKey.RightArrow:
                        var formerLeft = left;
                        left = left == leftBound ? rightBound : leftBound;
                        DrawSelection(left, formerLeft);
                        break;
                    case ConsoleKey.Backspace:
                        return 0;
                    case ConsoleKey.Enter:
                    case ConsoleKey.Spacebar:
                        return left;
                }
            }
        }

        private void DrawSelection(int left, int formerLeft)
        {
            loop.Draw(formerLeft, 16, eraseList);
            loop.Draw(formerLeft+1, 15, formerLeft == 32 ? NetworkMenuJoin : NetworkMenuHost);

            loop.Draw(left, 16, NetworkMenuSelection);
            loop.Draw(left+1, 15, left == 32 ? NetworkMenuJoin : NetworkMenuHost);
        }

        private void ExecuteSelection(int selection)
        {
            Console.Clear();
            var client = new Client(_settings);
            var host = new Host(_settings);
            switch (selection)
            {
                case 32:
                    client.ConnectToServer();
                    break;
                case 71:
                    host.BootServer();
                    break;
            }
        }
    }
}

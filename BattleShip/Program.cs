using System;
using System.Linq;

namespace BattleShip
{
    class Program
    {
        int boardSize = 10;
        int[,] board = new int[10, 10];
        const int ATTACK = 1, RELOAD = 2, QUIT = 3;
        const int ST_EMPTY = 1, ST_ATTACKED = 2, ST_SHIP = 4;
        static readonly int[] ALLOWED_ACTIONS = { ATTACK, RELOAD, QUIT};

        static void Main(string[] args)
        {
            new Program().Run();
        }

        public void Run()
        {
            bool running = true;
            bool victory = false;
            ResetBoard();
            PlaceRandomShips();
            int currentTurn = 0;
            do
            {
                Console.Clear();
                PrintBoard();

                switch (AskForInput())
                {
                    case ATTACK:
                        AskForAttackLocation();
                        victory = DestroyedAllShips();
                        running = !victory;
                        currentTurn++;
                        break;
                    case RELOAD:
                        ResetBoard();
                        PlaceRandomShips();
                        currentTurn = 0;
                        break;
                    case QUIT:
                        running = false;
                        break;
                }

                if(victory)
                {
                    Console.WriteLine("You win! Took you {0} turns", currentTurn);
                }
            } while (running);
            

            Console.Write("\n\nPress key to continue");
            Console.Read();
        }

        public void PrintBoard()
        {
            Console.WriteLine("Legend:\n  X=Missed Attack\n  O=Hit ship\n");

            char a = 'A';
            Console.Write("    ");
            for (int i = 1; i <= board.Length / boardSize; i++)
            {
                Console.Write("{0} ", i);
            }
            Console.WriteLine("\n    -------------------");

            for (int y = 0; y < board.Length / boardSize; y++)
            {
                Console.Write("{0} | ", a);
                for (int x = 0; x < board.Length / boardSize; x++)
                {
                    int tileValue = board[x, y];
                    if ( tileValue == ST_EMPTY || tileValue == ST_SHIP)
                    {
#if DEBUG
                        Console.Write("{0} ", tileValue);
#else
                        Console.Write("  ");
#endif
                    }
                    else if((tileValue & ST_ATTACKED) != 0)
                    {
                        // tile is attacked, if ship is there print o, if tile is empty print X
                        if ( (tileValue & ST_EMPTY) != 0)
                        {
                            Console.Write("X ");
                        }

                        if ((tileValue & ST_SHIP) != 0)
                        {
                            Console.Write("O ");
                        }
                    }
                    
                }
                a++;
                Console.WriteLine();
            }
            
        }

        public void ResetBoard()
        {
            for( int x=0; x < board.Length / boardSize; x++)
            {
                for(int y = 0; y < board.Length / boardSize; y++)
                {
                    board[x, y] = ST_EMPTY;
                }
            }
        }

        public void PlaceRandomShips()
        {
            var random = new Random();
            //always place 3 ships
            for (int s = 0; s < 3; s++)
            {
                int shipSize = random.Next(3, 5);
                bool isHorizontallyPlaced = random.Next(1, 11) > 5;
                if (isHorizontallyPlaced)
                {
                    int xStart = random.Next(10);
                    int y = random.Next(10);
                    if (xStart + shipSize >= 10)
                    {
                        xStart -= shipSize;
                    }
                    for(int x=0; x < shipSize; x++)
                    {
                        board[x + xStart, y] = ST_SHIP;
                    }
                }
                else
                {
                    int yStart = random.Next(10);
                    int x = random.Next(10);
                    if (yStart + shipSize >= 10)
                    {
                        yStart -= shipSize;
                    }

                    for (int y = 0; y < shipSize; y++)
                    {
                        board[x, y + yStart] = ST_SHIP;
                    }
                }
            }
        }

        public int AskForInput()
        {
            Console.Write("\n\n(1)Attack  (2)Restart  (3)Quit: ");
            int input;
            
            while (!int.TryParse(Console.ReadLine(), out input) || !ALLOWED_ACTIONS.Contains(input))
            {
                Console.WriteLine("Incorrect input, try again");
                Console.Write("(1)Attack  (2)Restart  (3)Quit: ");
            }
            return input;
        }

        public void AskForAttackLocation()
        {
            Console.Write("\n\n Choose Attack location (eg. A1): ");
            String input = Console.ReadLine().ToUpper();

            int row = input[0] - 'A';
            int col = int.Parse(input.Substring(1)) - 1;

            Console.WriteLine("Attacking board[{0}][{1}]", col, row);

            board[col, row] |= ST_ATTACKED;
            if ((board[col, row] | ST_SHIP) != 0)
            {
            }
        }

        bool DestroyedAllShips()
        {
            for (int y = 0; y < board.Length / boardSize; y++)
            {
                for (int x = 0; x < board.Length / boardSize; x++)
                {
                    if (board[x, y] == ST_SHIP)
                        return false;
                }
            }

            return true;
        }

        
    }
}
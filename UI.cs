using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Ročníkový_projekt_v_1._3
{
    class UI
    {

        private const int _WhiteChecker = 100;
        private const int _BlackChecker = -100;
        private const int _WhitePawn = 25;
        private const int _BlackPawn = -25;
        private const int _TopBorder = 0;
        private const int _BottomBorder = 9;
        private const int _LefBorder = 0;
        private const int _RightBorder = 9;
        // private static Brain Brain;
        private static GameManager Manager;
        // private static Chessboard Board;
        //  private static Rules Rules;

        public UI(GameManager gameManager)
        {
            Manager = gameManager;
            Start();
        }

        //pomocna metoda pro zjisteni zacinajiciho hrace
        private void Start()
        {
            Console.WriteLine("Vita vas hra Friska dama zadejte start pro zacatek hry!");
            Console.WriteLine("Pokud chcete znat vsechny prikazy pro hrani zadejte help po zobrazeni hraci desky!");
            Console.WriteLine("");
            Console.Write(">> ");

            bool konec = false;
            string command;
            do
            {
                command = Console.ReadLine().ToUpper();
                if (command.Equals("START"))
                {
                    konec = true;
                }
                else Console.WriteLine("Zadejte start pro zacatek");
            } while (konec != true);
        }

        //obsahuje smycku pro zpracovani prikazu pokud se jedna o hrace jinak po dvou vterinach provede tah vygenerovany pocitacem
        public void Play()
        {
            bool konec = false;
            
            do
            {
            if (Manager.Rules.OnMovePlayer().GetPlayerType() == 0)
                {
                    System.Threading.Thread.Sleep(2000);
                    CommandComputerMove(ref konec);
                }
                else
                {
                    ReadCommand(ref konec);
                }
            } while (konec != true);
        }

        //metoda na precteni prikazu od hrace
        private void ReadCommand(ref bool konec)
        {
            try
            {
                string command;
                command = Console.ReadLine().ToUpper();
                string[] parsed = command.Split(' ');
                switch (parsed[0])
                {
                    case "HELP":
                        {
                            if (1 == parsed.Length) HelpCommand();
                            else WarningMessage();
                        }
                        break;
                    case "KONEC":
                        {
                            if (1 == parsed.Length)
                            {
                                konec = true;
                                Console.WriteLine("Konec hry");
                            }
                            else WarningMessage();
                        }
                        break;
                    case "BEST":
                        {
                            if (1 == parsed.Length) CommandBestMove();
                            else WarningMessage();
                        }
                        break;
                    case "SHOW":
                        {
                            Regex regex1 = new Regex("[A-J][0-9]");
                            if ((regex1.IsMatch(parsed[1].ToUpper())) && (2 == parsed.Length) && (parsed[1].Length == 2)) ShowMove(parsed);
                            else if ((parsed.Length == 2) && (parsed[1].ToUpper().Equals(Manager.P1.GetName().ToUpper()))) ShowAllMoves(Manager.P1);
                            else if ((parsed.Length == 2) && (parsed[1].ToUpper().Equals(Manager.P2.GetName().ToUpper()))) ShowAllMoves(Manager.P2);
                            else WarningMessage();
                        }
                        break;
                    case "SAVE":
                        {
                            //GameManager.SaveGame();    
                        }
                        break;
                    default:
                        Regex regex3 = new Regex("[A-J][0-9] [A-J][0-9]");
                        if ((regex3.IsMatch(command.ToUpper())) && (0 == (parsed.Length % 2))) CommandPlayerMove(parsed, ref konec);
                        else WarningMessage();
                        break;
                }
            }
            catch
            {
                Console.WriteLine("Prikaz nebyl zadan koreknte nebo nemohl byt zpracovan");
            }
        }

        //metoda obsluhucici prikaz pro tah
        private void CommandPlayerMove(string[] parsed,ref bool konec)
        {
            int result = Manager.DoPlayerMove(parsed);
            PrintResult(result, ref konec);
        }

        //metoda ktera provadi tah pocitace
        public void CommandComputerMove(ref bool konec)
        {
            int result = Manager.DoComputerMove();
            PrintResult(result, ref konec);
        }

        public void HelpCommand()
        {
            ShowChessboard(Manager.Board);
            Console.WriteLine("");
            Console.WriteLine("Prikazy pro hru Friska dama:");
            Console.WriteLine("Konec - ukoncite okamzite hru");
            Console.WriteLine("Best  - napoveda nejlepsiho tahu");
            Console.WriteLine("Show  + Souradnice (ve tvaru pismeno cislice) - vypise vsechny mozne tahy z dane pozice");
            Console.WriteLine("Show  + jmeno hrace - vypise vsechny mozne tahy pro daneho hrace");
            Console.WriteLine("Souradnice ve tvaru pismeno cislice (odkud chcete tahnout) mezera \n        souradnice ve tvaru pismeno (kam chcete tahnout) pro provedeni tahu");
            Console.WriteLine("");
            ShowInfo();
        }

        public void CommandBestMove()
        {
            Move tah = Manager.Brain.GenerateBestMove(Manager.Rules.OnMovePlayer());
            ShowChessboard(Manager.Board);
            Console.WriteLine("");
            Console.WriteLine(tah.ToString());
            Console.WriteLine("");
            ShowInfo();
        }

        private void PrintResult(int result, ref bool konec)
        {
            ShowChessboard(Manager.Board);
            if (result == 1) Console.WriteLine("Tah je proti pravidlum");
            else if (result == 2) Console.WriteLine("Hrac neni na tahu");
            else if (result == 3)
            {
                Console.WriteLine("Konec hry vyhrava hrac {0}", Manager.Rules.WhoNotOnMove().GetName());
                konec = true;
                return;
            }
            else if (result == 4)
            {
                Console.WriteLine("Konec hry - hra konci remizou");
                konec = true;
                return;
            }
            ShowInfo();
        }
        //metoda obsluhujici prikaz show pro vypis moznych tahu pro danou pozici
        private void ShowMove(string[] parsed)
        {
            ShowChessboard(Manager.Board);
            int x,y;
            Char[] indexes = parsed[1].ToUpper().ToCharArray();
            {
                y = indexes[0];
                x = indexes[1];
                Manager.PrevodNaPole(ref x, ref y);
                foreach (Move m in Manager.Rules.GenerateMovesForPosition(x, y))
                {
                    Console.WriteLine(m.ToString());
                }
            }
            
            Console.WriteLine("");
            ShowInfo();
        }

        private void ShowAllMoves(Player player)
        {
            ShowChessboard(Manager.Board);
            List<Move> allMoves;

            if (Manager.P1.Equals(player)) allMoves = Manager.Rules.GenerateMoves(Manager.P1);
            else allMoves = Manager.Rules.GenerateMoves(Manager.P2);
            foreach (Move m in allMoves)
            {
                Console.WriteLine(m.ToString());
            }

            Console.WriteLine("");
            ShowInfo();
        }

        /*
        //metoda starajici se o start hry
        public void NewGame()
        {
            P1 = new Player();
            P2 = new Player();
            GetPlayersType(P1, P2);
            GetPlayersName(P1, P2);
            GetPlayersDifficulty(P1, P2);

            Player onMove = GetWhoFirst();
            Player notOnMove = P1;

            if (onMove.GetName().Equals(P1.GetName())) notOnMove = P2;

            Board = new Chessboard();
            Rules = new Rules(onMove, Board, notOnMove);
            Brain = new Brain(Rules, Board);
            GameManager = new GameManager(Board, Rules, Brain);
            ShowChessboard(Board);
            ShowInfo();
        }
         * */

        //metoda starajici se o start hry

        //pomocna metoda pro zadani jmen hracu
        public void GetPlayersType(Player p1, Player p2)
        {
            bool konec = false;
            string stringType;
            Regex regex = new Regex("[1-3]");

            Console.Clear();
            Console.WriteLine("Zvolte typ hry:");
            Console.WriteLine("1. PC vs Player");
            Console.WriteLine("2. Player vs Player");
            Console.WriteLine("3. PC vs PC");
            Console.Write(">> ");

            do
            {
                stringType = Console.ReadLine().ToUpper();
                if ((regex.IsMatch(stringType)) && (stringType.Length == 1))
                {
                    int type = Int32.Parse(stringType);

                    switch (type)
                    {
                        case 1 : 
                            {
                                p1.SetType(0).SetName("Computer"); 
                                p2.SetType(1);
                            }
                        break;
                        case 2 :
                            {
                                p1.SetType(1); 
                                p2.SetType(1);
                            }
                        break;
                        case 3 :
                            {
                                p1.SetType(0).SetName("Computer 1"); 
                                p2.SetType(0).SetName("Computer 2");
                            }
                        break;
                        default: Console.WriteLine("Zadali jste spatny prikaz");
                        break;
                        }
                        konec= true;
                  }
                else Console.WriteLine("Zadali jste spatny prikaz");
            } while (konec != true);
        }
        
        public void GetPlayersDifficulty(Player p1, Player p2)
        {
            int i = 0;
            bool konec = false;
            Regex regex = new Regex("[1-3]");
            Player[] players = {p1, p2};
            string strDifficulty;
            Console.Clear();
            Console.WriteLine("Obtiznosti: \n     1 - zacatecnik\n     2 - pohrocily\n     3 - profesional ");
            foreach (Player p in players)
            {
                if (p.isPC())
                {
                    do
                    {
                        i++;
                        Console.WriteLine("zadejte obliznost pocitacoveho hrace - {0}", p.GetName());
                        Console.Write(">> ");
                        strDifficulty = Console.ReadLine().ToUpper();
                        if ((regex.IsMatch(strDifficulty)) && (strDifficulty.Length == 1))
                        {
                            int difficulty = Int32.Parse(strDifficulty);
                            p.SetDifficulty(difficulty * 2);
                            konec = true;
                        }
                        else Console.WriteLine("Zadali jste spatny prikaz");
                    }
                    while (konec != true);
                }
                else p.SetDifficulty(6);
            }
        }

        public void GetPlayersName(Player p1, Player p2)
        {
            int i = 0;
            string name;
            bool konec = false;
            Player[] players = { p1, p2 };

            Console.Clear();
            foreach (Player p in players)
            {
                if (p.isPC()) continue;
                do
                {
                    i++;
                    Console.WriteLine("zadejte jmeno {0}.hrace", i);
                    name = Console.ReadLine();
                    if ((0 < name.Length) || (name.Length <= 15))
                    {
                        p.SetName(name);
                        konec = true;
                    }
                }
                while (konec != true);
            }
        }

        public Player GetWhoFirst()
        {
            bool g = false;
            string whofirst;
            Regex regex = new Regex("[1|2]");

            Console.Clear();
            Console.WriteLine("Zvolte hrace ktery zacne:");
            Console.WriteLine("1.{0}", Manager.P1.GetName());
            Console.WriteLine("2.{0}", Manager.P2.GetName());
            Console.Write(">> ");

            do
            {
                whofirst = Console.ReadLine().ToUpper();
                if ((regex.IsMatch(whofirst)) && (whofirst.Length == 1))
                {
                    if (Int32.Parse(whofirst) == 1) return Manager.P1;
                    g = true;
                }
                else Console.WriteLine("Zadali jste spatny prikaz");
            } while (g != true);
            return Manager.P2;
        }

        //graficke zobrazeni desky
        public void ShowChessboard(Chessboard Board)
        {
            Console.Clear();
            int[,] b = (Board.GetBoard());
            Console.WriteLine("                   {0} ", Manager.Rules.GetBlackPlayer().GetName());
            Console.WriteLine("\n\n  A   B   C   D   E   F   G   H   I   J");
            Console.WriteLine("\n|---|---|---|---|---|---|---|---|---|---|");
            for (int i = _TopBorder; i <= _BottomBorder; i++)
            {
                for (int j = _LefBorder; j <= _RightBorder; j++)
                {
                    if (b[i, j] == _WhitePawn) Console.Write("| a ");
                    else if (b[i, j] == _BlackPawn) Console.Write("| b ");
                    else if (b[i, j] == _WhiteChecker) Console.Write("| A ");
                    else if (b[i, j] == _BlackChecker) Console.Write("| B ");
                    else Console.Write("|   ");
                }
                Console.WriteLine("| {0}\n|---|---|---|---|---|---|---|---|---|---|", 9 - i);
            }
            Console.WriteLine("");
            Console.WriteLine("                   {0} ", Manager.Rules.GetWhitePlayer().GetName());
        }

        //metoda ktera slouzi k odeleni desky a zadavani prikazu
        public void ShowInfo()
        {
            Console.WriteLine("na tahu je Hrac {0}", Manager.Rules.WhoOnMove().GetName());
            Console.Write(">> ");
        }

        private void WarningMessage()
        {
            {
                ShowChessboard(Manager.Board);
                Console.WriteLine("");
                Console.WriteLine("Zadali jste spatny prikaz");
                Console.WriteLine("");
                ShowInfo();
            }
        }
    }
}

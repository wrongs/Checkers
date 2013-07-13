using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ročníkový_projekt_v_1._3
{
    class Brain
    {
        Rules Rules;
        Chessboard Board;
        private int _MAX = 3000;
        private int _Mnoho = 2500;
        private int[,] directions = { { -1, -1 }, { -2, 0 }, { -1, 1 }, { 0, -2 }, { 0, 2 }, { 1, -1 }, { 2, 0 }, { 1, 1 } };

        public Brain(Rules rules, Chessboard board)
        {
            Rules = rules;
            Board = board;
        }

        private int Farther (int value)
        {
            if (value > _Mnoho)
            {
                return value++;
            }
            if (value < -_Mnoho) 
            {
                return value--;
            }
            return value;
        }

        private int Closer (int value)
        {
            if (value > _Mnoho)
            {
                return value--;
            }
            if (value < -_Mnoho)
            {
                return value++;
            }
            return value;
        }

        private int Alfabeta(Player player, int depth, int alfa, int beta)
        {
            if (Rules.IsLoose(player)) return -_MAX;
            if (Rules.IsWin(player)) return _MAX;
            if (Rules.EndGame(player) == 2) return 0;
            if (depth == 0) return EvaluationBoard(Board, player);

            List<Move> moves = Rules.GenerateMoves(player);
            foreach (Move move in moves)
            {
                int tNoChange = 0, tLastSum = 0;
                SaveVariables(ref tNoChange, ref tLastSum);

                Rules.DoMoveInRules(move);
                int ohodnoceni = -Alfabeta(Rules.OppositePlayer(player), depth - 1, Farther(-beta), Farther(-alfa));
                
                Rules.DoInvMoveInRules(move);
                LoadVariables(tNoChange, tLastSum);

                ohodnoceni =  Closer(ohodnoceni);
                if (ohodnoceni > alfa)
                {
                    alfa = ohodnoceni;
                    if (ohodnoceni >= beta) return beta;
                }
            }
            return alfa;
        }

        //generování nejlepšího tahu
        public Move GenerateBestMove(Player player)
        {
            List<Move> moves = Rules.GenerateMoves(player);
            int alfa = -_MAX;
            Move BestMove = moves.First();
            foreach (Move m in moves)
            {
                int tNoChange= 0, tLastSum = 0;
                SaveVariables(ref tNoChange, ref tLastSum);
                
                Rules.DoMoveInRules(m);
                int ohodnoceni = -Alfabeta(Rules.OppositePlayer(player), player.GetDiffuculty(), -_MAX, Farther(-alfa));
                
                Rules.DoInvMoveInRules(m);
                LoadVariables(tNoChange, tLastSum);

                ohodnoceni = Closer(ohodnoceni);
                if (ohodnoceni > alfa)
                {
                    alfa = ohodnoceni;
                    BestMove = m;
                }
            }
            return BestMove;
        }

        private int EvaluationBoard(Chessboard board, Player player)
        {
            int sum = 0;
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    int value = board.GetValueOnPosition(i, j);
                    sum += value;
                    if (value != 0)
                    {
                        if (Rules.PlayerFigurine(Rules.GetWhitePlayer(), value))
                        {
                            if (i == 9) sum += 3;
                            else if ((i == 0) && (value == 10)) sum += 2;
                            else if ((j == 0) || (j == 9)) sum += 2;
                            for (int k = 0; k < 8; k++)
                            {
                                int x = i + directions[k, 0];
                                int y = j + directions[k, 1];
                                if (Rules.PlayerFigurine(Rules.GetWhitePlayer(), board.GetValueOnPosition(x, y))) sum += 1; 
                            }
                        }
                        else
                        {
                            if (i == 0) sum -= 3;
                            else if ((i == 9) && (value == -10)) sum -= 2;
                            else if ((j == 0) || (j == 9)) sum -= 2;

                            for (int k = 0; k < 8; k++)
                            {
                                int x = i + directions[k, 0];
                                int y = j + directions[k, 1];
                                if (Rules.PlayerFigurine(Rules.GetBlackPlayer(),board.GetValueOnPosition(x, y))) sum -= 1; 
                            }
                        }
                    }
                }
            }
            if(Rules.IsWhitePlayer(player)) return sum;
            return -sum;
        }

        private void SaveVariables(ref int variable1, ref int variable2)
        {
            variable1 = Rules.GetNotJumpMoves();
            variable2 = Rules.GetFigurinesCount();
        }

        private void LoadVariables(int variable1, int variable2)
        {
            Rules.SetNoJumpMoves(variable1);
            Rules.SetFigurinesCount(variable2);
        }


    }
}

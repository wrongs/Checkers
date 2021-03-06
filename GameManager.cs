﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ročníkový_projekt_v_1._3
{
    class GameManager
    {
        private Chessboard Board;
        private Rules Rules;
        private Brain Brain;

        //Trida ktera slouzi jako rozhrani meyi logikou a UI
        public GameManager(Chessboard board, Rules rules, Brain brain)
        {
            this.Board = board;
            this.Rules = rules;
            this.Brain = brain;
        }

        //Metoda ktera provadi kontrolu spravnosti tahu 
        public int DoPlayerMove(string[] parsed)
        {
            Move tah = new Move();
            if (!(GeneratePlayerMove(parsed, tah))) return 2;

             if (Rules.IsInRules(tah))
            {
                Board.DoMove(tah);
                Board.DoRemove(tah);
                Rules.Promote(tah);
                Rules.ChangeOnMove();
                int result = Rules.EndGame(Rules.OnMovePlayer());
                if (result == 1) return 3;
                if (result == 2) return 4;
                return 0;
            }
            return 1;
        }


        //metoda generujici tah hrace ze zadanych souradnic
        private bool GeneratePlayerMove(string[] parsed, Move move)
        {  

            Char[] from = parsed[0].ToUpper().ToCharArray();
            Char[] to = parsed[1].ToUpper().ToCharArray();
            int x1, y1, x2, y2, originalValue;
            List<int[]> available = new List<int[]>();

            for (int i = 0; i < parsed.Length; i = i + 2)
            {
                from = parsed[i].ToUpper().ToCharArray();
                to = parsed[i + 1].ToUpper().ToCharArray();
                x1 = from[1];
                y1 = from[0];
                x2 = to[1];
                y2 = to[0];
                PrevodNaPole(ref x1, ref y1);
                PrevodNaPole(ref x2, ref y2);
                available.Add(new int[] { x1, x2 });

                int[] direction = ComputeDirection(x1, y1, x2, y2);
                int x3 = x1;
                int y3 = y1;
                int jumpedValue = 0;
                do
                {
                    x3 = x3 + direction[0];
                    y3 = y3 + direction[1];
                    int value = Board.GetValueOnPosition(x3, y3);
                    if (0 != value)
                    {
                        jumpedValue = value;
                        break;
                    }
                }
                while (!((x3 == x2) && (y3 == y2)));

                if (i == 0)
                {    
                    originalValue = Board.GetValueOnPosition(x1, y1);
                    if (!(Rules.OnMoveStone(originalValue))) return false;
                    move.SetBefore(originalValue);
                }

                if (jumpedValue == 0) move.AddShift(x1, y1, x2, y2);
                else move.AddShift(x1, y1, x3, y3, x2, y2, jumpedValue);

                if ((i + 1) > parsed.Length)
                {
                    move.SetAfter(Board.GetValueOnPosition(x2, y2));
                    foreach (int[] array in available)
                    {
                        if ((array[0] == x2) && (array[1] == y2)) move.SetAfter(0);
                    }
                }
            }
            return true;
        }

        // metoda která provadí tah počítačem
        public int DoComputerMove()
        {
            Move tah = Brain.GenerateBestMove(Rules.OnMovePlayer());
            Board.DoMove(tah);
            Board.DoRemove(tah);
            Rules.Promote(tah);
            Rules.ChangeOnMove();
            int result = Rules.EndGame(Rules.OnMovePlayer());
            if (result == 1) return 3;
            if (result == 2) return 4;
            return 0;
        }

        //pomocna metoda pro prevod souradnic
        public void PrevodNaPole(ref int x, ref int y)
        {
            x = Math.Abs((x - '0') - 9);
            y = y - 17 - '0';
        }

        //pomocna metoda pro vypocet smeru pohybu
        private int[] ComputeDirection(int x1, int y1, int x2, int y2)
        {
            int d1 = x2 - x1;
            int d2 = y2 - y1;
            int[] direction = new int[2];
            if (d1 != 0) direction[0] = d1 / Math.Abs(d1);
            if (d2 != 0) direction[1] = d2 / Math.Abs(d2);
            return direction;
        }
    }
}

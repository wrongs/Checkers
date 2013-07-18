using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Ročníkový_projekt_v_1._3
{
    [Serializable]
    public class Chessboard
    {
        private const int _WhitePawn = 25;
        private const int _BlackPawn = -25;
        private const int _TopBorder = 0;
        private const int _BottomBorder = 9;
        private const int _LefBorder = 0;
        private const int _RightBorder = 9;

        private int[,] Board = new int[10, 10];

        private Stack<Move> UndoStack;
        private Stack<Move> RedoStack;


        public Chessboard()
        {
            FillBoard();
            UndoStack = new Stack<Move>();
            RedoStack = new Stack<Move>();
        }
        
        public int[,] GetBoard()
        {
            return Board;
        }
        
        //Fukce ktera naplni hraci desku na zakladní rozestavení podle pravidel friske damy
        private void FillBoard()
        {
            for (int i = _TopBorder; i <= _BottomBorder; i++)
                for (int j = _LefBorder; j <= _RightBorder; j++)
                {
                    if (i < 4)
                    {
                        if (((i % 2 == 0) & (j % 2 == 1)) | (i % 2 == 1) & (j % 2 == 0)) Board[i, j] = _BlackPawn;
                    }
                    if (i > 5)
                    {
                        if (((i % 2 == 0) & (j % 2 == 1)) | (i % 2 == 1) & (j % 2 == 0)) Board[i, j] = _WhitePawn;
                    }
                }
        }
          
       // Funkce provádí zmenu obsahu hraci desky podle daného tahu
        public void DoMove(Move t)
        {
            foreach (Shift s in t.GetShifts())
            {
                Board[s.X1, s.Y1] = 0; 
                Board[s.X3, s.Y3] = t.GetBefore();
            }
            UndoStack.Push(t);
            RedoStack.Clear();
        }

        // provadi odstraneni kamenu z desky
        public void DoRemove(Move t)
        {
            foreach (Shift s in t.GetShifts())
            {
                if (s.Jumped != 0) Board[s.X2, s.Y2] = 0;
            }
        }
 
        // provadeni inverzniho tahu
        public void DoInvMove(Move t)
        {
            List<Shift> reversed = t.GetShifts().GetRange(0, t.GetShifts().Count());
            reversed.Reverse();
            foreach (Shift s in reversed)
            {
                Board[s.X1, s.Y1] = t.GetBefore();
                if (s.Jumped != 0) Board[s.X2, s.Y2] = s.Jumped;
                Board[s.X3, s.Y3] = t.GetAfter();
            }
        }

        // metoda pro krok zpet ve hre
        public void Undo()
        {
            Move move = UndoStack.Pop();
            DoInvMove(move);
            RedoStack.Push(move);
        }

        public void Redo()
        {
            Move move = RedoStack.Pop();
            DoMove(move);
            UndoStack.Push(move);
        }

        // metoda vrací hodnotu daného policka desky
        public int GetValueOnPosition(int x, int y)
        {
            if (IsInBoard(x, y)) return Board[x, y];
            return -1;
        }

        //metoda na zjisteni jestli je dane policko prazdne
        public bool IsEmpty(int x, int y)
        {
            if (IsInBoard(x, y) && (Board[x, y] == 0)) return true;
            return false;
        }

        //medoa zjistujici zda je dane policko na desce
        public bool IsInBoard(int x, int y)
        {
            if ((x <= _BottomBorder) && (_TopBorder <= x) && (y <= _RightBorder) && (_LefBorder <= y)) return true;
            return false;
        }

        // metoda nastavujici hodnotu policka 
        public Chessboard SetValueOnPosition(int x, int y, int value)
        {
            Board[x, y] = value;
            return this;
        }

        // metoda která porovnává hodnotu policka x,y s hodnotou value 
        public bool equalValueOnPosition(int value, int x, int y)
        {
            if (value == GetValueOnPosition(x, y)) return true;
            return false;
        }
    }
}

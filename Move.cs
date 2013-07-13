using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

//struktura 
public struct Shift : IEquatable<Shift>
{
    public int X1, Y1, X3, Y3, X2, Y2, Jumped;

    public Shift(int x1, int y1, int x2, int y2, int x3, int y3, int jumped) 
    {
        X1 = x1;
        Y1 = y1;
        X2 = x2;
        Y2 = y2;
        X3 = x3;
        Y3 = y3;
        Jumped = jumped; 
    }

    public Shift(int x1, int y1, int x3, int y3)
    {
        X1 = x1;
        Y1 = y1;
        X2 = -1;
        Y2 = -1;
        X3 = x3;
        Y3 = y3;
        Jumped = 0;
    }

    public override string ToString()
    {
        string[] alphabet = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J" };
        StringBuilder builder = new StringBuilder().Append("(" + alphabet[Y1] + (9 -X1) + ", ");
        if (Jumped != 0) builder.Append(alphabet[Y2] + (9 - X2) + " [" + Jumped + "], ");
        builder.Append(alphabet[Y3] + (9 - X3) + ")");
        return builder.ToString();
    }

    public bool Equals(Shift s)
    {
        if (X1 != s.X1) return false;
        if (Y1 != s.Y1) return false;
        if (X2 != s.X2) return false;
        if (Y2 != s.Y2) return false;
        if (X3 != s.X3) return false;
        if (Y3 != s.Y3) return false;
        if (Jumped != s.Jumped) return false;
        return true;
    } 
 }

namespace Ročníkový_projekt_v_1._3
{
    class Move : IEquatable<Move>
    {
        private List<Shift> Shifts; //{ get; set; }
        private int Before, After;
        private const int _WhitePawn = 25;
        private const int _WhiteChecker = 100;

        public Move()
        {
            Shifts = new List<Shift>();
        }

        public Move(int x1, int y1, int x3, int y3, int after, int before)
        {
            Shifts = new List<Shift>();
            Shifts.Add(new Shift(x1, y1, x3, y3));
            Before = before;
            After = after;
        }

        public Move(int x1, int y1, int x2, int y2, int x3, int y3, int after, int jumped, int before)
        {
            Shifts = new List<Shift>();
            Shifts.Add(new Shift(x1, y1, x2, y2, x3, y3, jumped));
            Before = before;
            After = after;
        }

        public Move SetBefore(int before)
        {
            Before = before;
            return this;
        }

        public Move SetAfter(int after)
        {
            After = after;
            return this;
        }

        public int GetBefore()
        {
            return Before;
        }

        public int GetAfter()
        {
            return After;
        }

        public Move GetShifts(List<Shift> shifts)
        {
            Shifts = shifts;
            return this;
        }

        public List<Shift> GetShifts()
        {
            return Shifts;
        }

        // prida novy skok do seznamu pohybu
        public void AddShift(int x1, int y1, int x2, int y2, int x3, int y3,int jumped)
        {
            Shifts.Add(new Shift(x1, y1, x2, y2, x3, y3, jumped));
        }

        // prida novy pohyb do seznamu pohybu
        public void AddShift(int x1, int y1, int x3, int y3)
        {
            Shifts.Add(new Shift(x1, y1, x3, y3));
        }

        //udela kopii aktualniho tahu a vrati jej jako vysledek
        public Move CopyMove()
        {
            Move m = new Move();
            foreach (Shift s in Shifts)
            {
                m.AddShift(s.X1, s.Y1, s.X2, s.Y2, s.X3, s.Y3, s.Jumped);
            }
            m.SetBefore(GetBefore());
            m.SetAfter(GetAfter());
            return m;
        }

        //metoda pocitajíci hodnotu tahu která se používa při vyberu skoku s povinosti skakat figurky s co nejvyssi hodnotou  
        public double JumpSum()
        {
            double actualSum = 0;
            foreach (Shift s in GetShifts())
            {
                if (Math.Abs(s.Jumped) == _WhitePawn) actualSum += 1;
                else if (Math.Abs(s.Jumped) == _WhitePawn) actualSum += 1.5;
            }
            return actualSum;
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder().Append("(");

            foreach (Shift s in Shifts)
            {
                builder.Append(s.ToString());
                if (Shifts.Last().Equals(s)) continue;
                builder.Append(",");
            }
            builder.Append(" before: " + GetBefore() + ",");
            builder.Append(" after: " + GetAfter());
            builder.Append(")");
            return builder.ToString();
        }

        public bool Equals(Move m)
        {
            if (GetShifts().Count == m.GetShifts().Count())
            {
                int i = 0;
                foreach (Shift s in GetShifts())
                {
                     if(m.GetShifts()[i].Equals(s)) 
                     {
                         i++;
                         continue;
                         }
                     return false;
                }
            }
            else return false;
            if (m.GetBefore() != GetBefore()) return false;
            if (m.GetAfter() != GetAfter()) return false;

            return true;       
        }
 
    }
}

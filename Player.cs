using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ročníkový_projekt_v_1._3
{
    public class Player : IEquatable<Player>
    {
        string Name;
        int Type;
        int Difficulty;

        public Player(){}

        public Player(string name, int type, int difficulty)
        {
            Name = name;
            Type = type;
            Difficulty = difficulty;
        }

        public Player SetName(string name)
        {
            Name = name;
            return this;
        }

        public string GetName()
        {
            return Name;
        }

        public Player SetType(int type)
        {
            Type = type;
            return this;
        }

        public int GetPlayerType()
        {
            return Type;
        }

        public Player SetDifficulty(int difficulty)
        {
            Difficulty = difficulty;
            return this;
        }

        public int GetDiffuculty()
        {
            return Difficulty;
        }

        public bool isPC()
        {
            if (GetPlayerType() == 0) return true;
            return false;
        }

        public bool isHuman()
        {
            if (GetPlayerType() == 0) return false;
            return true;
        }

        public bool Equals(Player player)
        {
            if (GetName() != player.GetName()) return false;
            if (GetPlayerType() != player.GetPlayerType()) return false;
            if (GetDiffuculty() != player.GetDiffuculty()) return false;
            return true;
        }
    }
}

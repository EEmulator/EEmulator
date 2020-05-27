using System;
using EverybodyEdits.Game.CountWorld;

namespace EverybodyEdits.Game
{
    public struct Item : IEquatable<Item>
    {
        public bool Equals(Item other)
        {
            return this.Block.Equals(other.Block) && this.X == other.X && this.Y == other.Y;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            return obj is Item && Equals((Item) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = this.Block.GetHashCode();
                hashCode = (hashCode*397) ^ (int) this.X;
                hashCode = (hashCode*397) ^ (int) this.Y;
                return hashCode;
            }
        }

        public static bool operator ==(Item left, Item right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Item left, Item right)
        {
            return !left.Equals(right);
        }

        public readonly ForegroundBlock Block;
        public readonly uint X;
        public readonly uint Y;

        public Item(ForegroundBlock block, uint x, uint y)
        {
            this.Block = block;
            this.X = x;
            this.Y = y;
        }
    }
}
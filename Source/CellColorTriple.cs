
namespace CellularAutomata
{
    using System;

    /// <summary>
    /// A triplet of cells.
    /// </summary>
    public struct CellColorTriple : IEquatable<CellColorTriple>
    {
        /// <summary>
        /// The left cell.
        /// </summary>
        public readonly CellColor First;

        /// <summary>
        /// The midle cell.
        /// </summary>
        public readonly CellColor Second;

        /// <summary>
        /// The right cell.
        /// </summary>
        public readonly CellColor Third;

        public CellColorTriple( CellColor first, CellColor second, CellColor third )
        {
            this.First = first;
            this.Second = second;
            this.Third = third;
        }

        public override bool Equals( object obj )
        {
            if( obj is CellColorTriple )
            {
                return base.Equals( (CellColorTriple)obj );
            }

            return false;
        }

        public bool Equals( CellColorTriple other )
        {
            return this.First == other.First &&
                   this.Second == other.Second &&
                   this.Third == other.Third;
        }

        public override int GetHashCode()
        {
            return this.First.GetHashCode() ^ this.Second.GetHashCode() ^ this.Third.GetHashCode();
        }
    }
}

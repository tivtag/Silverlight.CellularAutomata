// <copyright file="GameLoop.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the CellularAutomata.CellPatternTable class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace CellularAutomata
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;

    /// <summary>
    /// Represents the configuration of the Cellular Automata; e.g. how the state of a new cell is generated based on the state of the previous generation.
    /// See MainPage.xaml.cs for a more complete description.
    /// </summary>
    public sealed class CellPatternTable : INotifyPropertyChanged
    {
        /// <summary>
        /// Called when a property of this CellPatternTable has changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets or sets the value the pattern '1 1 1' maps onto.
        /// </summary>
        public bool Entry111
        {
            get
            {
                return this.MapBool( true, true, true );
            }

            set
            {
                this.Set( true, true, true, value );
                this.NotifyPropertyChanged( "Entry111" );
            }
        }

        /// <summary>
        /// Gets or sets the value the pattern '1 1 0' maps onto.
        /// </summary>
        public bool Entry110
        {
            get
            {
                return this.MapBool( true, true, false );
            }

            set
            {
                this.Set( true, true, false, value );
                this.NotifyPropertyChanged( "Entry110" );
            }
        }

        /// <summary>
        /// Gets or sets the value the pattern '1 0 1' maps onto.
        /// </summary>
        public bool Entry101
        {
            get
            {
                return this.MapBool( true, false, true );
            }

            set
            {
                this.Set( true, false, true, value );
                this.NotifyPropertyChanged( "Entry101" );
            }
        }

        /// <summary>
        /// Gets or sets the value the pattern '1 0 0' maps onto.
        /// </summary>
        public bool Entry100
        {
            get
            {
                return this.MapBool( true, false, false );
            }

            set
            {
                this.Set( true, false, false, value );
                this.NotifyPropertyChanged( "Entry100" );
            }
        }

        /// <summary>
        /// Gets or sets the value the pattern '0 1 1' maps onto.
        /// </summary>
        public bool Entry011
        {
            get
            {
                return this.MapBool( false, true, true );
            }

            set
            {
                this.Set( false, true, true, value );
                this.NotifyPropertyChanged( "Entry011" );
            }
        }

        /// <summary>
        /// Gets or sets the value the pattern '0 1 0' maps onto.
        /// </summary>
        public bool Entry010
        {
            get
            {
                return this.MapBool( false, true, false );
            }

            set
            {
                this.Set( false, true, false, value );
                this.NotifyPropertyChanged( "Entry010" );
            }
        }

        /// <summary>
        /// Gets or sets the value the pattern '0 0 1' maps onto.
        /// </summary>
        public bool Entry001
        {
            get
            {
                return this.MapBool( false, false, true );
            }

            set
            {
                this.Set( false, false, true, value );
                this.NotifyPropertyChanged( "Entry001" );
            }
        }

        /// <summary>
        /// Gets or sets the value the pattern '0 0 0' maps onto.
        /// </summary>
        public bool Entry000
        {
            get
            {
                return this.MapBool( false, false, false );
            }

            set
            {
                this.Set( false, false, false, value );
                this.NotifyPropertyChanged( "Entry000" );
            }
        }

        /// <summary>
        /// Initializes a new instance of the CellPatternTable class.
        /// </summary>
        public CellPatternTable()
        {
            this.colorMap.Add( new CellColorTriple( CellColor.Black, CellColor.Black, CellColor.Black ), CellColor.Black );
            this.colorMap.Add( new CellColorTriple( CellColor.Black, CellColor.Black, CellColor.White ), CellColor.White );
            this.colorMap.Add( new CellColorTriple( CellColor.Black, CellColor.White, CellColor.Black ), CellColor.White );
            
            this.colorMap.Add( new CellColorTriple( CellColor.Black, CellColor.White, CellColor.White ), CellColor.Black );
            this.colorMap.Add( new CellColorTriple( CellColor.White, CellColor.Black, CellColor.Black ), CellColor.White );
            this.colorMap.Add( new CellColorTriple( CellColor.White, CellColor.Black, CellColor.White ), CellColor.Black );
            
            this.colorMap.Add( new CellColorTriple( CellColor.White, CellColor.White, CellColor.Black ), CellColor.White );
            this.colorMap.Add( new CellColorTriple( CellColor.White, CellColor.White, CellColor.White ), CellColor.Black );
        }

        /// <summary>
        /// Maps the given CellColorTriple onto a single CellColor using the lookup table.
        /// </summary>
        /// <param name="triple">
        /// The input triple.
        /// </param>
        /// <returns>
        /// The mapped color.
        /// </returns>
        public CellColor Map( CellColorTriple triple )
        {
            return colorMap[triple];
        }

        /// <summary>
        /// Maps the given CellColorTriple onto a single value using the lookup table.
        /// </summary>
        /// <param name="triple">
        /// The input triple.
        /// </param>
        /// <returns>
        /// The mapped value.
        /// </returns>
        private bool MapBool( CellColorTriple triple )
        {
            return this.colorMap[triple] == CellColor.Black;
        }

        /// <summary>
        /// Maps the given CellColorTriple onto a single value using the lookup table.
        /// </summary>
        /// <param name="first">
        /// The first element of the input triple.
        /// </param>
        /// <param name="second">
        /// The second element of the input triple.
        /// </param>
        /// <param name="third">
        /// The third element of the input triple.
        /// </param>
        /// <returns>
        /// The mapped value.
        /// </returns>
        private bool MapBool( bool first, bool second, bool third )
        {
            var triple = new CellColorTriple(
                first  ? CellColor.Black : CellColor.White,
                second ? CellColor.Black : CellColor.White,
                third  ? CellColor.Black : CellColor.White
            );

            return this.colorMap[triple] == CellColor.Black;
        }

        /// <summary>
        /// Gets the value the given CellColorTriple maps onto.
        /// </summary>
        /// <param name="first">
        /// The first element of the input triple.
        /// </param>
        /// <param name="second">
        /// The second element of the input triple.
        /// </param>
        /// <param name="third">
        /// The third element of the input triple.
        /// </param>
        /// <value>The value to map the given input triple onto.</value>
        private void Set( bool first, bool second, bool third, bool value )
        {
            var triple = new CellColorTriple(
                first  ? CellColor.Black : CellColor.White,
                second ? CellColor.Black : CellColor.White,
                third  ? CellColor.Black : CellColor.White
            );

            this.colorMap[triple] = value ? CellColor.Black : CellColor.White;
        }

        /// <summary>
        /// Fires the PropertyChanged event.
        /// </summary>
        /// <param name="propertyName">
        /// The name of the property that has changed.
        /// </param>
        private void NotifyPropertyChanged( string propertyName )
        {
            if( this.PropertyChanged != null )
            {
                this.PropertyChanged( this, new PropertyChangedEventArgs( propertyName ) );
            }
        }

        /// <summary>
        /// Stores the internal table.
        /// </summary>
        private readonly Dictionary<CellColorTriple, CellColor> colorMap = new Dictionary<CellColorTriple, CellColor>();
    }
}

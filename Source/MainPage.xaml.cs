// <copyright file="MainPage.xaml.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>Defines the CellularAutomata.MainPage class.</summary>
// <author>Paul Ennemoser (Tick)</author>

namespace CellularAutomata
{
    using System;
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Shapes;
    using System.Threading;

    /// <summary>
    /// A cellular automata consists of a grid of cells, with each cell having a number of states.
    /// The cells of this simple automata can have one of two states: On (Black) or Off (White).
    /// <para>
    /// Each row in the grid represents one generation(in this automata).
    /// The state of a row depends on the state of the previous row, and so on.
    /// </para>
    /// <para>
    /// A simple rule is used to generate the state of each cell of a row:
    /// For each entry in the row we take the(N – 1th, Nth, N + 1th) cell triple in the previous row.
    /// Now we take this triple and use a lookup table to get the state of the entry.
    /// </para>
    /// <para>
    /// Here are the lookup tables for two of the most famous patterns:
    /// </para>
    /// <para>
    /// Rule 30 cellular automaton lookup table:
    /// current pattern            111 110 101 100 011 010 001 000
    /// new state for center cell   0	0	0	1	1	1	1	0
    /// </para>
    /// <para>
    /// Rule 110 cellular automaton lookup table:
    /// current pattern            111 110 101 100 011 010 001 000
    /// new state for center cell   0	1	1	0	1	1	1	0
    /// </para>
    /// </summary>
    public sealed partial class MainPage : UserControl, INotifyPropertyChanged
    {
        #region [ Events ]

        /// <summary>
        /// Called when a property of this Page has changed.
        /// </summary>
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Gets or sets the speed the simulation is running at.
        /// </summary>
        public double SimulationSpeed
        {
            get
            {
                return tickTime.Milliseconds / 1000.0;
            }

            set
            {
                tickTime = TimeSpan.FromMilliseconds( value * 1000.0 );

                if( PropertyChanged != null )
                    PropertyChanged( this, new PropertyChangedEventArgs( "SimulationSpeed" ) );
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the simulation
        /// is currently running.
        /// </summary>
        public bool IsRunning
        {
            get
            {
                return isRunning;
            }
        
            set
            {
                isRunning = value;

                if( value )
                {
                    buttonStart.IsEnabled = false;
                    buttonPause.IsEnabled = true;
                }
                else
                {
                    buttonStart.IsEnabled = true;
                    buttonPause.IsEnabled = false;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the simulation
        /// is currently paused.
        /// </summary>
        public bool IsPaused
        {
            get
            {
                return isPaused;
            }

            set
            {
                isPaused = value;

                if( value )
                {
                    buttonStart.IsEnabled = true;
                    buttonPause.IsEnabled = false;
                }
                else
                {
                    buttonPause.IsEnabled = true;
                }
            }
        }

        #endregion

        #region [ Initialization ]

        /// <summary>
        /// Initializes a new instance of the MainPage class.
        /// </summary>
        public MainPage()
        {
            this.InitializeComponent();
            this.CreateField( 45 * 16, 45 * 16, 8 );

            this.patternTableControl.DataContext = this.patternTable;

            this.gameLoop = new GameLoop( "Main Loop" );
            this.gameLoop.Update += OnSimulationUpdate;
            this.gameLoop.Attach( this );

            this.Reset();
        }

        /// <summary>
        /// Creates the ant field on which the simulation runs on.
        /// </summary>
        /// <param name="fieldWidth">The width of the field (in tile-space).</param>
        /// <param name="fieldHeight">The height of the field (in tile-space).</param>
        /// <param name="cellSize">The size of a single cell.</param>
        private void CreateField( int fieldWidth, int fieldHeight, int cellSize )
        {
            cellGrid.Children.Clear();
            cellGrid.RowDefinitions.Clear();
            cellGrid.ColumnDefinitions.Clear();

            // Create rows.
            this.rows = fieldWidth / cellSize;
            for( int i = 0; i < rows; ++i )
            {
                cellGrid.RowDefinitions.Add( new RowDefinition() { Height = new GridLength( cellSize ) } );
            }

            // Create columns.
            this.columns = fieldHeight / cellSize;
            for( int i = 0; i < columns; ++i )
            {
                cellGrid.ColumnDefinitions.Add( new ColumnDefinition() { Width = new GridLength( cellSize ) } ); 
            }

            // Create field.
            this.cellField = new Rectangle[rows, columns];

            // And finally.. create the cells :)
            for( int row = 0; row < rows; ++row )
            {
                for( int column = 0; column < columns; ++column )
                {
                    Rectangle cell = new Rectangle() {
                        Fill = brushA
                    };

                    Grid.SetRow( cell, row );
                    Grid.SetColumn( cell, column );

                    cellField[column, row] = cell;
                    cellGrid.Children.Add( cell );
                }
            }

            this.RandomlyFillFirstRow();
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Updates the Ant simulation.
        /// </summary>
        /// <param name="elapsedTime">
        /// The time the last frame took to execute.
        /// </param>
        private void OnSimulationUpdate( TimeSpan elapsedTime )
        {
            if( !this.IsRunning || this.IsPaused )
            {
                return;
            }

            tickTimeLeft -= elapsedTime;

            if( tickTimeLeft <= TimeSpan.Zero )
            {
                SimulateStep();
                tickTimeLeft = tickTime;
            }            
        }

        /// <summary>
        /// Simulates the next step of the autmomata.
        /// </summary>
        private void SimulateStep()
        {
            int previousRowIndex = nextRowIndex - 1;

            for( int columnIndex = 0; columnIndex < this.columns; ++columnIndex )
            {
                SimulateCell( previousRowIndex, columnIndex );
            }

            this.MoveToNextRow();

            ++step;
            textBoxStep.Text = "Step: " + step.ToString();
        }

        private void SimulateCell( int previousRowIndex, int columnIndex )
        {
            // The index wraps over in a circular fashion
            int indexA = (columnIndex == 0) ? this.columns - 1 : (columnIndex - 1);
            int indexB = (columnIndex + 0) % this.columns;
            int indexC = (columnIndex + 1) % this.columns;

            Rectangle cellA = cellField[indexA, previousRowIndex];
            Rectangle cellB = cellField[indexB, previousRowIndex];
            Rectangle cellC = cellField[indexC, previousRowIndex];

            Rectangle next = cellField[columnIndex, this.nextRowIndex];
            next.Fill = GetCellFill( cellA.Fill, cellB.Fill, cellC.Fill );
        }

        private void MoveToNextRow()
        {
            ++this.nextRowIndex;

            if( this.nextRowIndex == this.rows )
            {
                // Move everything up by one row.
                for( int row = 1; row < rows; ++row )
                {
                    int previousRow = row - 1;

                    for( int column = 0; column < columns; ++column )
                    {
                        Rectangle lowerRect = cellField[column, row];
                        Rectangle upperRect = cellField[column, previousRow];

                        if( upperRect.Fill != lowerRect.Fill )
                        {
                            upperRect.Fill = lowerRect.Fill;
                        }
                    }
                }

                --this.nextRowIndex;
            }
        }

        private Brush GetCellFill( Brush brushA, Brush brushB, Brush brushC )
        {
            var triple = new CellColorTriple(
                ConvertToColor( brushA ),
                ConvertToColor( brushB ),
                ConvertToColor( brushC )
            );

            return GetCellColor( triple );
        }

        private CellColor ConvertToColor( Brush brush )
        {
            if( brush == brushA )
            {
                return CellColor.White;
            }
            else
            {
                return CellColor.Black;
            }
        }

        private Brush GetCellColor( CellColorTriple triple )
        {
            CellColor color = this.patternTable.Map( triple );

            if( color == CellColor.White )
            {
                return brushA;
            }
            else
            {
                return brushB;
            }
        }

        /// <summary>
        /// Resets the Cellular Automata.
        /// </summary>
        private void Reset()
        {

            this.IsPaused  = false;
            this.IsRunning = false;

            foreach( Rectangle cell in this.cellField )
            {
                cell.Fill = brushA;
            }

            this.RandomlyFillFirstRow();

            nextRowIndex = 1;
            textBoxStep.Text = string.Empty;
        }

        #region - Events -

        /// <summary>
        /// Called when the user clicks on the Start button.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The RoutedEventArgs that contain the event data.</param>
        private void OnStartButtonClicked( object sender, RoutedEventArgs e )
        {
            if( !this.IsPaused )
            {
                tickTimeLeft = tickTime;
                step = 0;
            }

            this.IsPaused  = false;
            this.IsRunning = true;
        }

        /// <summary>
        /// Called when the user clicks on the Pause button.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The RoutedEventArgs that contain the event data.</param>
        private void OnPauseButtonClicked( object sender, RoutedEventArgs e )
        {
            this.IsPaused = true;
        }

        /// <summary>
        /// Called when the user clicks on the Reset button.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The RoutedEventArgs that contain the event data.</param>
        private void OnResetButtonClicked( object sender, RoutedEventArgs e )
        {
            this.Reset();
        }

        /// <summary>
        /// Randomly fills the first row of the cell field.
        /// </summary>
        private void RandomlyFillFirstRow()
        {
            for( int x = 0; x < this.rows; ++x )
            {
                int value = random.Next( 0, 2 );

                this.cellField[x, 0].Fill = (value == 0) ? brushA : brushB;
            }
        }

        /// <summary>
        /// Gets called when the value of the speed slider control has changed.
        /// </summary>
        /// <param name="sender">The sender fo the event.</param>
        /// <param name="e">The RoutedPropertyChangedEventArgs{Double} that contain the event data.</param>
        private void OnSpeedSliderValueChanged( object sender, RoutedPropertyChangedEventArgs<double> e )
        {
            // Workaround, Binding is still somewhat bugged in SL B2
            this.SimulationSpeed = sliderSpeed.Value;
        }

        #endregion

        #endregion

        #region [ Fields ]

        /// <summary>
        /// The zero-based index of the current row in-progress.
        /// </summary>
        private int nextRowIndex;

        /// <summary>
        /// The table that maps an input triple onto a single value.
        /// </summary>
        private readonly CellPatternTable patternTable = new CellPatternTable();
     
        #region - Cell Field -

        /// <summary>
        /// The number of rows the cell field has.
        /// </summary>
        private int rows;

        /// <summary>
        /// The number of columns the cell field has.
        /// </summary>
        private int columns;

        /// <summary>
        /// The cellular field.
        /// </summary>
        private Rectangle[,] cellField;

        #endregion

        #region - Simulation -

        /// <summary>
        /// States whether the simulation is currently running.
        /// </summary>
        private bool isRunning;

        /// <summary>
        /// States whether a running simulation has been paused by the user.
        /// </summary>
        private bool isPaused;

        /// <summary>
        /// The time between two simulation steps.
        /// </summary>
        private TimeSpan tickTime;

        /// <summary>
        /// The time left until the next simulation step is done.
        /// </summary>
        private TimeSpan tickTimeLeft;

        /// <summary>
        /// The number of steps since the start of the simulation.
        /// </summary>
        private int step;

        #endregion

        #region - Other -

        /// <summary>
        /// A random number generator.
        /// </summary>
        private readonly Random random = new Random();

        /// <summary>
        /// The GameLoop that runs the simulation.
        /// </summary>
        private readonly GameLoop gameLoop;

        #endregion

        #endregion
    }
}

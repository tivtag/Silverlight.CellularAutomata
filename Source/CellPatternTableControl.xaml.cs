// <copyright file="CellPatternTableControl.xaml.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the CellularAutomata.CellPatternTableControl class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace CellularAutomata
{
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// Provides a view over a <see cref="CellPatternTable"/>.
    /// </summary>
    public sealed partial class CellPatternTableControl : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the CellPatternTableControl class.
        /// </summary>
        public CellPatternTableControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Called when the user clicks the Rule 30 button,
        /// switching the pattern over to Rule 30.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The RoutedEventArgs that contains the event data.</param>
        private void OnRule30ButtonClicked( object sender, RoutedEventArgs e )
        {
            var patternTable = this.DataContext as CellPatternTable;
            if( patternTable == null )
            {
                return;
            }

            patternTable.Entry111 = false;
            patternTable.Entry110 = false;
            patternTable.Entry101 = false;
            patternTable.Entry100 = true;
            patternTable.Entry011 = true;
            patternTable.Entry010 = true;
            patternTable.Entry001 = true;
            patternTable.Entry000 = false;
        }

        /// <summary>
        /// Called when the user clicks the Rule 110 button,
        /// switching the pattern over to Rule 110.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The RoutedEventArgs that contains the event data.</param>
        private void OnRule110ButtonClicked( object sender, RoutedEventArgs e )
        {
            var patternTable = this.DataContext as CellPatternTable;
            if( patternTable == null )
            {
                return;
            }

            patternTable.Entry111 = false;
            patternTable.Entry110 = true;
            patternTable.Entry101 = true;
            patternTable.Entry100 = false;
            patternTable.Entry011 = true;
            patternTable.Entry010 = true;
            patternTable.Entry001 = true;
            patternTable.Entry000 = false;
        }
    }
}

Cellular Automata

How to start:
Open automata.html with a Silverlight compatible browser like Microsoft Internet Explorer

About:
A cellular automata consists of a grid of cells, with each cell having a number of states.
The cells of this simple automata can have one of two states: On (Black) or Off (White).

Each row in the grid represents one generation (in this automata).
The state of a row depends on the state of the previous row, and so on.

A simple rule is used to generate the state of each cell of a row:
For each entry in the row we take the (N – 1th, Nth, N + 1th) cell triple
in the previous row.

Now we take this triple and use a lookup table to get the state of the entry.
Here are the lookup tables for two of the most famous patterns:

Rule 30 cellular automaton lookup table:
current pattern	           111 110 101 100 011 010 001 000
new state for center cell	0	0	0	1	1	1	1	0
Rule 110 cellular automaton lookup table:

current pattern			   111 110 101 100 011 010 001 000
new state for center cell	0	1	1	0	1	1	1	0

Wiki: http://en.wikipedia.org/wiki/Cellular_automata

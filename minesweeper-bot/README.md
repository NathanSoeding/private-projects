# Minesweeper Bot  
While in 12'th grade I wrote a minesweeper bot that solves the browser version of minesweeper using python to read the screen and output clicking commands to play the game. A video of the working algorithm can be found in this repo.  

Minesweeper is a logic-based puzzle game where the player aims to clear a grid without detonating hidden mines. Each square on the grid may hide a mine or display a number indicating how many adjacent squares contain mines. Players reveal squares by clicking; if a mine is uncovered, the game ends. Using the numbers as clues, players deduce the location of mines, marking suspected squares to avoid them, and aim to uncover all safe squares to win.  

In each step the algorithm does the following:  
1. Read the grid
2. Determine the coordinates of candidate squares for uncovering
3. Create a list of all mine arrangements that would not violate the games rules
4. Mark all squares as mines containing one in every possible arrangement
5. Reveal all squares which do not contain a mine in a single arrangement


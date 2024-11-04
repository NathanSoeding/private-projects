# Minesweeper Bot  
In 12'th grade i wrote a minesweeper bot that solves the browser version of the game using python to read the screen and output clicking commands to play the game. The code is broken due to python library updates but a video of the working algorithm can be found in this repo.  

Minesweeper is a logic-based puzzle game where the player aims to clear a grid without detonating hidden mines. Each square on the grid may hide a mine or display a number indicating how many adjacent squares contain mines. Players reveal squares by clicking; if a mine is uncovered, the game ends. Using the numbers as clues, players deduce the location of mines, marking suspected squares to avoid them, and aim to uncover all safe squares to win.  

For each step the algorithm does the following:  
1. Read the grid and locate squares to be potentially uncovered.
```text
code();
address@domain.example
```

2. Create a list to store possible bomb arrangements in selected squares.
3. Iterating over every selected square and add a possible bomb arrangement for the two cases which are square contains a bomb and the opposite.
4. After every 

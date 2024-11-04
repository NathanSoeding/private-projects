# Minesweeper Bot  
In 12'th grade i wrote a minesweeper bot that solves the browser version of minesweeper using python to read the screen and output clicking commands to play the game. The code is broken due to python library updates but a video of the working algorithm can be found in this repo.  

Minesweeper is a logic-based puzzle game where the player aims to clear a grid without detonating hidden mines. Each square on the grid may hide a mine or display a number indicating how many adjacent squares contain mines. Players reveal squares by clicking; if a mine is uncovered, the game ends. Using the numbers as clues, players deduce the location of mines, marking suspected squares to avoid them, and aim to uncover all safe squares to win.  

In each step the algorithm does the following:  
1. Read the grid
2. Determine the coordinates of candidate squares for uncovering
3. Create a list of all mines arrangements that would not violate the games rules
4. Mark all squares as mines containing one in every possible arrangement
5. Reveal all squares which do not contain a mine in a single arrangement

```
new_possible_bomb_arrangements = [[]]

for (x, y) in group:
   possible_bomb_arrangements = new_possible_bomb_arrangements
   new_possible_bomb_arrangements = [[]]

   for arrangement in possible_bomb_arrangements:
      arrangement.append(bomb)
      if check_rules(arrangement):
         new_possible_bomb_arrangements.append(arrangement)

      arrangement.append(no bomb)
      if check_rules(arrangement):
         new_possible_bomb_arrangements.append(arrangement)
  
```

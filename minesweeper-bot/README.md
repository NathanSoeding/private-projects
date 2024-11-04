# Minesweeper Bot  
In 12'th grade i wrote a minesweeper bot that solves the browser version of the game using python to read the screen and output clicking commands to play the game. The code is broken due to python library updates but a video of the working algorithm can be found in this repo.  

Minesweeper is a logic-based puzzle game where the player aims to clear a grid without detonating hidden mines. Each square on the grid may hide a mine or display a number indicating how many adjacent squares contain mines. Players reveal squares by clicking; if a mine is uncovered, the game ends. Using the numbers as clues, players deduce the location of mines, marking suspected squares to avoid them, and aim to uncover all safe squares to win.  

For each step the algorithm does the following:  
1. Read the grid
2. Determine the coordinates of candidate squares for uncovering
3. Devide them into connected groups
4. Iterate over every square in a group:
   - assume the current square contains a bomb
   - if this assumption does not contradict known information save this bomb arrangement for the next iteration
   - assume the current square does not contain a bomb
   - again check for contradictions and save arrangement if necessary
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

2. Create a list to store possible bomb arrangements in selected squares.
3. Iterating over every selected square and add a possible bomb arrangement for the two cases which are square contains a bomb and the opposite.
4. After every 

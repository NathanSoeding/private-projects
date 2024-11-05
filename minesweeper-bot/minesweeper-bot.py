from tokenize import group
import time
import copy
import win32api, win32con
import numpy as np
import mss

field = []
bombs_found = 0

# Liest ein Feld ein und wenn dies eine 0 ist auch seine Nachbarzellen (Rekursiv)
def update_field(y, x, screenshot):
    top, left, lenght = 240 + y * 56.214, 455 + x * 56.214, 56.214

    cell = screenshot[int(top):int(top + lenght), 
                      int(left):int(left + lenght)]

    if np.any(np.all(cell == np.array([25, 118, 210]), axis=-1)):
        field[y][x] = 1
    elif np.any(np.all(cell == np.array([56, 142, 60]), axis=-1)):
        field[y][x] = 2
    elif np.any(np.all(cell == np.array([211, 47, 47]), axis=-1)):
        field[y][x] = 3
    elif np.any(np.all(cell == np.array([123, 31, 162]), axis=-1)):
        field[y][x] = 4
    elif np.any(np.all(cell == np.array([255, 143, 0]), axis=-1)):
        field[y][x] = 5
    elif np.any(np.all(cell == np.array([0, 151, 167]), axis=-1)):
        field[y][x] = 6
    else:
        field[y][x] = 0     

    if field[y][x] == 0:
        for y_flex in range(-1, 2):
            for x_flex in range(-1, 2):
                if y+y_flex >= 0 and y+y_flex < len(field) and x+x_flex >= 0 and x+x_flex < len(field[0]) and field[y+y_flex][x+x_flex] == 'u':
                    update_field(y+y_flex, x+x_flex, screenshot)

# Gibt die Anzahl an benachbarten Bomben und unbekannten Feldern für ein Feld(x,y) an
def count_neighbours(y, x, list):
    bombs = 0
    unknown = 0
    numbers = 0

    for y_flex in range(-1, 2):
        for x_flex in range(-1, 2):
            # Bedingung um nicht out of Bounds zu suchen
            if y+y_flex >= 0 and y+y_flex < len(list) and x+x_flex >= 0 and x+x_flex < len(list[0]):
                if list[y+y_flex][x+x_flex] == 'b':
                    bombs += 1
                elif list[y+y_flex][x+x_flex] == 'u':
                    unknown += 1
                elif list[y+y_flex][x+x_flex] == 'k':
                    w = 0
                elif list[y+y_flex][x+x_flex] > 0:
                    numbers += 1

    return (bombs, unknown, numbers)

# Klickt ein Feld(x,y)
def click_cell(y, x, left_or_right):
    win32api.SetCursorPos((round(480 + x * 56.214), round(265 + y * 56.214)))
    if left_or_right == 'L':
        win32api.mouse_event(win32con.MOUSEEVENTF_LEFTDOWN, 0, 0)
        time.sleep(0.001)
        win32api.mouse_event(win32con.MOUSEEVENTF_LEFTUP, 0, 0)
    else:
        win32api.mouse_event(win32con.MOUSEEVENTF_RIGHTDOWN, 0, 0)
        time.sleep(0.001)
        win32api.mouse_event(win32con.MOUSEEVENTF_RIGHTUP, 0, 0)

def pr_list(list):
    for i in range(len(list)):
        print(list[i])
    print()
     
# Prüft ob alle Zahlen die richtgie Anzahl an Bomben im Umkreis haben
def check_rules(bombs_comb, no_bombs_comb, final_check):
    aus = True
    p_field = copy.deepcopy(field)
    for bomb in bombs_comb:
        p_field[bomb[0]][bomb[1]] = 'b'
    
    if not(final_check):
        for no_bomb in no_bombs_comb:
            p_field[no_bomb[0]][no_bomb[1]] = 'k'
    
    for y in range(len(field)):
        for x in range(len(field[0])):
            if not(p_field[y][x] == 'u' or p_field[y][x] == 0 or p_field[y][x] == 'b' or p_field[y][x] == 'k'): 
                # Hat die Zahl Nachbarzellen aus group?
                check = False
                for cell in group:
                    if abs(cell[0] - y) <= 1 and abs(cell[1] - x) <= 1:
                        check = True

                if check and ((final_check and p_field[y][x] != count_neighbours(y, x, p_field)[0]) or p_field[y][x] < count_neighbours(y, x, p_field)[0]):
                    aus = False
                elif check and not(final_check) and (count_neighbours(y, x, p_field)[1] < p_field[y][x] - count_neighbours(y, x, p_field)[0]):
                    aus = False
    return aus

# Schreibt alle möglichen Bombenkombinationen in bombs_poss
def try_bomb(bombs_list, no_bombs_list, i):
    # Erhöhung der Effizienz
    if check_rules(bombs_list, no_bombs_list, False):
        if i < len(group):
            temp1 = copy.deepcopy(no_bombs_list)    
            temp1.append(group[i])
            try_bomb(bombs_list, temp1, i + 1)

            temp2 = copy.deepcopy(bombs_list)    
            temp2.append(group[i])
            try_bomb(temp2, no_bombs_list, i + 1)
        else:
            if check_rules(bombs_list, no_bombs_list, True):
                bombs_poss.append(bombs_list)

# Prüft ob ein Objekt in einer 2D Liste ist
def is_in_2D_list(a, list):
    aus = False
    for i in range(len(list)):
        if a in list[i]:
            aus = True
    return aus

# Füllt die Liste cell_group mit einer zusammenhängenden Gruppe aus cells (Rekursiv)
def fill_group(y, x):
    cell_group.append((y, x))
    for cell in cells:
        if abs(cell[0] - y) + abs(cell[1] - x) == 1 and not(is_in_2D_list(cell, cell_groups_2d)) and not(cell in cell_group):
            fill_group(cell[0], cell[1])

    for cell in cells:
        if abs(cell[0] - y) <= 1 and abs(cell[1] - x) <= 1 and not(is_in_2D_list(cell, cell_groups_2d)) and not(cell in cell_group):
            fill_group(cell[0], cell[1])

    # Wichtig da die Gruppe beeinflusst werden kann von Feldern die außerhalb der Gruppe sind
    for cell in cells:
        if abs(cell[0] - y) <= 2 and abs(cell[1] - x) <= 2 and not(is_in_2D_list(cell, cell_groups_2d)) and not(cell in cell_group):
            fill_group(cell[0], cell[1])

def start():
    # 2D Liste die das Spielfeld enthält
    global field
    field = []
    global bombs_found
    bombs_found = 0
    global screenshot
    
    for y in range(14):
        row = []
        for x in range(18):
            row.append('u')
        field.append(row)

    click_cell(6, 8, 'L')
    time.sleep(1)
    with mss.mss() as sct:
        screenshot = np.array(sct.grab(sct.monitors[1]))[:, :, :3][:, :, ::-1]
    update_field(6, 8, screenshot)

# Start
start()

win = 0
loose = 0
# Hauptschleife
while True:
    # Wenn Endscreen
    endscreen = screenshot[530, 960]

    if np.array_equal(endscreen, np.array([252, 193, 0])) or np.array_equal(endscreen, np.array([74, 192, 253])) or np.array_equal(endscreen, np.array([162, 209, 73])):
        if np.array_equal(endscreen, np.array([74, 192, 253])):
            loose += 1
            print(str(win) + " " + str(loose))
        if not np.array_equal(endscreen, np.array([162, 209, 73])):
            win += 1
            print(str(win) + " " + str(loose))
            click_cell(10, 7,'L')
        time.sleep(1)
        start()

    # Speichert jedes unknown Feld welches neben einer Zahl ist
    cells = []
    for y in range(len(field)):
        for x in range(len(field[0])):
            if field[y][x] == 'u' and count_neighbours(y, x, field)[2] > 0:
                cells.append((y, x))

    # Teilt die Liste cells in zusammenhängende Gruppen auf
    cell_groups_2d = []
    for cell in cells:
        if not(is_in_2D_list(cell, cell_groups_2d)):
            neighbours = 0
            for y_flex in range(-1, 2):
                for x_flex in range(-1, 2):
                    if (cell[0]+y_flex, cell[1]+x_flex) in cells:
                        neighbours += 1

            # Felder die am Rand liegen und nur eine Nachbarzelle haben die neben einer Zahl liegt werden priorisiert
            if neighbours == 2 and (cell[0] == 0 or cell[0] == len(field) - 1 or cell[1] == 0 or cell[1] == len(field[0]) - 1):
                cell_group = []
                fill_group(cell[0], cell[1])
                cell_groups_2d.append(cell_group)

    for cell in cells:
        if not(is_in_2D_list(cell, cell_groups_2d)):
            cell_group = []
            fill_group(cell[0], cell[1])
            cell_groups_2d.append(cell_group)
            
    
    # Vorgang für Finden der Bomben und aufdecker der Felder für jede einzelne Gruppe
    remember_update = []
    for group in cell_groups_2d:
        # Rekursion die alle möglichen Bombenkombinationen in eine Liste schreibt
        bombs_poss = []
        try_bomb([], [], 0)

        # Falls einbe Kombination von Bomben nicht möglich ist, da nicht mehr genug Bomben übrig sind wird diese entfernt
        for g in bombs_poss:
            if len(g) > 40 - bombs_found:
                bombs_poss.remove(g)

        # Markiert Bomben
        for i in range(len(bombs_poss)):
            for bomb in bombs_poss[i]:
                if field[bomb[0]][bomb[1]] != 'b':
                    # Wenn in jeder möglichen Bombenkombination die Bombe enthalten ist wir sie markiert
                    is_bomb = True
                    for h in range(len(bombs_poss)):
                        if not(bomb in bombs_poss[h]):
                            is_bomb = False
            
                    if is_bomb:
                        bombs_found += 1
                        field[bomb[0]][bomb[1]] = 'b'
                        click_cell(bomb[0], bomb[1], 'R')

        # Klickt alle Feldern an für die keine Möglichkeit besteht dass sie eine Bombe haben könnten
        for cell in group:
            if not(is_in_2D_list(cell, bombs_poss)):
                remember_update.append(cell)
                click_cell(cell[0], cell[1], 'L')


    # Für den Fall dass alle Bomben gefunden wurden
    if bombs_found == 40:
        for y in range(len(field)):                                                        
            for x in range(len(field[0])):
                if field[y][x] == 'u':
                    click_cell(y, x, 'L')

    time.sleep(1)
    with mss.mss() as sct:
        screenshot = np.array(sct.grab(sct.monitors[1]))[:, :, :3][:, :, ::-1]

    for cell in remember_update:
        update_field(cell[0], cell[1], screenshot)

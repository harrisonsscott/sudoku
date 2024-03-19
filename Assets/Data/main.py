import random, json, os, os.path

def is_valid_move(grid, row, col, num):
    for i in range(9):
        if grid[row][i] == num or grid[i][col] == num:
            return False
    start_row, start_col = 3 * (row // 3), 3 * (col // 3)
    for i in range(3):
        for j in range(3):
            if grid[start_row + i][start_col + j] == num:
                return False
    return True

def solve_sudoku(grid):
    for row in range(9):
        for col in range(9):
            if grid[row][col] == 0:
                for num in random.sample(range(1, 10), 9):
                    if is_valid_move(grid, row, col, num):
                        grid[row][col] = num
                        if solve_sudoku(grid):
                            return True
                        grid[row][col] = 0
                return False
    return True

def generate_grid():
    grid = [[0] * 9 for _ in range(9)]
    solve_sudoku(grid)
    return grid

def print_grid(grid):
    for row in grid:
        print(" ".join(str(num) for num in row))


def encode(grid):
    path = f"{os.path.dirname(__file__)}/data.json"
    string = ""
    for row in range(9):
        for col in range(9):
            string += str(grid[row][col])
    with open(path, "r") as file:
        data = json.load(file)
    data["puzzles"].append(string)
    with open(path, 'w') as file:    
        json.dump(data, file, indent=4)

for i in range(2500):
    grid = generate_grid()
    print_grid(grid)
    encode(grid)

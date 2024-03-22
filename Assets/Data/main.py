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


def encode(full, partial):
    path = f"{os.path.dirname(__file__)}/data.json"
    stringFull = ""
    stringPartial = ""
    for row in range(9):
        for col in range(9):
            stringFull += str(full[row][col])
            stringPartial += str(partial[row][col])

    with open(path, "r") as file:
        data = json.load(file)
    print(stringPartial)
    print(stringFull)
    data["puzzles"].append({"partial": stringPartial, "full": stringFull})
    with open(path, 'w') as file:
        json.dump(data, file, indent=4)


def generate_partial(grid):
    grid1 = [row[:] for row in grid]
    cells_to_remove = 40
    index = 0
    while cells_to_remove > 0:
        index += 1
        if index > 500:
            return 0
        row = random.randint(0, 8)
        col = random.randint(0, 8)
        old = grid1[row][col]
        if grid1[row][col] != 0:
            grid1[row][col] = 0
            temp_grid = [row[:] for row in grid]
            if solve_sudoku(temp_grid):
                grid1[row][col] = 0
                cells_to_remove -= 1
            else:
                grid1[row][col] = old
    return grid1


for i in range(249):
    full = generate_grid()
    partial = generate_partial(full)
    if partial != 0:
        encode(full,partial)
    else:
        print("0")



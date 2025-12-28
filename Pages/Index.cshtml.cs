using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Diagnostics;

namespace Sudoku16x16Solver.Pages;

public class IndexModel : PageModel
{
    public int[,] Grid { get; set; } = new int[16, 16];
    public int[,] OriginalGrid { get; set; } = new int[16, 16];
    public bool IsSolved { get; set; }
    public string? Message { get; set; }
    public bool IsError { get; set; }

    public void OnGet()
    {
    }

    public IActionResult OnPostSolve(Dictionary<int, Dictionary<int, string>> cells)
    {
        ParseCells(cells);
        Array.Copy(Grid, OriginalGrid, Grid.Length);

        var solver = new SudokuSolver();
        var stopwatch = Stopwatch.StartNew();
        bool solved = solver.Solve(Grid);
        stopwatch.Stop();

        if (solved)
        {
            IsSolved = true;
            Message = $"Risolto in {stopwatch.ElapsedMilliseconds} ms!";
            IsError = false;
        }
        else
        {
            Message = "Nessuna soluzione trovata. Il puzzle potrebbe essere invalido.";
            IsError = true;
        }

        return Page();
    }

    public IActionResult OnPostClear()
    {
        Grid = new int[16, 16];
        OriginalGrid = new int[16, 16];
        IsSolved = false;
        return Page();
    }

    public IActionResult OnPostExample()
    {
        Grid = GetExamplePuzzle();
        OriginalGrid = new int[16, 16];
        IsSolved = false;
        return Page();
    }

    public IActionResult OnPostExample2()
    {
        Grid = GetTestPuzzle();
        OriginalGrid = new int[16, 16];
        IsSolved = false;
        return Page();
    }

    private void ParseCells(Dictionary<int, Dictionary<int, string>> cells)
    {
        Grid = new int[16, 16];

        if (cells == null) return;

        foreach (var row in cells)
        {
            if (row.Value == null) continue;

            foreach (var col in row.Value)
            {
                var value = col.Value?.Trim().ToUpper();

                if (string.IsNullOrEmpty(value))
                {
                    Grid[row.Key, col.Key] = 0;
                }
                else if (value.Length == 1)
                {
                    char c = value[0];
                    if (char.IsDigit(c) && c >= '1' && c <= '9')
                    {
                        Grid[row.Key, col.Key] = c - '0';
                    }
                    else if (c >= 'A' && c <= 'G')
                    {
                        Grid[row.Key, col.Key] = c - 'A' + 10;
                    }
                }
            }
        }
    }

    private static int[,] GetExamplePuzzle()
    {
        return new int[16, 16]
        {
            { 1, 0, 0, 4,  0, 0, 7, 8,  9,10, 0,12, 13,14, 0,16},
            { 0, 6, 0, 8,  0, 0, 0,12,  0, 0,15,16,  1, 0, 3, 4},
            { 9,10,11,12,  1, 2, 0, 4,  5, 6, 0, 8,  0,15,16, 0},
            {13,14,15,16,  5, 6, 0, 8,  1, 2, 3, 4,  9,10,11,12},
            { 0, 0, 0, 0,  0, 0, 0, 0,  0, 0, 0, 0,  0, 0, 0, 0},
            { 0, 0, 0, 0,  0, 0, 0, 0,  0, 0, 0, 0,  0, 0, 0, 0},
            { 0, 0, 0, 0,  0, 0, 0, 0,  0, 0, 0, 0,  0, 0, 0, 0},
            { 0, 0, 0, 0,  0, 0, 0, 0,  0, 0, 0, 0,  0, 0, 0, 0},
            { 0, 0, 0, 0,  0, 0, 0, 0,  0, 0, 0, 0,  0, 0, 0, 0},
            { 0, 0, 0, 0,  0, 0, 0, 0,  0, 0, 0, 0,  0, 0, 0, 0},
            { 0, 0, 0, 0,  0, 0, 0, 0,  0, 0, 0, 0,  0, 0, 0, 0},
            { 0, 0, 0, 0,  0, 0, 0, 0,  0, 0, 0, 0,  0, 0, 0, 0},
            { 0, 0, 0, 0,  0, 0, 0, 0,  0, 0, 0, 0,  0, 0, 0, 0},
            { 0, 0, 0, 0,  0, 0, 0, 0,  0, 0, 0, 0,  0, 0, 0, 0},
            { 0, 0, 0, 0,  0, 0, 0, 0,  0, 0, 0, 0,  0, 0, 0, 0},
            { 0, 0, 0, 0,  0, 0, 0, 0,  0, 0, 0, 0,  0, 0, 0, 0}
        };
    }

    private static int[,] GetTestPuzzle()
    {
        // Puzzle 16x16 completo per test
        return new int[16, 16]
        {
            { 0, 2, 0, 0,  0, 0, 0, 0,  0, 0, 0, 0, 15, 0, 0,12},
            { 0, 0, 0, 0,  0, 6, 0, 0,  2, 0, 0, 0,  0, 0, 0, 0},
            { 0,10, 0, 0, 15, 0, 2,12,  0, 0, 0, 0,  0, 0,14, 0},
            { 0, 0,16, 0,  0, 0, 0, 5,  0,11, 0, 0,  0, 6, 0, 0},
            {14, 0, 0, 0,  0, 0, 0, 0,  0, 0, 5, 0,  6, 0, 0, 0},
            { 0, 0, 0, 0,  0, 0,14, 0,  0, 0, 0, 0,  0, 0, 0, 5},
            { 0, 0, 6, 0,  0, 0, 0, 0,  0, 0, 0,15,  0,12, 0, 0},
            { 0, 0, 0, 5,  0, 0, 0, 0, 12, 6, 0, 0,  0, 0, 0,11},
            { 6, 0, 0, 0,  0, 0,12, 0,  0, 0, 0, 0,  5, 0, 0, 0},
            { 0, 0,14, 0,  6, 0, 0, 0,  0, 0, 0, 0,  0,11, 0, 0},
            {12, 0, 0, 0,  0, 0, 0, 0,  0,14, 0, 0,  0, 0, 0, 0},
            { 0, 0, 0,11,  0, 5, 0, 0,  0, 0, 0, 0,  0, 0, 0,14},
            { 0, 0, 6, 0,  0, 0,11, 0,  5, 0, 0, 0,  0,16, 0, 0},
            { 0,14, 0, 0,  0, 0, 0, 0, 11, 0,12, 0,  0, 0,10, 0},
            { 0, 0, 0, 0,  0, 0, 0, 2,  0, 0, 6, 0,  0, 0, 0, 0},
            { 5, 0, 0,12,  0, 0, 0, 0,  0, 0, 0, 0,  0, 0, 2, 0}
        };
    }
}

public class SudokuSolver
{
    private const int Size = 16;
    private const int BoxSize = 4;

    public bool Solve(int[,] grid)
    {
        var empty = FindEmpty(grid);
        if (empty == null)
            return true;

        int row = empty.Value.Row;
        int col = empty.Value.Col;

        for (int num = 1; num <= Size; num++)
        {
            if (IsValid(grid, row, col, num))
            {
                grid[row, col] = num;

                if (Solve(grid))
                    return true;

                grid[row, col] = 0;
            }
        }

        return false;
    }

    private bool IsValid(int[,] grid, int row, int col, int num)
    {
        for (int j = 0; j < Size; j++)
        {
            if (grid[row, j] == num)
                return false;
        }

        for (int i = 0; i < Size; i++)
        {
            if (grid[i, col] == num)
                return false;
        }

        int boxRow = (row / BoxSize) * BoxSize;
        int boxCol = (col / BoxSize) * BoxSize;

        for (int i = boxRow; i < boxRow + BoxSize; i++)
        {
            for (int j = boxCol; j < boxCol + BoxSize; j++)
            {
                if (grid[i, j] == num)
                    return false;
            }
        }

        return true;
    }

    private (int Row, int Col)? FindEmpty(int[,] grid)
    {
        for (int i = 0; i < Size; i++)
        {
            for (int j = 0; j < Size; j++)
            {
                if (grid[i, j] == 0)
                    return (i, j);
            }
        }
        return null;
    }
}

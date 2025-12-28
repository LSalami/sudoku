namespace Sudoku16x16Solver;

public class DancingLinks
{
    private readonly int _size;
    private readonly int _boxSize;
    private readonly int _numConstraints;
    private readonly DLXNode _header;
    private readonly DLXNode[] _columns;

    public DancingLinks(int size, int boxSize)
    {
        _size = size;
        _boxSize = boxSize;
        _numConstraints = size * size * 4;

        _header = new DLXNode { Size = int.MaxValue };
        _columns = new DLXNode[_numConstraints];

        DLXNode prev = _header;
        for (int i = 0; i < _numConstraints; i++)
        {
            _columns[i] = new DLXNode { Size = 0, Column = null! };
            _columns[i].Column = _columns[i];
            _columns[i].Up = _columns[i];
            _columns[i].Down = _columns[i];
            _columns[i].Left = prev;
            prev.Right = _columns[i];
            prev = _columns[i];
        }
        prev.Right = _header;
        _header.Left = prev;

        for (int row = 0; row < size; row++)
            for (int col = 0; col < size; col++)
                for (int num = 1; num <= size; num++)
                    AddPossibility(row, col, num);
    }

    private void AddPossibility(int row, int col, int num)
    {
        int box = (row / _boxSize) * _boxSize + (col / _boxSize);
        int[] constraints = {
            row * _size + col,
            _size * _size + row * _size + (num - 1),
            2 * _size * _size + col * _size + (num - 1),
            3 * _size * _size + box * _size + (num - 1)
        };

        DLXNode? first = null;
        DLXNode? prev = null;

        foreach (int c in constraints)
        {
            var node = new DLXNode
            {
                Row = row,
                Col = col,
                Num = num,
                Column = _columns[c]
            };

            node.Down = _columns[c];
            node.Up = _columns[c].Up;
            _columns[c].Up.Down = node;
            _columns[c].Up = node;
            _columns[c].Size++;

            if (first == null)
            {
                first = node;
                node.Left = node;
                node.Right = node;
            }
            else
            {
                node.Left = prev!;
                node.Right = first;
                prev!.Right = node;
                first.Left = node;
            }
            prev = node;
        }
    }

    public void Cover(int row, int col, int num)
    {
        int box = (row / _boxSize) * _boxSize + (col / _boxSize);
        int[] constraints = {
            row * _size + col,
            _size * _size + row * _size + (num - 1),
            2 * _size * _size + col * _size + (num - 1),
            3 * _size * _size + box * _size + (num - 1)
        };

        foreach (int c in constraints)
            CoverColumn(_columns[c]);
    }

    private void CoverColumn(DLXNode col)
    {
        col.Right.Left = col.Left;
        col.Left.Right = col.Right;

        for (var row = col.Down; row != col; row = row.Down)
            for (var node = row.Right; node != row; node = node.Right)
            {
                node.Down.Up = node.Up;
                node.Up.Down = node.Down;
                node.Column.Size--;
            }
    }

    private void UncoverColumn(DLXNode col)
    {
        for (var row = col.Up; row != col; row = row.Up)
            for (var node = row.Left; node != row; node = node.Left)
            {
                node.Column.Size++;
                node.Down.Up = node;
                node.Up.Down = node;
            }

        col.Right.Left = col;
        col.Left.Right = col;
    }

    public bool Solve(List<(int row, int col, int num)> solution)
    {
        if (_header.Right == _header)
            return true;

        DLXNode? best = null;
        int minSize = int.MaxValue;
        for (var col = _header.Right; col != _header; col = col.Right)
            if (col.Size < minSize) { minSize = col.Size; best = col; }

        if (best == null || best.Size == 0)
            return false;

        CoverColumn(best);

        for (var row = best.Down; row != best; row = row.Down)
        {
            solution.Add((row.Row, row.Col, row.Num));

            for (var node = row.Right; node != row; node = node.Right)
                CoverColumn(node.Column);

            if (Solve(solution))
                return true;

            solution.RemoveAt(solution.Count - 1);

            for (var node = row.Left; node != row; node = node.Left)
                UncoverColumn(node.Column);
        }

        UncoverColumn(best);
        return false;
    }

    private class DLXNode
    {
        public DLXNode Left = null!;
        public DLXNode Right = null!;
        public DLXNode Up = null!;
        public DLXNode Down = null!;
        public DLXNode Column = null!;
        public int Size;
        public int Row, Col, Num;
    }
}

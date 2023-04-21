using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellManager : MonoBehaviour
{
    [SerializeField] private Cell cellPrefab;
    private Cell[] cells;
    private int columns;
    private int rows;

    public void Initialize(int columns, int rows)
    {
        this.columns = columns;
        this.rows = rows;
        cells = new Cell[columns * rows];
        for (int i = 0; i < cells.Length; i++)
        {
            var (x, y) = Index2XY(i);
            var cell = Instantiate(cellPrefab);
            cell.Initialize(this, x, y);
            cells[i] = cell;
        }
    }

    public Cell GetCell(int x, int y) => cells[XY2Index(x, y)];
    private int XY2Index(int x, int y) => x + y * columns;
    private (int x, int y) Index2XY(int i) => (i % columns, i / columns);

    public Cell GetNeighbor(Cell cell, Direction dir)
    {
        var x = cell.x - (dir == Direction.Left ? 1 : 0) + (dir == Direction.Right ? 1 : 0);
        var y = cell.y - (dir == Direction.Up ? 1 : 0) + (dir == Direction.Down ? 1 : 0);
        if (x < 0 || x >= columns) return null;
        if (y < 0 || y >= rows) return null;
        return GetCell(x, y);
    }

    public void ResetAll()
    {
        foreach (var cell in cells)
        {
            cell.ResetCell();
        }
    }
}


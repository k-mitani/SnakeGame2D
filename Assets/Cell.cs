using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    [SerializeField] private SpriteRenderer snakeSegment;
    [SerializeField] private SpriteRenderer apple;
    [SerializeField] private ParticleSystem ps;

    private CellManager cells;
    public int x;
    public int y;

    public bool IsSnakeSegment { get; private set; }
    public bool IsApple { get; private set; }
    private bool IsEmpty => !IsSnakeSegment && !IsApple;

    private void Awake()
    {
        TryGetComponent(out ps);
    }

    public void Initialize(CellManager cells, int x, int y)
    {
        this.cells = cells;
        this.x = x;
        this.y = y;
        transform.SetParent(cells.transform);
        transform.localPosition = new Vector3(x, -y, 0);
        ResetCell();
    }

    public void ResetCell()
    {
        snakeSegment.enabled = false;
        apple.enabled = false;
        IsSnakeSegment = false;
        IsApple = false;
    }

    public void SetSnakeSegment()
    {
        ResetCell();
        snakeSegment.enabled = true;
        IsSnakeSegment = true;
    }

    public void SetApple()
    {
        ResetCell();
        apple.enabled = true;
        IsApple = true;
    }

    internal void Explode()
    {
        ps.Play();
    }
}

public enum Direction
{
    None,
    Up,
    Right,
    Down,
    Left,
}

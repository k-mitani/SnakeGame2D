using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private float tickInterval = 0.1f;
    [SerializeField] private int appleCount = 3;
    [SerializeField] private int cellColumnCount = 10;
    [SerializeField] private int cellRowCount = 10;
    [SerializeField] private CellManager cells;
    [SerializeField] private TextMeshProUGUI labelHighScore;
    [SerializeField] private TextMeshProUGUI labelScore;
    [SerializeField] private TextMeshProUGUI labelPressSpace;
    private int score = 0;
    private int highScore = -1;

    private LinkedList<Cell> snakeSegments = new LinkedList<Cell>();
    private Direction currentDirection = Direction.None;
    private Direction lastInputDirection = Direction.None;

    private bool isGameOver = false;

    // Start is called before the first frame update
    void Start()
    {
        cells.Initialize(cellColumnCount, cellRowCount);
        StartGame();
    }

    private void StartGame()
    {
        labelPressSpace.enabled = false;
        UpdateScore(0);

        cells.ResetAll();
        var cell = cells.GetCell(0, 0);
        cell.SetSnakeSegment();
        snakeSegments.AddLast(cell);
        currentDirection = Direction.Right;
        lastInputDirection = Direction.None;
        
        for (int i = 0; i < appleCount; i++)
        {
            SpawnApple();
        }

        StartCoroutine(MainLoop());
    }

    private IEnumerator MainLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(tickInterval);

            var headCell = snakeSegments.First.Value;

            // 進行方向を決定する。
            // 最後の進行方向と真反対でなければそちらに曲がる。
            if (lastInputDirection != Direction.None &&
                !IsOppositeDirection(lastInputDirection, currentDirection))
            {
                currentDirection = lastInputDirection;
                lastInputDirection = Direction.None;
            }

            var nextHeadCell = cells.GetNeighbor(headCell, currentDirection);
            if (nextHeadCell == null ||
                (nextHeadCell.IsSnakeSegment && nextHeadCell != snakeSegments.Last.Value))
            {
                StartCoroutine(GameOver());
                yield break;
            }

            // 移動先にりんごがあるか確認する。
            var appleExists = nextHeadCell.IsApple;
            if (appleExists)
            {
                SpawnApple();
                UpdateScore(score + 1);
            }
            // りんごがなければ末尾のセルをリセットする。
            else
            {
                // 末尾のセルの状態をリセットする。
                var lastCell = snakeSegments.Last.Value;
                lastCell.ResetCell();
                snakeSegments.RemoveLast();
            }

            // 頭の位置を更新する。
            nextHeadCell.SetSnakeSegment();
            snakeSegments.AddFirst(nextHeadCell);
        }
    }

    private void SpawnApple()
    {
        var count = 10000;
        while (count > 0)
        {
            count--;
            var x = Random.Range(0, cellColumnCount);
            var y = Random.Range(0, cellRowCount);
            var cell = cells.GetCell(x, y);
            if (cell.IsSnakeSegment) continue;
            if (cell.IsApple) continue;
            cell.SetApple();
            return;
        }
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.UpArrow)) lastInputDirection = Direction.Up;
        else if (Input.GetKey(KeyCode.DownArrow)) lastInputDirection = Direction.Down;
        else if (Input.GetKey(KeyCode.LeftArrow)) lastInputDirection = Direction.Left;
        else if (Input.GetKey(KeyCode.RightArrow)) lastInputDirection = Direction.Right;

        if (isGameOver)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                isGameOver = false;
                StartGame();
            }
        }
    }

    private void UpdateScore(int value)
    {
        score = value;
        labelScore.text = score.ToString();
        if (score > highScore)
        {
            highScore = score;
            labelHighScore.text = highScore.ToString();
        }
    }

    private bool IsOppositeDirection(Direction a, Direction b)
    {
        return (a == Direction.Up && b == Direction.Down) ||
            (a == Direction.Down && b == Direction.Up) ||
            (a == Direction.Left && b == Direction.Right) ||
            (a == Direction.Right && b == Direction.Left);
    }

    private IEnumerator GameOver()
    {
        // 先頭から順番に消していく。
        while (snakeSegments.Count > 0)
        {
            var cell = snakeSegments.First.Value;
            cell.ResetCell();
            cell.Explode();
            snakeSegments.RemoveFirst();
            yield return new WaitForSeconds(0.02f);
        }
        labelPressSpace.enabled = true;
        isGameOver = true;
    }
}

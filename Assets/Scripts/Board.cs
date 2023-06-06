using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Random = System.Random;

namespace Match3
{
    public class Board
    {
        public event Action<int, int> OnGetScore;

        private const float SwipeDuration = 0.2f;
        private const float CollapseDuration = 1.5f;
        private const int SpawnDelay = 250;

        private Random m_Random;
        private BoardModel m_BoardModel;
        private Cell[,] m_Cells;
        private List<TileType> m_GameTileTypes;
        private Queue<Cell> m_EmptyCells;
        private Cell m_SelectedCell;

        private DataSerializator m_DataSerializator;
        private int m_CountOfSwipes;
        private GameData m_GameData;
        private bool m_IsBoardFilled;

        public Board(BoardModel boardModel)
        {
            m_Random = new Random();
            UpdateModel(boardModel);
        }

        public void UpdateModel(BoardModel boardModel)
        {
            m_BoardModel = boardModel;
            m_BoardModel.BoardUI.Init(m_BoardModel.TileInfos);
            m_GameTileTypes = GetUniqueTileTypes();
        }

        public void Start()
        {
            if (m_DataSerializator == null)
            {
                m_DataSerializator = new DataSerializator();
            }

            m_GameData = m_DataSerializator.Load<GameData>(Application.persistentDataPath) ?? GetInitialGameData();
            OnGetScore?.Invoke(m_GameData.ScoreCount, m_GameData.StepCount);
            Stop();
            CreateCells();
            FillCells();
            m_IsBoardFilled = true;
            AddCellListeners();
        }

        public void Stop()
        {
            if (m_IsBoardFilled)
            {
                RemoveCellListeners();
                DestroyCells();
                m_IsBoardFilled = false;
            }
        }

        public void SetStepsCount(int count)
        {
            m_GameData.StepCount = count;
        }

        public void Check()
        {
            foreach (var cell in m_Cells)
            {
                cell.IsMarkedToDestroy = false;
            }

            if (m_IsBoardFilled)
            {
                CheckHorizontal();
                CheckVertical();
            }
        }

        public void Destroy()
        {
            foreach (var cell in m_Cells)
            {
                if (cell.IsMarkedToDestroy)
                {
                    cell.SetTileToEmpty();
                }
            }
            CollapseTiles();
            m_DataSerializator.Save(m_GameData, Application.persistentDataPath);
            OnGetScore?.Invoke(m_GameData.ScoreCount, m_GameData.StepCount);
        }

        private void CreateCells()
        {
            m_BoardModel.BoardUI.CreateCells(m_BoardModel.ColumnCount, m_BoardModel.RowCount);
            m_Cells = new Cell[m_BoardModel.ColumnCount, m_BoardModel.RowCount];

            for (int x = 0; x < m_BoardModel.ColumnCount; x++)
            {
                for (int y = 0; y < m_BoardModel.RowCount; y++)
                {
                    var coordinates = new Coordinates(x, y);
                    m_Cells[x, y] = new Cell(coordinates, m_BoardModel.BoardUI.GetUICell(coordinates));
                    m_Cells[x, y].SetTileType(TileType.Empty);
                }
            }
        }

        private void DestroyCells()
        {
            m_Cells = null;
            m_BoardModel.BoardUI.DestroyCells();
        }

        private void FillCells()
        {
            GenerateBlocksInRandomPlaces(m_Cells, m_BoardModel.CountOfBlocks);
            GenerateRandomGameTiles(m_Cells);
        }

        private void GenerateBlocksInRandomPlaces(Cell[,] cells, int count)
        {
            int x;
            int y;
            for (int i = 0; i < count; i++)
            {
                do
                {
                    x = m_Random.Next(0, cells.GetLength(0));
                    y = m_Random.Next(0, cells.GetLength(1));
                } while (cells[x, y].TileType != TileType.Empty);
                cells[x, y].SetTileType(TileType.Block);
            }
        }

        private void GenerateRandomGameTiles(Cell[,] cells)
        {
            for (int x = 0; x < m_BoardModel.ColumnCount; x++)
            {
                for (int y = 0; y < m_BoardModel.RowCount; y++)
                {
                    if (cells[x, y].TileType == TileType.Empty)
                    {
                        cells[x, y].SetTileType(GetRandomGameTileType());
                    }
                }
            }
        }

        private TileType GetRandomGameTileType()
        {
            var randomNumber = m_Random.Next(0, m_GameTileTypes.Count);
            var randomInfo = m_GameTileTypes[randomNumber];
            return randomInfo;
        }

        private List<TileType> GetUniqueTileTypes()
        {
            var tileTypes = new List<TileType>();

            foreach (var i in m_BoardModel.TileInfos)
            {
                tileTypes.Add(i.Type);
                for (int t = 0; t < tileTypes.Count; t++)
                {
                    if (tileTypes[t] == TileType.Block || tileTypes[t] == TileType.Empty)
                    {
                        tileTypes.Remove(tileTypes[t]);
                    }
                }
            }
            return tileTypes;
        }

        private void CheckHorizontal()
        {
            var currentQueue = new Queue<Cell>();

            for (int y = 0; y < m_BoardModel.RowCount; y++)
            {
                TileType lastTile = TileType.Block;
                for (int x = 0; x < m_BoardModel.ColumnCount; x++)
                {
                    CheckLine(currentQueue, x, y, ref lastTile);
                }
                TryToMarkQueueToDestroy(currentQueue);
            }
        }

        private void CheckVertical()
        {
            var currentQueue = new Queue<Cell>();

            for (int x = 0; x < m_BoardModel.ColumnCount; x++)
            {
                TileType lastTile = TileType.Block;
                for (int y = 0; y < m_BoardModel.RowCount; y++)
                {
                    CheckLine(currentQueue, x, y, ref lastTile);
                }
                TryToMarkQueueToDestroy(currentQueue);
            }
        }

        private void CheckLine(Queue<Cell> currentQueue, int x, int y, ref TileType lastTile)
        {
            if (m_Cells[x, y].TileType != TileType.Block &&
                m_Cells[x, y].TileType == lastTile &&
                m_Cells[x, y].TileType != TileType.Empty)
            {
                currentQueue.Enqueue(m_Cells[x, y]);
            }
            else
            {
                TryToMarkQueueToDestroy(currentQueue);

                if (m_Cells[x, y].TileType != TileType.Block &&
                    m_Cells[x, y].TileType != TileType.Empty)
                {
                    currentQueue.Enqueue(m_Cells[x, y]);
                }
            }

            lastTile = m_Cells[x, y].TileType;
        }

        private void TryToMarkQueueToDestroy(Queue<Cell> tiles)
        {
            if (tiles.Count >= m_BoardModel.CountInLine)
            {
                while (tiles.Count != 0)
                {
                    tiles.Dequeue().IsMarkedToDestroy = true;
                }
            }
            else
            {
                tiles.Clear();
            }
        }

        private void AddCellListeners()
        {
            foreach (var cell in m_Cells)
            {
                cell.OnClick += SelectCell;
            }
        }

        private void RemoveCellListeners()
        {
            foreach (var cell in m_Cells)
            {
                cell.OnClick -= SelectCell;
            }
        }

        private void SelectCell(Coordinates coordinates)
        {
            if (m_SelectedCell == null)
            {
                m_SelectedCell = GetCell(coordinates);
            }
            else
            {
                if (m_SelectedCell.Coordinates.Y == coordinates.Y)
                {
                    if (m_SelectedCell.Coordinates.X - 1 == coordinates.X)
                    {
                        TryToSwipeTiles(m_SelectedCell, GetCell(coordinates));
                    }
                    else if (m_SelectedCell.Coordinates.X + 1 == coordinates.X)
                    {
                        TryToSwipeTiles(m_SelectedCell, GetCell(coordinates));
                    }
                    m_SelectedCell = null;
                }
                else if (m_SelectedCell.Coordinates.X == coordinates.X)
                {
                    if (m_SelectedCell.Coordinates.Y - 1 == coordinates.Y)
                    {
                        TryToSwipeTiles(m_SelectedCell, GetCell(coordinates));
                    }
                    else if (m_SelectedCell.Coordinates.Y + 1 == coordinates.Y)
                    {
                        TryToSwipeTiles(m_SelectedCell, GetCell(coordinates));
                    }
                    m_SelectedCell = null;
                }
                else
                {
                    m_SelectedCell = GetCell(coordinates);
                }
            }
        }

        private Cell GetCell(Coordinates coordinates)
        {
            return m_Cells[coordinates.X, coordinates.Y];
        }

        private void TryToSwipeTiles(Cell firstCell, Cell secondCell)
        {
            if (firstCell.TileType != TileType.Block && secondCell.TileType != TileType.Block && firstCell.TileType != secondCell.TileType)
            {
                SwipeCells(firstCell, secondCell, SwipeDuration, () => OnSwipeComplete(firstCell, secondCell));
            }
        }

        private void SwipeCells(Cell firstCell, Cell secondCell, float duration, Action onComplete = null)
        {
            var hashedUITile = firstCell.UITile;
            firstCell.SwipeTile(secondCell.UITile, duration, onComplete);
            secondCell.SwipeTile(hashedUITile, duration, onComplete);
        }

        private void OnSwipeComplete(Cell firstCell, Cell secondCell)
        {
            m_CountOfSwipes++;

            if (m_CountOfSwipes == 2)
            {
                m_CountOfSwipes = 0;
                m_GameData.StepCount--;
                if (CheckIfCanBeMatched(secondCell) || CheckIfCanBeMatched(firstCell))
                {
                    DestroyWhilePossible();
                }
                else
                {
                    SwipeCells(firstCell, secondCell, SwipeDuration);
                }
                OnGetScore?.Invoke(m_GameData.ScoreCount, m_GameData.StepCount);
            }
        }

        private bool CheckIfCanBeMatched(Cell cell)
        {
            int countInLine = 1;
            int countInColumn = 1;

            countInLine += GetCountOfTilesInOrder(cell, -1, 0);
            countInLine += GetCountOfTilesInOrder(cell, 1, 0);

            countInColumn += GetCountOfTilesInOrder(cell, 0, -1);
            countInColumn += GetCountOfTilesInOrder(cell, 0, 1);

            return countInColumn >= m_BoardModel.CountInLine || countInLine >= m_BoardModel.CountInLine;
        }

        private int GetCountOfTilesInOrder(Cell cell, int xStep, int yStep)
        {
            int result = -1;
            int x = cell.Coordinates.X;
            int y = cell.Coordinates.Y;

            while (m_Cells[x, y].TileType == cell.TileType)
            {
                result++;

                bool isXOnEdge = x == 0 || x == m_Cells.GetLength(0) - 1;
                bool isYOnEdge = y == 0 || y == m_Cells.GetLength(1) - 1;

                if (!isXOnEdge)
                {
                    x += xStep;
                }
                else if (xStep != 0)
                {
                    break;
                }

                if (!isYOnEdge)
                {
                    y += yStep;
                }
                else if (yStep != 0)
                {
                    break;
                }
            }

            return result;
        }

        private void DestroyWhilePossible()
        {
            Check();
            Destroy();
        }

        private void CollapseTiles()
        {
            for (int x = m_Cells.GetLength(0) - 1; x >= 0; x--)
            {
                for (int y = m_Cells.GetLength(1) - 1; y >= 0; y--)
                {
                    if (m_Cells[x, y].TileType == TileType.Empty)
                    {
                        CollapseColumn(m_Cells[x, y].Coordinates);
                        break;
                    }
                }
            }
        }

        private void CollapseColumn(Coordinates coordinates)
        {
            m_EmptyCells = new Queue<Cell>();
            for (int y = coordinates.Y; y >= 0; y--)
            {
                if (m_Cells[coordinates.X, y].TileType == TileType.Empty)
                {
                    m_EmptyCells.Enqueue(m_Cells[coordinates.X, y]);
                }
                else
                {
                    if (m_EmptyCells.Count > 0)
                    {
                        var lastEmptyCell = m_EmptyCells.Dequeue();
                        SwipeCells(lastEmptyCell, m_Cells[coordinates.X, y], CollapseDuration);
                        m_EmptyCells.Enqueue(m_Cells[coordinates.X, y]);
                    }
                }
            }
            _ = FillEmptyCellsByNewTiles(coordinates.X, m_EmptyCells.Dequeue().Coordinates.Y);
        }

        private async Task FillEmptyCellsByNewTiles(int x, int y)// question
        {
            m_GameData.ScoreCount+=3;
            for (int i = y; i >= 0; i--)
            {
                await Task.Delay(SpawnDelay);
                m_Cells[x, 0].SetTileType(GetRandomGameTileType());
                var emptyCell = m_Cells[x, i];
                if (m_Cells[x, 0].Coordinates != emptyCell.Coordinates)
                {
                    SwipeCells(m_Cells[x, 0], emptyCell, CollapseDuration);
                }
                else
                {
                    break;
                }
            }
        }

        private GameData GetInitialGameData()
        {
            return new GameData(0, m_BoardModel.StepsCountForLevel);
        }

        public void Reset()
        {
            m_DataSerializator.Reset<GameData>(Application.persistentDataPath);
        }
    }
}
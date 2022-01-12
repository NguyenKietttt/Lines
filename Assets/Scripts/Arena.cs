using System.Collections.Generic;
using UnityEngine;

public class Arena : MonoBehaviour
{
    [SerializeField] private int _requiredTilesInLine = 3;

    private ArenaTile[,] _tile;
    private List<ArenaTile> _tileList;
    private Game _game;
    private int _maxX, _maxY;

    public ArenaTile[,] tile
    {
        get { return _tile; }
        private set { _tile = value; }
    }
    

    void Awake()
    {
        _maxX = transform.GetChild(0).childCount;
        _maxY = transform.childCount;

        tile = new ArenaTile[_maxX, _maxY];
        _tileList = new List<ArenaTile>();

        _game = GameObject.FindGameObjectWithTag("GameController").GetComponent<Game>();

        for (int y = 0; y < _maxY; ++y)
        {
            for (int x = 0; x < _maxX; ++x)
            {
                tile[x, y] = transform.GetChild(y).GetChild(x).GetComponent<ArenaTile>();
                _tileList.Add(tile[x, y]);
            }
        }
    }

    public List<ArenaTile> getEmptyTiles()
    {
        return _tileList.FindAll(b => b.empty);
    }

    public void removeAllTiles()
    {
        foreach (var obj in _tileList.FindAll(b => !b.empty))
        {
            obj.tile.remove();
        }
    }

    public void checkPoints(bool spawnCheck = false)
    {
        checkPointsRow();
        checkPointsColumn();

        if (!spawnCheck)
        {
            _game.spawner.spawn();
        }
    }

    private void checkPointsRow()
    {
        for (int y = 0; y < _maxY; ++y)
        {
            int sameColor = 1, start = 0;

            for (int x = 1; x < _maxX; ++x)
            {
                tilesCompareState compState = compareTiles(tile[x, y], tile[x - 1, y]);

                if (compState == tilesCompareState.SAME)
                {
                    ++sameColor;

                    if (x == _maxX - 1 && sameColor >= _requiredTilesInLine)
                    {
                        removeRow(y, start, x);
                    }
                }
                else
                {
                    if (sameColor >= _requiredTilesInLine)
                    {
                        removeRow(y, start, x - 1);
                    }

                    sameColor = 1;
                    start = x;
                }
            }
        }
    }

    private void checkPointsColumn()
    {
        for (int x = 0; x < _maxX; ++x)
        {
            int sameColor = 1, start = 0;

            for (int y = 1; y < _maxY; ++y)
            {
                tilesCompareState compState = compareTiles(tile[x, y], tile[x, y - 1]);

                if (compState == tilesCompareState.SAME)
                {
                    ++sameColor;

                    if (y == _maxY - 1 && sameColor >= _requiredTilesInLine)
                    {
                        removeColumn(x, start, y);
                    }
                }
                else
                {
                    if (sameColor >= _requiredTilesInLine)
                    {
                        removeColumn(x, start, y - 1);
                    }

                    sameColor = 1;
                    start = y;
                }
            }
        }
    }



    private tilesCompareState compareTiles(ArenaTile A, ArenaTile B)
    {
        if (A.empty || B.empty)
            return tilesCompareState.EMPTY;

        if (A.tile.color == B.tile.color)
            return tilesCompareState.SAME;

        return tilesCompareState.DIFFERENT;
    }

    private void removeRow(int row, int start, int end)
    {
        _game.addPoints(end - start + 1);

        for (int i = start; i <= end; ++i)
        {
            tile[i, row].tile.remove();
        }
    }

    private void removeColumn(int col, int start, int end)
    {
        _game.addPoints(end - start + 1);

        for (int i = start; i <= end; ++i)
        {
            tile[col, i].tile.remove();
        }
    }
}

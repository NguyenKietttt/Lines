using System.Collections.Generic;
using UnityEngine;

public class TileSpawner : MonoBehaviour
{
    [SerializeField] private GameObject _tilePrefab;
    [SerializeField] private Transform _tileParent;
    [SerializeField] private Color[] _possibleColors;

    private Tile[] _tiles;
    private Arena _arena;
    private Game _game;

    void Awake()
    {
        _tiles = GetComponentsInChildren<Tile>();
        _arena = GameObject.FindGameObjectWithTag("Arena").GetComponent<Arena>();
        _game = GameObject.FindGameObjectWithTag("GameController").GetComponent<Game>();
    }

    public void randNewTiles()
    {
        foreach (var til in _tiles)
        {
            int rand = Random.Range(0, _possibleColors.Length);
            til.color = _possibleColors[rand];
        }
    }

    public void spawn()
    {
        List<ArenaTile> possibleTiles = _arena.getEmptyTiles();

        if (possibleTiles.Count < 6) 
        {
            _game.gameLost();
        }
        else
        {
            foreach (var ti in _tiles)
            {
                ArenaTile pos = possibleTiles[Random.Range(0, possibleTiles.Count)];
                possibleTiles.Remove(pos);
                Tile obj = Instantiate(_tilePrefab, _tileParent).GetComponent<Tile>();
                obj.initialize(pos, ti.color);
            }

            randNewTiles();
            _arena.checkPoints(true);
        }
    }
}

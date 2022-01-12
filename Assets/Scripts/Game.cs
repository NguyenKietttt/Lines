using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Game : MonoBehaviour
{
    [SerializeField] private TileSpawner _spawner;
    [SerializeField] private Text _pointsText;

    private int _points = 0;

    public TileSpawner spawner 
    { 
        get { return _spawner; } 
    }


    void Start()
    {
        newGame();
    }

    public void newGame()
    {
        spawner.randNewTiles();
        _points = 0;
        _pointsText.text = _points.ToString();
        GameObject.FindGameObjectWithTag("Arena").GetComponent<Arena>().removeAllTiles();
        spawner.spawn();
    }

    public void addPoints(int pointsToAdd)
    {
        _points += pointsToAdd;
        _pointsText.text = _points.ToString();
    }

    public void gameLost()
    {
        SceneManager.LoadScene(0);
    }
}

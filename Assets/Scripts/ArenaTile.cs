using UnityEngine;

public class ArenaTile : MonoBehaviour
{
    private TileManager _tileManager;
    private Tile _tile;
    private bool _empty = true;

    public Tile tile
    {
        get { return _tile; }
        set
        {
            _tile = value;
            empty = (value == null);
        }
    }

    public bool empty
    {
        get { return _empty; }
        private set { _empty = value; }
    }

    void Start()
    {
        _tileManager = GameObject.FindGameObjectWithTag("TileManager").GetComponent<TileManager>();
    }

    private void OnMouseDown()
    {
        if (!(_tileManager.selected is null) && empty)
        {
            _tileManager.selected.moveToPosition(this);
        }
    }
}

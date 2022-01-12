using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Tile : MonoBehaviour
{
    [SerializeField] private Material _matUnselect;
    [SerializeField] private Material _matSelect;
    private MeshRenderer _mesh;
    private Color _color;
    private TileManager _manager;
    private NavMeshAgent _navMesh;
    private NavMeshObstacle _navObstacle;
    private ArenaTile _currentTile;
    private Arena _arena;
    private Animation _anim;
    private bool _selected = false;
    private bool _movement = false;
    private bool _toRemove = false;

    public bool selected
    {
        get { return _selected; }
        set
        {
            if (value)
            {
                if (!(_manager.selected is null))
                {
                    _manager.selected.selected = false;
                }
                _mesh.material = _matSelect;
                _manager.selected = this;
            }
            else
            {
                _mesh.material = _matUnselect;
                _manager.selected = null;
            }

            _selected = value;
            if (!_toRemove)
            {
                StartCoroutine(toggleNavigation());
            }
            _mesh.material.color = color;
        }
    }

    public bool movement
    {
        get { return _movement; }
        private set { _movement = value; }
    }

    public Color color
    {
        get { return _color; }
        set
        {
            _color = value;
            if (!(_mesh is null))
            {
                _mesh.material.color = value;
            }
            else
            {
                _mesh = GetComponent<MeshRenderer>();
                color = value;
            }
        }
    }

    void Start()
    {
        _mesh = GetComponent<MeshRenderer>();
        _navMesh = GetComponent<NavMeshAgent>();
        _navObstacle = GetComponent<NavMeshObstacle>();
        _manager = transform.parent.GetComponent<TileManager>();
        _arena = GameObject.FindGameObjectWithTag("Arena").GetComponent<Arena>();
        _anim = GetComponent<Animation>();
    }

    void LateUpdate()
    {
        if (movement)
        {
            if (_navMesh.remainingDistance == Mathf.Infinity)
            {
                transform.rotation = new Quaternion();
            }

            if (_navMesh.remainingDistance == 0)
            {
                _arena.checkPoints();
                movement = false;
                selected = false;
            }
        }
    }

    void OnMouseDown()
    {
        if (selected && !movement)
        {
            selected = false;
        }
        else
        {
            if (_manager.selected is null || !_manager.selected.movement)
            {
                selected = true;
            }
        }
    }

    public void initialize(ArenaTile arTile, Color col)
    {
        transform.position = arTile.transform.position;
        color = col;
        _currentTile = arTile;
        arTile.tile = this;

        _anim = GetComponent<Animation>();
        transform.localScale = new Vector3(0, transform.localScale.y, 0);
        _anim.Play("Tile New");
    }

    public void moveToPosition(ArenaTile tile)
    {
        Vector3 pos = tile.transform.position;
        NavMeshPath path = new NavMeshPath();
        _navMesh.CalculatePath(pos, path);

        if (path.status == NavMeshPathStatus.PathComplete)
        {
            _navMesh.SetPath(path);

            if (!(_currentTile is null))
            {
                _currentTile.tile = null;
            }
            _currentTile = tile;
            tile.tile = this;
            movement = true;
        }
        else
        {
            selected = false;
        }
    }

    public void remove()
    {
        _toRemove = true;
        selected = false;
        _currentTile.tile = null;

        _anim.Play("Tile Remove");
    }

    public void Destroy() => Destroy(gameObject);

    private IEnumerator toggleNavigation()
    {
        float waitTime = 0.01f;

        if (_navMesh.enabled)
        {
            _navMesh.enabled = false;
            yield return new WaitForSeconds(waitTime);
            _navObstacle.enabled = true;
        }
        else
        {
            _navObstacle.enabled = false;
            yield return new WaitForSeconds(waitTime);
            _navMesh.enabled = true;
        }
    }
}

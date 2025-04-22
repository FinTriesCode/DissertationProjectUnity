using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WFC_Builder : MonoBehaviour
{
    //initialise fields
    [SerializeField]
    public int _width, _height;

    private WFC_Node[,] _grid;

    public List<WFC_Node> _nodes = new List<WFC_Node>();
    private List<Vector2Int> _toCollapse = new List<Vector2Int>();

    private Vector2Int[] _offsets = new Vector2Int[]
    {
        new Vector2Int(0, 1),
        new Vector2Int(0, -1),
        new Vector2Int(1, 0),
        new Vector2Int(-1, 0)
    };

    public DataManager _dataManager;

    private void Awake()
    {
        _dataManager = FindObjectOfType<DataManager>();
    }

    private void Start()
    {
        //set up grid size
        _grid = new WFC_Node[_width, _height];

        //call main recusrive method
            //responsible for level generation
        Collapse();

        //update number of scenes generated
        _dataManager._wfcScenesLoaded++;
    }

    private void Collapse()
    {
        //ensure canvas is clear for a new generation
        _toCollapse.Clear();

        //initialise starting node (middle of board)
        //_toCollapse.Add(new Vector2Int(_width / 2, _height / 2));
        _toCollapse.Add(new Vector2Int(0, 0));

        //recursive generation
        while (_toCollapse.Count > 0)
        {
            int x = _toCollapse[0].x;
            int y = _toCollapse[0].y;

            //list of nodes to keep track of propogation potentials
            List<WFC_Node> _potentialNodes = new List<WFC_Node>(_nodes);

            //loop through neighbouring nodes
            for (int i = 0; i < _offsets.Length; i++)
            {
                //get neighbor position
                Vector2Int _neighbour = new Vector2Int(x + _offsets[i].x, y + _offsets[i].y);

                if (IsInGridRange(_neighbour)) //check if in range of grid
                {
                    //set neighbouring node's position on grid
                    WFC_Node _neighbourNode = _grid[_neighbour.x, _neighbour.y];

                    //if neighbour has not been designated a node type and is valid
                        //trim down potential node posibilities (propogation) to find neighbouring node's model/object type
                            //do this for each neighbour of each grid slot
                    if (_neighbourNode != null)
                    {
                        switch (i)
                        {
                            case 0:
                                TrimNodes(_potentialNodes, _neighbourNode._bottomConnection._compatibleNodes);
                                break;
                            case 1:
                                TrimNodes(_potentialNodes, _neighbourNode._topConnection._compatibleNodes);
                                break;
                            case 2:
                                TrimNodes(_potentialNodes, _neighbourNode._leftConnection._compatibleNodes);
                                break;
                            case 3:
                                TrimNodes(_potentialNodes, _neighbourNode._rightConnection._compatibleNodes);
                                break;

                        }
                    }
                    else
                    {
                        //if the neighbour has not been added to the nodes to collapse, add it
                        if (!_toCollapse.Contains(_neighbour)) _toCollapse.Add(_neighbour);
                    }
                }
            }

            if (_potentialNodes.Count < 1)
            {
                //attempt to place node at designated grid slot, inform user that pairing is invalid
                _grid[x, y] = _nodes[0];

                Debug.LogWarning($"Attempted to collapse wave on ({x}, {y}) but found no compatible nodes.");
            }
            else
            {
                //if invalid or not present, pick a random node and fill it in
                _grid[x, y] = _potentialNodes[Random.Range(0, _potentialNodes.Count)];
            }

            //instantiate new node/model onto the grid - at designated grid coords
            GameObject _newNode = Instantiate(_grid[x, y]._prefab, new Vector3(x, 0f, y) * 4, _grid[x, y]._prefab.transform.rotation);

            //remove from list of nodes that needed to be collapsed
            _toCollapse.RemoveAt(0);
        }
    }

    private bool IsInGridRange(Vector2Int _pVector2Int)
    {
        return (_pVector2Int.x > -1 && _pVector2Int.x < _width && _pVector2Int.y > -1 && _pVector2Int.y < _height);
    }

    private void TrimNodes(List<WFC_Node> _pPotentialNodes, List<WFC_Node> _validNodes)
    {
        for (int i = _pPotentialNodes.Count - 1; i > -1; i--)
        {
            if (!_validNodes.Contains(_pPotentialNodes[i])) _pPotentialNodes.RemoveAt(i);
        }
    }

}

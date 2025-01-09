using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WFC_Builder : MonoBehaviour
{
    [SerializeField]
    private int _width, _height;

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


    private void Start()
    {
        _grid = new WFC_Node[_width, _height];

        Collapse();
    }

    private void Collapse()
    {
        _toCollapse.Clear();

        _toCollapse.Add(new Vector2Int(_width / 2, _height / 2));

        while (_toCollapse.Count > 0)
        {
            int x = _toCollapse[0].x;
            int y = _toCollapse[0].y;

            List<WFC_Node> _potentialNodes = new List<WFC_Node>(_nodes);

            for (int i = 0; i < _offsets.Length; i++)
            {
                Vector2Int _neighbour = new Vector2Int(x + _offsets[i].x, y + _offsets[i].y);

                if (IsInGridRange(_neighbour))
                {
                    WFC_Node _neighbourNode = _grid[_neighbour.x, _neighbour.y];

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
                        if (!_toCollapse.Contains(_neighbour)) _toCollapse.Add(_neighbour);
                    }
                }
            }

            if (_potentialNodes.Count < 1)
            {
                _grid[x, y] = _nodes[0];

                Debug.LogWarning($"Attempted to collapse wave on ({x}, {y}) but found no compatible nodes.");
            }
            else
            {
                _grid[x, y] = _potentialNodes[Random.Range(0, _potentialNodes.Count)];
            }

            GameObject _newNode = Instantiate(_grid[x, y]._prefab, new Vector3(x, 0f, y) * 4, _grid[x, y]._prefab.transform.rotation);

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

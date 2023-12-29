
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    #region Member Variables

    /* Editor Setup */
    public Vector2 _GridWorldSize;
    public LayerMask UnwalkableMask;
    public Transform Player;
    public float NodeRadius;
    public bool IsDisplayGridGizmos;
    
    /* Fields */
    private Node[,] _grid;

    private float _nodeDiameter;
    private int _gridSizeX;
    private int _gridSizeY;

    #endregion


    
    #region Getter

    public int MaxSize => _gridSizeX * _gridSizeY;

    #endregion
    


    #region Behavior

    private void Awake()
    {
        _nodeDiameter = NodeRadius * 2;
        _gridSizeX = Mathf.RoundToInt(_GridWorldSize.x / _nodeDiameter);
        _gridSizeY = Mathf.RoundToInt(_GridWorldSize.y / _nodeDiameter);

        CreateGrid();
    }

    #endregion



    #region Grid

    private void CreateGrid()
    {
        _grid = new Node[_gridSizeX, _gridSizeY];
        var worldBottomLeft = transform.position - Vector3.right * _GridWorldSize.x / 2 - Vector3.forward * _GridWorldSize.y / 2;

        for (var x = 0; x < _gridSizeX; ++x)
        {
            for (var y = 0; y < _gridSizeY; ++y)
            {
                var worldPoint = worldBottomLeft + Vector3.right * (x * _nodeDiameter + NodeRadius) + Vector3.forward * (y * _nodeDiameter + NodeRadius);
                var walkable = !Physics.CheckSphere(worldPoint, NodeRadius, UnwalkableMask);
                _grid[x, y] = new Node(walkable, worldPoint, x, y);
            }
        }
    }

    #endregion



    #region Get Grid Nodes

    public List<Node> GetNeighbours(Node node)
    {
        var neighbours = new List<Node>();

        for (var x = -1; x <= 1; ++x)
        {
            for (var y = -1; y <= 1; ++y)
            {
                if(x == 0 && y == 0) continue;

                var checkX = node.GridX + x;
                var checkY = node.GridY + y;

                if (checkX >= 0 && checkX < _gridSizeX && checkY >= 0 && checkY < _gridSizeY)
                {
                    neighbours.Add(_grid[checkX, checkY]);
                }
            }
        }

        return neighbours;
    }

    public Node GetNodeFromWorldPoint(Vector3 worldPosition)
    {
        var percentX = (worldPosition.x + _GridWorldSize.x / 2f) / _GridWorldSize.x;
        var percentY = (worldPosition.z + _GridWorldSize.y / 2f) / _GridWorldSize.y;

        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        var x = Mathf.RoundToInt((_gridSizeX - 1) * percentX);
        var y = Mathf.RoundToInt((_gridSizeY - 1) * percentY);

        return _grid[x, y];
    }

    #endregion
    
    
    
    #region Sub Methods

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(_GridWorldSize.x, 1, _GridWorldSize.y));

        if (_grid == null || !IsDisplayGridGizmos) return;
        
        foreach (var node in _grid)
        {
            Gizmos.color = (node.Walkable) ? Color.white : Color.red;
            Gizmos.DrawCube(node.WorldPosition, Vector3.one * (_nodeDiameter - 0.1f));
        }
    }

    #endregion
}


using UnityEngine;

public class Node : IHeapItem<Node>
{
    #region Properties

    public bool Walkable { get; private set; }
    public Vector3 WorldPosition { get; private set; }

    public int GridX { get; private set; }
    public int GridY { get; private set; }

    public int GCost { get; set; }
    public int HCost { get; set; }
    public Node ParentNode { get; set; }
    
    public int HeapIndex { get; set; }

    #endregion
    
    

    // Constructor
    public Node(bool walkable, Vector3 worldPosition, int gridX, int gridY)
    {
        Walkable = walkable;
        WorldPosition = worldPosition;
        
        GridX = gridX;
        GridY = gridY;
    }



    #region Getter

    private int FCost => GCost + HCost;

    #endregion
    

    public int CompareTo(Node nodeToCompare)
    {
        // FCost가 작은 노드를 더 높게 평가하도록 한다.
        // FCost가 동일한 경우 HCost가 작은 노드를 선호하게 한다.
        var compare = FCost.CompareTo(nodeToCompare.FCost);
        
        if (compare == 0)
        {
            compare = HCost.CompareTo(nodeToCompare.HCost);
        }

        return -compare;
    }
}

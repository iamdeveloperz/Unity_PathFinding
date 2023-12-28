
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;

public class PathFinder : MonoBehaviour
{
    private Grid _grid;

    private void Awake()
    {
        _grid = GetComponent<Grid>();
    }

    public void StartFindPath(Vector3 startPos, Vector3 targetPos)
    {
        StartCoroutine(FindPath(startPos, targetPos));
    }

    private IEnumerator FindPath(Vector3 startPos, Vector3 targetPos)
    {
        Stopwatch sw = new Stopwatch();
        sw.Start();
        
        var wayPoints = Array.Empty<Vector3>();
        var pathSuccess = false;
        
        var startNode = _grid.GetNodeFromWorldPoint(startPos);
        var targetNode = _grid.GetNodeFromWorldPoint(targetPos);

        if (startNode.Walkable && targetNode.Walkable)
        {
            var openSet = new Heap<Node>(_grid.MaxSize);
            var closedSet = new HashSet<Node>();

            openSet.Add(startNode);

            while (openSet.Count > 0)
            {
                var currentNode = openSet.RemoveFirst();

                // Heap을 사용하지 않고 List를 사용했을 때
                // for (var i = 1; i < openSet.Count; ++i)
                // {
                //     if (openSet[i].FCost < currentNode.FCost || openSet[i].FCost == currentNode.FCost && openSet[i].HCost < currentNode.HCost)
                //     {
                //         currentNode = openSet[i];
                //     }
                // }
                //
                // openSet.Remove(currentNode);
                closedSet.Add(currentNode);

                if (currentNode == targetNode)
                {
                    sw.Stop();
                    print("Path Found: " + sw.ElapsedMilliseconds + " ms");
                    pathSuccess = true;
                    break;
                }

                foreach (var neighbour in _grid.GetNeighbours(currentNode))
                {
                    if (!neighbour.Walkable || closedSet.Contains(neighbour)) continue;

                    var newMovementCostToNeighbour = currentNode.GCost + GetDistance(currentNode, neighbour);

                    if (newMovementCostToNeighbour >= neighbour.GCost && openSet.Contains(neighbour)) continue;

                    neighbour.GCost = newMovementCostToNeighbour;
                    neighbour.HCost = GetDistance(neighbour, targetNode);
                    neighbour.ParentNode = currentNode;

                    if (!openSet.Contains(neighbour))
                    {
                        openSet.Add(neighbour);
                    }
                    else
                    {
                        openSet.UpdateItem(neighbour);
                    }
                }
            }
        }

        yield return null;

        if (pathSuccess)
        {
            wayPoints = RetracePath(startNode, targetNode);
        }
        
        PathRequestManager.Instance.FinishedProcessingPath(wayPoints, pathSuccess);
    }

    private Vector3[] RetracePath(Node startNode, Node endNode)
    {
        var path = new List<Node>();
        var currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);

            currentNode = currentNode.ParentNode;
        }

        var wayPoints = SimplifyPath(path);

        Array.Reverse(wayPoints);

        return wayPoints;
    }

    Vector3[] SimplifyPath(List<Node> path)
    {
        var wayPoints = new List<Vector3>();
        var directionOld = Vector2.zero;

        for (var i = 1; i < path.Count; ++i)
        {
            var directionNew = new Vector2(
                path[i - 1].GridX - path[i].GridX,
                path[i - 1].GridY - path[i].GridY);

            if (directionNew != directionOld)
            {
                wayPoints.Add(path[i].WorldPosition);
            }

            directionOld = directionNew;
        }

        return wayPoints.ToArray();
    }

    private int GetDistance(Node nodeA, Node nodeB)
    {
        var distanceX = Mathf.Abs(nodeA.GridX - nodeB.GridX);
        var distanceY = Mathf.Abs(nodeA.GridY - nodeB.GridY);
        
        if(distanceX > distanceY)
        {
            return distanceY * 14 + (distanceX - distanceY) * 10;
        }

        return distanceX * 14 + (distanceY - distanceX) * 10;
    }
}

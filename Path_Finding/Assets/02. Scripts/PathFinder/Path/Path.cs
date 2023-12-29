
using UnityEngine;

public class Path
{
    #region Member Variables

    public readonly Vector3[] LookPoints;
    public readonly PathLine[] TurnBoundaries;
    
    public readonly int FinishLineIndex;

    #endregion



    #region Properties

    public int SlowDownIndex { get; private set; }

    #endregion



    #region Constructor

    public Path(Vector3[] wayPoints, Vector3 startPos, float turnDst, float stoppingDst)
    {
        // Set Up Fields
        LookPoints = wayPoints;
        TurnBoundaries = new PathLine[LookPoints.Length];
        FinishLineIndex = TurnBoundaries.Length - 1;
        SlowDownIndex = LookPoints.Length - 1; // 기본값 설정

        var prevPoint = Vector3ToVector2(startPos);
        for (var i = 0; i < LookPoints.Length; i++)
        {
            CreatePathLine(i, prevPoint, turnDst);
            prevPoint = Vector3ToVector2(LookPoints[i]);
        }

        CalculateSlowDownIndex(stoppingDst);
    }

    #endregion



    #region Initialize by Constructor => Create and Calculating

    private void CreatePathLine(int index, Vector2 prevPoint, float turnDst)
    {
        var currentPoint = Vector3ToVector2(LookPoints[index]);
        var dirToCurrentPoint = (currentPoint - prevPoint).normalized;
        var turnBoundaryPoint = (index == FinishLineIndex) ? currentPoint : currentPoint - dirToCurrentPoint * turnDst;

        TurnBoundaries[index] = new PathLine(turnBoundaryPoint, prevPoint - dirToCurrentPoint * turnDst);
    }
    
    private void CalculateSlowDownIndex(float stoppingDst)
    {
        var dstFromEndPoint = 0f;
        
        for (var i = LookPoints.Length - 1; i > 0; --i)
        {
            dstFromEndPoint += Vector3.Distance(LookPoints[i], LookPoints[i - 1]);
            
            if (!(dstFromEndPoint > stoppingDst)) continue;
            
            SlowDownIndex = i;
            break;
        }
    }

    #endregion



    #region Sub Methods

    private Vector2 Vector3ToVector2(Vector3 vector3)
    {
        return new Vector2(vector3.x, vector3.z);
    }

    #endregion



    #region Sub Methods

    public void DrawWithGizmos()
    {
        Gizmos.color = Color.black;
        foreach (var point in LookPoints)
        {
            Gizmos.DrawCube(point + Vector3.up, Vector3.one);
        }
        
        Gizmos.color = Color.white;
        foreach (var line in TurnBoundaries)
        {
            line.DrawWithGizmos(10);
        }
    }

    #endregion
    
}


using UnityEngine;

public class PathLine
{
    #region Member Variables

    private float _gradient;
    private float _interceptY;
    private float _gradientPerpendicular;

    private Vector2 _pointOnLine1;
    private Vector2 _pointOnLine2;

    private readonly bool _approachSide;
    
    // Literals
    private const float VERTICAL_LINE_GRADIENT = 1e5f;

    #endregion


    #region Constructor

    public PathLine(Vector2 pointOnLine, Vector2 pointPerpendicularToLine)
    {
        CalculateGradientAndIntercept(pointOnLine, pointPerpendicularToLine);
        _approachSide = GetSide(pointPerpendicularToLine);
    }
    
    #endregion

    
    
    #region Initialize By Constructor

    private void CalculateGradientAndIntercept(Vector2 pointOnLine, Vector2 pointPerpendicularToLine)
    {
        var dx = pointOnLine.x - pointPerpendicularToLine.x;
        var dy = pointOnLine.y - pointPerpendicularToLine.y;

        if (dx == 0)
        {
            _gradient = VERTICAL_LINE_GRADIENT;
        }
        else
        {
            _gradient = dy / dx;
        }

        _interceptY = pointOnLine.y - _gradient * pointOnLine.x;
        _pointOnLine1 = pointOnLine;
        _pointOnLine2 = pointOnLine + new Vector2(1, _gradient);
    }

    #endregion



    #region Sub Methods

    public bool HasCrossedLine(Vector2 point)
    {
        return GetSide(point) != _approachSide;
    }

    public float DistanceFromPoint(Vector2 point)
    {
        var interceptYPerpendicular = point.y - _gradientPerpendicular * point.x;
        var interceptX = (interceptYPerpendicular - _interceptY) / (_gradient - _gradientPerpendicular);
        var interceptY = _gradient * interceptX + _interceptY;

        return Vector2.Distance(point, new Vector2(interceptX, interceptY));
    }

    private bool GetSide(Vector2 point)
    {
        return (point.x - _pointOnLine1.x) * (_pointOnLine2.y - _pointOnLine1.y) >
               (point.y - _pointOnLine1.y) * (_pointOnLine2.x - _pointOnLine1.x);
    }
    
    // Draw Gizmos
    public void DrawWithGizmos(float length)
    {
        var lineDir = new Vector3(1, 0, _gradient).normalized;
        var lineCentre = new Vector3(_pointOnLine1.x, 0, _pointOnLine1.y) + Vector3.up;
        
        Gizmos.DrawLine(lineCentre - lineDir * length / 2f, lineCentre + lineDir * length / 2f);
    }

    #endregion
}

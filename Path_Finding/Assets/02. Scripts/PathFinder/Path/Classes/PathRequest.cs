
using System;
using UnityEngine;

public class PathRequest
{
    #region Member Variables

    public Vector3 PathStart;
    public Vector3 PathEnd;
    
    public readonly Action<Vector3[], bool> Callback;

    #endregion



    #region Constructor

    public PathRequest(Vector3 pathStart, Vector3 pathEnd, Action<Vector3[], bool> callback)
    {
        PathStart = pathStart;
        PathEnd = pathEnd;
        Callback = callback;
    }

    #endregion
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathRequest
{
    #region Member Variables

    public Vector3 PathStart;
    public Vector3 PathEnd;
    public Action<Vector3[], bool> Callback;

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

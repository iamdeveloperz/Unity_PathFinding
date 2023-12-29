
using System;
using UnityEngine;

public class PathResult
{
    #region Member Variables

    public readonly Vector3[] Path;
    public readonly bool IsSuccess;
    public readonly Action<Vector3[], bool> Callback;

    #endregion
    

    
    // Constructor
    public PathResult(Vector3[] path, bool isSuccess, Action<Vector3[], bool> callback)
    {
        Path = path;
        IsSuccess = isSuccess;
        Callback = callback;
    }
}


using System;
using System.Collections.Generic;
using UnityEngine;

public class PathRequestManager : SingletonBehavior<PathRequestManager>
{
    #region Member Variables

    // Path Find Requests
    private readonly Queue<PathRequest> _pathRequestQueue = new Queue<PathRequest>();
    private PathRequest _currentPathRequest;
    
    // Path Finder
    private PathFinder _pathFinder;

    private bool _isProcessingPath;

    #endregion



    #region Mono Behaviour

    private void Awake()
    {
        _pathFinder = FindFirstObjectByType<PathFinder>();
    }

    #endregion
    

    
    public static void RequestPath(Vector3 pathStart, Vector3 pathEnd, Action<Vector3[], bool> callback)
    {
        var newRequest = new PathRequest(pathStart, pathEnd, callback);

        Instance._pathRequestQueue.Enqueue(newRequest);
        Instance.TryProcessNext();
    }

    private void TryProcessNext()
    {
        if (_isProcessingPath || _pathRequestQueue.Count <= 0) return;
        
        _currentPathRequest = _pathRequestQueue.Dequeue();
        _isProcessingPath = !_isProcessingPath;

        _pathFinder.StartFindPath(_currentPathRequest.PathStart, _currentPathRequest.PathEnd);
    }

    public void FinishedProcessingPath(Vector3[] path, bool success)
    {
        _currentPathRequest.Callback(path, success);

        _isProcessingPath = false;
        
        TryProcessNext();
    }
}


using System;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

// Singleton Behavior는 임시용 DontBeDestroyed가 아니어야함
public class PathRequestManager : SingletonBehavior<PathRequestManager>
{
    #region Member Variables

    //private static PathRequestManager _instance;

    // Path Find Requests
    private readonly Queue<PathRequest> _pathRequestQueue = new();

    // Path Finder
    private PathFinder _pathFinder;

    #endregion



    #region Mono Behaviour

    private void Awake()
    {
        //_instance = this;
        _pathFinder = FindFirstObjectByType<PathFinder>();

        StartPathFindingThread();
    }

    // private void Update()
    // {
    //     if (_pathResultQueue.Count <= 0) return;
    //     
    //     var itemsInQueue = _pathResultQueue.Count;
    //
    //     lock (_pathResultQueue)
    //     {
    //         for (var i = 0; i < itemsInQueue; ++i)
    //         {
    //             var result = _pathResultQueue.Dequeue();
    //             result.Callback(result.Path, result.IsSuccess);
    //         }
    //     }
    // }

    #endregion



    #region Thread

    private void StartPathFindingThread()
    {
        var pathFindingThread = new Thread(new ThreadStart(ProcessPathRequests));
        pathFindingThread.Start();
    }
    
    private void ProcessPathRequests()
    {
        while (true)
        {
            if (_pathRequestQueue.Count <= 0) continue;
            
            lock (_pathRequestQueue)
            {
                if (_pathRequestQueue.Count <= 0) continue;
                
                var request = _pathRequestQueue.Dequeue();
                var result = _pathFinder.FindPath(request);
                UnityMainThreadDispatcher.Instance.Enqueue(() => request.Callback(result.Path, result.IsSuccess));
            }
        }
    }

    #endregion
    

    
    public static void RequestPath(PathRequest request)
    {
        lock (Instance._pathRequestQueue)
        {
            Instance._pathRequestQueue.Enqueue(request);
        }
    }
}

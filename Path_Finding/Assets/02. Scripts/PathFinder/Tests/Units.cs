
using System;
using System.Collections;
using UnityEngine;

public class Units : MonoBehaviour
{
    public Transform Target;

    private float _speed = 20f;
    private int _targetIndex;
    
    private Vector3[] _path;

    #region Mono Behaviour

    private void Start()
    {
        PathRequestManager.RequestPath(transform.position, Target.position, OnPathFound);
    }
    
    #endregion

    public void OnPathFound(Vector3[] newPath, bool pathSuccessful)
    {
        if (!pathSuccessful) return;
        
        _path = newPath;
        
        StopCoroutine(FollowPath());
        StartCoroutine(FollowPath());
    }

    private IEnumerator FollowPath()
    {
        var currentWayPoint = _path[0];

        while (true)
        {
            if (transform.position == currentWayPoint)
            {
                ++_targetIndex;
                if (_targetIndex >= _path.Length) yield break;
            }

            currentWayPoint = _path[_targetIndex];
            
            transform.position = Vector3.MoveTowards(transform.position, currentWayPoint, _speed * Time.deltaTime);
            yield return null;
        }
    }

    private void OnDrawGizmos()
    {
        if (_path == null) return;
        
        for (var i = _targetIndex; i < _path.Length; ++i)
        {
            Gizmos.color = Color.black;
            Gizmos.DrawCube(_path[i], Vector3.one);

            Gizmos.DrawLine(i == _targetIndex ? transform.position : _path[i - 1], _path[i]);
        }
    }
}

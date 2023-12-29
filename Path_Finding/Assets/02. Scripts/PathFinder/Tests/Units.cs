
using System.Collections;
using UnityEngine;

public class Units : MonoBehaviour
{
    private const float MIN_PATH_UPDATE_TIME = 0.2f;
    private const float PATH_UPDATE_MOVE_THRESHOLD = 0.5f;
    
    public Transform Target;

    public float Speed = 20f;
    public float TurnSpeed = 3f;
    public float TurnDst = 5f;
    public float StoppingDst = 10f;

    private Path _path;

    private void OnPathFound(Vector3[] wayPoints, bool pathSuccessful)
    {
        if (!pathSuccessful) return;

        _path = new Path(wayPoints, transform.position, TurnDst, StoppingDst);
        
        StopCoroutine(FollowPath());
        StartCoroutine(FollowPath());
    }

    private void Start()
    {
        StartCoroutine(UpdatePath());
    }

    private IEnumerator UpdatePath()
    {
        if (Time.timeSinceLevelLoad < 0.3f)
        {
            yield return new WaitForSeconds(0.3f);
        }
        
        PathRequestManager.RequestPath(new PathRequest(transform.position, Target.position, OnPathFound));
        
        var sqrMoveThreshold = Mathf.Pow(PATH_UPDATE_MOVE_THRESHOLD, PATH_UPDATE_MOVE_THRESHOLD);
        var targetPosOld = Target.position;
        
        while (true)
        {
            yield return new WaitForSeconds(MIN_PATH_UPDATE_TIME);

            if (!((Target.position - targetPosOld).sqrMagnitude > sqrMoveThreshold)) continue;
            
            PathRequestManager.RequestPath(new PathRequest(transform.position, Target.position, OnPathFound));
            targetPosOld = Target.position;
        }
    }

    private IEnumerator FollowPath()
    {
        var isFollowingPath = true;
        var pathIndex = 0;
        var speedPercent = 1f;

        transform.LookAt(_path.LookPoints[0]);
        
        while (true)
        {
            var posV2 = new Vector2(transform.position.x, transform.position.z);

            while (_path.TurnBoundaries[pathIndex].HasCrossedLine(posV2))
            {
                if (pathIndex == _path.FinishLineIndex)
                {
                    isFollowingPath = false;
                    break;
                }
                else
                {
                    ++pathIndex;
                }
            }

            if (isFollowingPath)
            {
                if (pathIndex >= _path.SlowDownIndex && StoppingDst > 0)
                {
                    speedPercent = Mathf.Clamp01(_path.TurnBoundaries[_path.FinishLineIndex].DistanceFromPoint(posV2) / StoppingDst);
                    if (speedPercent < 0.01f)
                    {
                        isFollowingPath = false;
                    }
                }

                var targetRotation = Quaternion.LookRotation(_path.LookPoints[pathIndex] - transform.position);
                transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * TurnSpeed);
                transform.Translate(Vector3.forward * (Time.deltaTime * (Speed * speedPercent)));
            }
            
            yield return null;
        }
    }

    private void OnDrawGizmos()
    {
        _path?.DrawWithGizmos();
    }
}

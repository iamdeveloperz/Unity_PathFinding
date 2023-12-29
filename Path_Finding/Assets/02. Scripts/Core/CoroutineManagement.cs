using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;

public class CoroutineManagement : SingletonBehavior<CoroutineManagement>
{
    #region Coroutines

    private enum CoroutineState
    {
        Running,
        Finished
    }

    private class CoroutineInfo
    {
        public Coroutine Coroutine;
        public CoroutineState State;
    }

    private readonly Dictionary<IEnumerator, CoroutineInfo> _coroutines =
        new Dictionary<IEnumerator, CoroutineInfo>();
    
    // Exception Event => 예외에 대한 이벤트 등록을 꼭 해야됌.
    public event Action<Exception> OnCoroutineException;

    #endregion



    #region Behavior

    private void Awake()
    {
        // 기본 예외 처리 핸들러 구독
        OnCoroutineException += HandleCoroutineException;
    }

    protected override void OnDestroy()
    {
        // 구독 해제
        // 싱글톤 인스턴스가 아직 존재하는지 확인
        if (Instance != null)
        {
            OnCoroutineException -= HandleCoroutineException;
        }
    }

    #endregion



    #region Managed Coroutine

    public Coroutine StartManagedCoroutine(IEnumerator coroutine, float delaySeconds = 0f, Action onComplete = null)
    {
        if (_coroutines.TryGetValue(coroutine, out var existingCoroutine))
        {
            if (existingCoroutine.State != CoroutineState.Finished)
            {
                // 이미 실행 중인 코루틴일 경우 해당 코루틴 반환
                return existingCoroutine.Coroutine;
            }
        }

        var coroutineInstance = StartCoroutine(delaySeconds > 0 ?
            // 지연 시간이 있을 경우 지연 시작
            DelayedStartCoroutine(coroutine, onComplete, delaySeconds) :
            // 지연 시간이 없을 경우 바로 시작
            CoroutineWrapper(coroutine, onComplete));

        var coroutineInfo = new CoroutineInfo
        {
            Coroutine = coroutineInstance,
            State = CoroutineState.Running
        };

        _coroutines[coroutine] = coroutineInfo;

        return coroutineInstance;
    }

    public void StopManagedCoroutine(IEnumerator coroutine)
    {
        if (!_coroutines.TryGetValue(coroutine, out var runningCoroutine)) return;
    
        if (runningCoroutine.State == CoroutineState.Running)
        {
            StopCoroutine(runningCoroutine.Coroutine);
            runningCoroutine.State = CoroutineState.Finished; // 상태를 Finished로 업데이트
        }

        _coroutines.Remove(coroutine);
    }

    #endregion

    
    
    #region Getter

    public List<IEnumerator> GetRunningCoroutines()
    {
        return (from entry in _coroutines where entry.Value.State == CoroutineState.Running select entry.Key).ToList();
    }

    #endregion

    
    
    #region Coroutine Utils
    
    private IEnumerator DelayedStartCoroutine(IEnumerator coroutine, Action onComplete, float delaySeconds)
    {
        yield return new WaitForSeconds(delaySeconds);
        yield return StartCoroutine(CoroutineWrapper(coroutine, onComplete));
    }

    private IEnumerator CoroutineWrapper(IEnumerator coroutine, Action onComplete)
    {
        yield return RunCoroutineWithExceptionHandling(coroutine);

        onComplete?.Invoke();

        if (_coroutines.TryGetValue(coroutine, out var info))
        {
            info.State = CoroutineState.Finished;
        }
    }

    private IEnumerator RunCoroutineWithExceptionHandling(IEnumerator coroutine)
    {
        while (true)
        {
            object current;
            try
            {
                if (!coroutine.MoveNext())
                {
                    yield break;
                }
                current = coroutine.Current;
            }
            catch (Exception exception)
            {
                OnCoroutineException?.Invoke(exception);
                yield break;
            }

            yield return current;
        }
    }

    #endregion
    
    
    
    #region Basic Exception Handler
    
    private void HandleCoroutineException(Exception exception)
    {
        // 예외 발생 시 기본 로그 처리
        Debug.LogError($"Coroutine exception: {exception}");
    }
    
    #endregion
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using MacacaGames.EffectSystem.Model;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MacacaGames.EffectSystem
{
    public class DefaultTimerBase : IEffectTimer
    {
        public DefaultTimerBase(
            Action _OnTimerStart,
            Action _OnTimerComplete,
            Action _OnTimerPause,
            Action _OnTimerStop,
            Action _OnTimerTick
        )
        {
            this._OnTimerStart = _OnTimerStart;
            this._OnTimerComplete = _OnTimerComplete;
            this._OnTimerPause = _OnTimerPause;
            this._OnTimerStop = _OnTimerStop;
            this._OnTimerTick = _OnTimerTick;
        }

        public bool IsFinish => currentTime <= 0;
        public bool IsPause => isPause;
        public bool IsStop => isStop;

        public bool IsCounting => currentTime > 0;

        public float CurrentTime
        {
            get
            {

                return currentTime;
            }
        }

        bool isPause = false;
        bool isStop = false;
        float currentTime = -1;

        public string GetId()
        {
            return EffectSystemScriptableBuiltIn.TimerTickerId.Default;
        }

        public void Tick(float delta)
        {
            // Debug.Log($"Tick(float {delta})");
            if (isStop == false && isPause == false && currentTime > 0)
            {
                // Debug.Log($"real Tick(float {delta})");
                currentTime -= delta;
                OnTimerTick();
            }
        }

        public void Reset()
        {
            isStop = false;
            isPause = false;
            currentTime = -1;
            ClearChecking();
        }

        public void Start(float targetTime)
        {
            Debug.Log($"Start counting: {targetTime}");
            currentTime = targetTime;
            // checkingCoroutine = CoroutineManager.Instance.StartCoroutine(TimerChecking());
            checkingTask = TimerChecking();
            OnTimerStart();
        }

        // Coroutine checkingCoroutine;
        private Task checkingTask;
        CancellationTokenSource cancellationTokenSource;
        const float checkInterval = 0.033f; // 30fps
        async Task TimerChecking()
        {
            cancellationTokenSource = new CancellationTokenSource();
            while (cancellationTokenSource.IsCancellationRequested == false)
            {
                if (currentTime <= 0 || Mathf.Approximately(currentTime, 0))
                {
                    cancellationTokenSource.Dispose();
                    break;
                }
                await Task.Delay(TimeSpan.FromSeconds(checkInterval));
            }
            OnTimerComplete();
        }
        void ClearChecking()
        {
            if (checkingTask != null && checkingTask.IsCompleted == false)
            {
                cancellationTokenSource.Cancel();
            }
            cancellationTokenSource?.Dispose();
        }

        public void Stop()
        {
            if (currentTime <= 0)
            {
                return;
            }
            isStop = true;
            currentTime = -1;
            OnTimerStop();
        }

        public bool Toggle()
        {
            isPause = !isPause;
            if (isPause)
            {
                OnTimerPause();
            }
            return isPause;
        }

        Action _OnTimerStart;
        Action _OnTimerComplete;
        Action _OnTimerPause;
        Action _OnTimerStop;
        Action _OnTimerTick;
        public virtual void OnTimerStart()
        {
            _OnTimerStart?.Invoke();
        }
        public virtual void OnTimerComplete()
        {
            _OnTimerComplete?.Invoke();
        }
        public virtual void OnTimerPause()
        {
            _OnTimerPause?.Invoke();
        }
        public virtual void OnTimerStop()
        {
            _OnTimerStop?.Invoke();
        }
        public virtual void OnTimerTick()
        {
            _OnTimerTick?.Invoke();
        }
    }
}
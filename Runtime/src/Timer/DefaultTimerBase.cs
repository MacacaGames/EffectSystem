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
        public float StartTime
        {
            get
            {
                return startTime;
            }
        }

        bool isPause = false;
        bool isStop = false;
        float currentTime = -1;
        float startTime = -1;
        bool isIgnoreTimerChecking = false;

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
                TimerCheck();
            }
        }

        public void Reset()
        {
            isStop = false;
            isPause = false;
            currentTime = -1;
        }

        public void Start(float targetTime)
        {
            startTime = targetTime;
            currentTime = targetTime;
            isIgnoreTimerChecking = targetTime <= 0;
            OnTimerStart();
        }

        protected virtual void TimerCheck()
        {
            if(isIgnoreTimerChecking) return;
            
            if (currentTime <= 0 || Mathf.Approximately(currentTime, 0))
            {
                OnTimerComplete();
            }
        }

        public void Stop()
        {
            if (currentTime <= 0 && isIgnoreTimerChecking == false)
            {
                return;
            }

            if (isStop)
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
using MacacaGames.EffectSystem.Model;
using System.Collections.Generic;

namespace MacacaGames.EffectSystem
{

    public class TimerTicker
    {
        public TimerTicker(string Id)
        {
            this.Id = Id;
        }

        public string Id { get; private set; }
        List<IEffectTimer> timers = new List<IEffectTimer>();
        Queue<IEffectTimer> waitForRemoveTimer = new Queue<IEffectTimer>();
        Queue<IEffectTimer> waitForAddTimer = new Queue<IEffectTimer>();

        /// <summary>
        /// Mark a time is wait for add a timer to Ticker, the timer will be remove after next Tick()
        /// </summary>
        /// <param name="timer"></param>
        public void AddTimer(IEffectTimer timer)
        {
            waitForAddTimer.Enqueue(timer);
        }


        /// <summary>
        /// Mark a time is wait for remove, the timer will be remove after next Tick()
        /// </summary>
        /// <param name="timer"></param>
        public void RemoveTimer(IEffectTimer timer)
        {
            waitForRemoveTimer.Enqueue(timer);
        }

        public void Tick(float delta)
        {
            foreach (var item in timers)
            {
                item.Tick(delta);
            }
            while (waitForAddTimer.Count > 0)
            {
                var item = waitForAddTimer.Dequeue();
                timers.Add(item);
            }
            while (waitForRemoveTimer.Count > 0)
            {
                var item = waitForRemoveTimer.Dequeue();
                timers.Remove(item);
            }
        }
    }

    public interface IEffectTimer
    {
        string GetId();
        void Tick(float delta);
        bool IsPause { get; }
        bool IsStop { get; }
        bool IsCounting { get; }
        bool IsFinish { get; }

        /// <summary>
        /// Start a timer with target time
        /// </summary>
        /// <param name="targetTime">For instance, 3 secs.</param>
        void Start(float targetTime);
        /// <summary>
        /// Fire once on the timer is start
        /// </summary>
        void OnTimerStart();
        /// <summary>
        /// Fire once when the timer is complete normally
        /// </summary>
        void OnTimerComplete();
        /// <summary>
        /// Fire each time on the timer is toggle pause/unpause
        /// </summary>
        void OnTimerPause();

        /// <summary>
        /// Fire once when the timer is stop by outer reason
        /// </summary>
        void OnTimerStop();

        /// <summary>
        /// Switch the Timer pause status
        /// </summary>
        /// <returns>The current pause status, true is in-pause</returns>
        bool Toggle();

        /// <summary>
        /// Force stop the timer, once the timer is stop, it cannot be start again
        /// </summary>
        void Stop();

        /// <summary>
        /// Reset timer to default value
        /// </summary>
        void Reset();

    }
}
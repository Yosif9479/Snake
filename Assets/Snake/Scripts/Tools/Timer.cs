using System;
using UnityEngine;
using UnityEngine.Events;

namespace Tools
{
    [Serializable]
    public class Timer
    {
        public Timer(float time, bool repeat = false)
        {
            _initialTime = time;
            _repeat = repeat;
        }

        public UnityEvent Finished { get; private set; } = new();

        public float CurrentTime { get; private set; }
        public bool IsRunning { get; private set; }

        private readonly float _initialTime;
        private readonly bool _repeat;

        public void Start()
        {
            Reset();
            IsRunning = true;
        }

        public void Tick()
        {
            if (!IsRunning) return;

            CurrentTime -= Time.deltaTime;

            if (CurrentTime > 0) return;
            
            Reset();
            
            if (_repeat) Start();
            
            Finished?.Invoke();
        }

        public void Pause()
        {
            IsRunning = false;
        }

        public void Resume()
        {
            IsRunning = true;
        }
        
        private void Reset()
        {
            CurrentTime = _initialTime;
            IsRunning = false;
        }
    }
}
using System.Timers;
using UnityEngine;
using Game339.Shared.Services;

namespace Game
{
    public class TimerComponent : MonoBehaviour
    {
        private Timer _timer;

        public float roundTimer = 60f; 

        private void Awake()
        {
            _timer = new Timer(new UnityTimeProvider());
            _timer.Start(roundTimer);
        }

        private void Update()
        {
            _timer.Tick();
            Debug.Log(_timer.Current);
        }
    }
}
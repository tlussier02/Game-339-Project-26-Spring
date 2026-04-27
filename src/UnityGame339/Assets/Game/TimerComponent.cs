using UnityEngine;
using Game339.Shared.Services;

namespace Game
{
    public class TimerComponent : MonoBehaviour
    {
        private Game339.Shared.Services.Timer _timer;

        public float roundTimer = 60f; 

        private void Awake()
        {
            _timer = new Game339.Shared.Services.Timer(new UnityTimeProvider());
            _timer.Start(roundTimer);
        }

        private void Update()
        {
            _timer.Tick();
            Debug.Log(_timer.Current);
        }
    }
}

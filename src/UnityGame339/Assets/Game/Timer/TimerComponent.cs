using System;
using UnityEngine;
using Game339.Shared.Services;
using TMPro;

namespace Game
{
    public class TimerComponent : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _timerText;
        
        private Game339.Shared.Services.Timer _timer;
        public float roundTimer = 60f; 
        public event Action OnTimerFinished;

        private void Awake()
        {
            _timer = new Game339.Shared.Services.Timer(new UnityTimeProvider());
            _timer.Start(roundTimer);
        }

        private void Update()
        {
            _timer.Tick();
            _timerText.text = Mathf.CeilToInt(_timer.Current).ToString();
            if(_timer.Current <= 0)
            {
                OnTimerFinished?.Invoke();  // 👈 "hey anyone listening, timer is done!"
            }
        }
        
    }
}

using Game.Runtime;
using UnityEngine;

public class PoisonDiceAudioView : MonoBehaviour
{

    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioClip gameplayBGM;

    private PoisonDiceRoundState _currentState;

    private void Awake()
    {
        if (bgmSource != null && gameplayBGM != null)
        {
            bgmSource.clip = gameplayBGM;
            bgmSource.loop = true;
            bgmSource.Play();
        }
    }

}

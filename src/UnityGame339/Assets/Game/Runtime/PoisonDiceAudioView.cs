using Game.Runtime;
using UnityEngine;

public class PoisonDiceAudioView : MonoBehaviour
{

    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioClip gameplayBGM;

    private PoisonDiceRoundState _currentState;

    public void Render(PoisonDiceScreenViewModel viewModel)
    {
        if (viewModel.RoundState != _currentState)
        {
            _currentState = viewModel.RoundState;
            PlayBGMForState(_currentState);
        }
    }

    private void PlayBGMForState(PoisonDiceRoundState state)
    {
        bgmSource.clip = gameplayBGM;
        bgmSource.Play();
    
    }

}

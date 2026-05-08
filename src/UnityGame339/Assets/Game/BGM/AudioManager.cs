using Game.Runtime.FarmMatch;
using UnityEngine;

public enum SFXType
 {
     Select,
     FailedSelect,
     Match
 }

public enum BGMType
{
    MainResult,
    Game
}

public class AudioManager : MonoBehaviour
{
    //singleton
    public static AudioManager Instance { get; private set; }

    public AudioSource musicSource;
    public AudioSource sfxSource;
    
    public AudioClip selectClip;
    public AudioClip failedSelectClip;
    public AudioClip matchClip;
    
    public AudioClip mainResultClip;
    public AudioClip gameClip;

    [SerializeField] private FarmMatchGameController _controller;
    
    

    private void OnEnable()
    { 
        if (_controller == null || _controller.Model == null) return;
        _controller.Model.MatchResolved += OnMatchResolved;
        _controller.Model.RoundEnded += OnRoundEnded;
        _controller.Model.StateChanged += OnStateChanged;
    }

    private void OnDisable()
    {
        _controller.Model.MatchResolved -= OnMatchResolved;
        _controller.Model.RoundEnded -= OnRoundEnded;
        _controller.Model.StateChanged -= OnStateChanged;
    }
    private void OnMatchResolved(FarmMatchResolution resolution)
    {
        Play(SFXType.Match);
    }

    private void OnRoundEnded(FarmMatchRoundResult result)
    {
        PlayBGM(BGMType.MainResult);
    }

    private void OnStateChanged()
    {
        if (_controller.Model.State.RoundState == FarmMatchRoundState.Playing)
        {
            if (musicSource.clip != gameClip) // only switch if not already playing!
            {
                PlayBGM(BGMType.Game);
            }
        }
        
        if (_controller.Model.State.LastSelectionFailureReason != FarmMatchSelectionFailureReason.None)
        {
            Play(SFXType.FailedSelect);
        }
    
        // selection sfx when a crop is successfully added to selection
        if (_controller.Model.State.LastSelectionClearReason == FarmMatchSelectionClearReason.None 
            && _controller.Model.State.SelectionCount > 0)
        {
            Play(SFXType.Select);
        }
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void Play(SFXType type)
    {
        switch (type)
        {
            case SFXType.Select:
                PlaySingleSoundEffect(selectClip);
                break;
            case SFXType.FailedSelect:
                PlaySingleSoundEffect(failedSelectClip);
                break;
            case SFXType.Match:
                PlaySingleSoundEffect(matchClip);
                break;
        }
    }

    public void PlayBGM(BGMType bgm)
    {
        musicSource.loop = true;
        switch (bgm)
        { 
            case BGMType.MainResult:
                musicSource.clip = mainResultClip;
                break;
            case BGMType.Game:
                musicSource.clip = gameClip;
                break;
        }
        musicSource.Play();
    }

    public void PlaySingleSoundEffect(AudioClip clip)
    {
        if (clip != null && sfxSource != null)
            sfxSource.PlayOneShot(clip);
    }
}

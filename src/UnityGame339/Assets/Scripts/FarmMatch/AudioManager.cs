using System.Collections;
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

[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Sources")]
    public AudioSource musicSource;
    public AudioSource sfxSource;

    [Header("SFX")]
    public AudioClip selectClip;
    public AudioClip failedSelectClip;
    public AudioClip matchClip;

    [Header("BGM")]
    public AudioClip mainResultClip;
    public AudioClip gameClip;

    [Header("Volume")]
    [Range(0f, 1f)] [SerializeField] private float musicVolume = 0.35f;
    [Range(0f, 1f)] [SerializeField] private float sfxVolume = 0.8f;

    private Coroutine _bgmRoutine;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            if (HasAssignedClips() && !Instance.HasAssignedClips())
            {
                Destroy(Instance.gameObject);
            }
            else
            {
                Destroy(gameObject);
                return;
            }
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        ResolveSources();
        Debug.Log("AudioManager ready. Music source assigned: "
            + (musicSource != null)
            + ", SFX source assigned: "
            + (sfxSource != null)
            + ", game clip assigned: "
            + (gameClip != null)
            + ", results clip assigned: "
            + (mainResultClip != null));
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    public static AudioManager Resolve()
    {
        return Instance != null ? Instance : FindFirstObjectByType<AudioManager>();
    }

    public void PlayGameBgm()
    {
        PlayBGM(BGMType.Game);
    }

    public void PlayResultsBgm()
    {
        PlayBGM(BGMType.MainResult);
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
        if (musicSource == null)
        {
            Debug.LogWarning("AudioManager cannot play BGM because musicSource is not assigned.");
            return;
        }

        AudioClip nextClip = null;
        switch (bgm)
        {
            case BGMType.MainResult:
                nextClip = mainResultClip;
                break;
            case BGMType.Game:
                nextClip = gameClip;
                break;
        }

        if (nextClip == null)
        {
            Debug.LogWarning("AudioManager cannot play BGM because the requested clip is not assigned.");
            return;
        }

        if (_bgmRoutine != null)
        {
            StopCoroutine(_bgmRoutine);
        }

        _bgmRoutine = StartCoroutine(PlayBgmWhenLoaded(nextClip));
    }

    public void PlaySingleSoundEffect(AudioClip clip)
    {
        if (clip == null)
        {
            Debug.LogWarning("AudioManager cannot play SFX because the requested clip is not assigned.");
            return;
        }

        if (sfxSource != null)
        {
            StartCoroutine(PlaySfxWhenLoaded(clip));
            return;
        }

        Debug.LogWarning("AudioManager cannot play SFX because sfxSource is not assigned.");
    }

    private void ResolveSources()
    {
        if (musicSource == null)
        {
            musicSource = GetComponent<AudioSource>();
        }

        if (sfxSource == null)
        {
            var sources = GetComponents<AudioSource>();
            sfxSource = sources.Length > 1 ? sources[1] : musicSource;
        }

        if (musicSource != null)
        {
            ForceAudible(musicSource);
        }

        if (sfxSource != null)
        {
            ForceAudible(sfxSource);
        }
    }

    private static void LoadClip(AudioClip clip)
    {
        if (clip != null && clip.loadState == AudioDataLoadState.Unloaded)
        {
            clip.LoadAudioData();
        }
    }

    private IEnumerator PlayBgmWhenLoaded(AudioClip clip)
    {
        yield return WaitForClipLoad(clip);

        ForceAudible(musicSource);
        musicSource.loop = true;
        musicSource.volume = musicVolume;
        musicSource.spatialBlend = 0f;
        musicSource.clip = clip;
        musicSource.Play();

        Debug.Log("AudioManager playing BGM: "
            + clip.name
            + " length="
            + clip.length
            + "s loadState="
            + clip.loadState
            + " sourceVolume="
            + musicSource.volume
            + " isPlaying="
            + musicSource.isPlaying);

        if (!musicSource.isPlaying)
        {
            Debug.LogWarning("AudioManager called Play on BGM, but the music source did not start playing.");
        }
    }

    private IEnumerator PlaySfxWhenLoaded(AudioClip clip)
    {
        yield return WaitForClipLoad(clip);

        ForceAudible(sfxSource);
        sfxSource.spatialBlend = 0f;
        sfxSource.PlayOneShot(clip, sfxVolume);
        Debug.Log("AudioManager playing SFX: " + clip.name + " length=" + clip.length + "s loadState=" + clip.loadState);
    }

    private static IEnumerator WaitForClipLoad(AudioClip clip)
    {
        if (clip == null)
        {
            yield break;
        }

        LoadClip(clip);

        while (clip.loadState == AudioDataLoadState.Loading)
        {
            yield return null;
        }

        if (clip.loadState != AudioDataLoadState.Loaded)
        {
            Debug.LogWarning("AudioManager clip is not loaded: " + clip.name + " state=" + clip.loadState);
        }
    }

    private static void ForceAudible(AudioSource source)
    {
        AudioListener.pause = false;
        AudioListener.volume = 1f;

        if (source == null)
        {
            return;
        }

        source.enabled = true;
        source.mute = false;
        source.ignoreListenerPause = true;
        source.ignoreListenerVolume = true;
        source.spatialBlend = 0f;
    }

    private bool HasAssignedClips()
    {
        return gameClip != null
            || mainResultClip != null
            || selectClip != null
            || failedSelectClip != null
            || matchClip != null;
    }
}

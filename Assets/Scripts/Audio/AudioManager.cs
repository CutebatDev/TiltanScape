using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{

    [HideInInspector] public AudioManager Instance;
    
    
    [Header("Audio Mixers")]
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private string masterMixerVolumeName = "MasterVolume";
    [SerializeField] private string musicMixerVolumeName = "MusicVolume";
    [SerializeField] private string sfxMixerVolumeName = "SFXVolume";
    
    //[Header("Clips")]

    [Header("Music")] 
    private AudioSource currentMusic;
    [SerializeField] private AudioClip mainMenuMusic;
    [SerializeField] private AudioClip floor3Music;
    [SerializeField] private AudioClip floor2AndAHalfMusic;
    [SerializeField] private AudioClip floor2Music;
    
    [Header("Prefabs")]
    [SerializeField] private GameObject audioSourcePrefab;

    
    void Awake()
    {
        if (!Instance) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
    
    

    #region Mixer Functions
    
    public void SetAudioMixerVolume(AudioType type, float volume)
    {
        string volumeName = "";
        switch (type)
        {
            case AudioType.Master:
                volumeName = masterMixerVolumeName;
                break;
            case AudioType.Music:
                volumeName = musicMixerVolumeName;
                break;
            case AudioType.SFX:
                volumeName = sfxMixerVolumeName;
                break;
        }
        audioMixer.SetFloat(volumeName, LinearVolumeToDB(volume));
    }


    public void SetMasterVolume(float volume)
    {
        SetAudioMixerVolume(AudioType.Master, volume);
    }


    public void SetMusicVolume(float volume)
    {
        SetAudioMixerVolume(AudioType.Music, volume);
    }


    public void SetSFXVolume(float volume)
    {
        SetAudioMixerVolume(AudioType.SFX, volume);
    }
    
    #endregion
    


    #region Helper Functions

    private float LinearVolumeToDB(float volume)
    {
        return Mathf.Log10(Mathf.Clamp(volume, 0.0001f, 1f)) * 20;
    }

    #endregion
    
    
    
    #region General Functions

    public AudioSource PlayAudio3D(AudioClip audioClip, Vector3 position, float volume)
    {
        return null;
    }


    public AudioSource PlayAudio2D(AudioClip audioClip, float volume)
    {
        return null;
    }
    
    #endregion
    
    
    #region Music Functions
    /// <summary>
    /// Play Music For Whichever Floor
    /// </summary>
    public void PlayMusic(Floor floor)
    {
        AudioClip music = null;
        switch (floor)
        {
            case Floor.Floor3:
                music = floor3Music;
                break;
            case Floor.Floor2AndAHalf:
                music = floor2AndAHalfMusic;
                break;
            case Floor.Floor2:
                music = floor2Music;
                break;
        }
        currentMusic = PlayAudio2D(music,0f);
    }


    /// <summary>
    /// Resume the music player if paused
    /// </summary>
    public void ResumeMusic()
    {
        
    }


    /// <summary>
    /// Pause the currently playing music 
    /// </summary>
    public void PauseMusic()
    {
        
    }
    

    /// <summary>
    /// Indefinitely Stop the current Music
    /// </summary>
    public void StopMusic()
    {
        
    }
    #endregion


    public enum Floor
    {
        Floor3,
        Floor2AndAHalf,
        Floor2
    }

    public enum AudioType
    {
        Master,
        SFX,
        Music,
    }
}

using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{

    public AudioManager Instance;
    
    
    [Header("Audio Mixers")]
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private string mainMixerName = "Main";
    [SerializeField] private string musicMixerName = "Music";
    [SerializeField] private string sfxMixerName = "SFX";
    
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

    public void SetAudioMixerVolume(float volume)
    {
        
    }

    #endregion
    


    #region Helper Functions

    private float LinearVolumeToDB(float volume)
    {
        return Mathf.Log10(volume) * 20;
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
}

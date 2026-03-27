using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{

    [HideInInspector] public AudioManager Instance;
    
    
    [Header("Audio Mixers")]
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private AudioMixerGroup masterMixerGroup;
    [SerializeField] private AudioMixerGroup musicMixerGroup;
    [SerializeField] private AudioMixerGroup sfxMixerGroup;
    [SerializeField] private string masterMixerVolumeName = "MasterVolume";
    [SerializeField] private string musicMixerVolumeName = "MusicVolume";
    [SerializeField] private string sfxMixerVolumeName = "SFXVolume";
    
    [Header("Music")] 
    [SerializeField] private AudioClip floor3Music;
    [SerializeField] private AudioClip floor2AndAHalfMusic;
    [SerializeField] private AudioClip floor2Music;
    //[SerializeField] private AudioClip mainMenuMusic;
    private AudioSource currentMusic;
    
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
    
    /// <summary>
    /// The function to set the volume of a mixer group, via exposed parameters
    /// </summary>
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

    /// <summary>
    /// Translates linear volume to decibels
    /// </summary>
    private float LinearVolumeToDB(float volume)
    {
        return Mathf.Log10(Mathf.Clamp(volume, 0.0001f, 1f)) * 20;
    }


    /// <summary>
    /// Returns a mixer group from a type of audio
    /// </summary>
    private AudioMixerGroup GetMixerGroupFromAudioType(AudioType type)
    {
        AudioMixerGroup mixerGroup = null;

        switch (type)
        {
            case AudioType.Master:
                mixerGroup = masterMixerGroup;
                break;
            case AudioType.Music:
                mixerGroup = musicMixerGroup;
                break;
            case AudioType.SFX:
                mixerGroup = sfxMixerGroup;
                break;
        }
        
        return mixerGroup;
    }
    
    
    /// <summary>
    /// Private helper function to create an audio source
    /// </summary>
    private AudioSource InstantiateAudioSource(AudioClip audioClip,AudioType type, float linearVolume, bool isLoop)
    {
        AudioSource audioSource = Instantiate(audioSourcePrefab).GetComponent<AudioSource>();
        audioSource.outputAudioMixerGroup = GetMixerGroupFromAudioType(type);
        audioSource.clip = audioClip;
        audioSource.volume = linearVolume;
        audioSource.loop = isLoop;
        return audioSource;
    }
    #endregion
    
    
    
    #region General Functions

    /// <summary>
    /// Plays an audio in 3D space at a position
    /// </summary>
    public AudioSource PlayAudio3D(AudioClip audioClip,AudioType type, Vector3 position, float linearVolume,bool isLooping)
    {
        AudioSource audioSource = InstantiateAudioSource(audioClip,type,linearVolume,isLooping);
        audioSource.spatialBlend = 1;
        audioSource.gameObject.transform.position = position;
        audioSource.Play();
        return audioSource;
    }


    /// <summary>
    /// Plays an audio in mono space
    /// </summary>
    public AudioSource PlayAudio2D(AudioClip audioClip,AudioType type, float linearVolume,bool isLooping)
    {
        AudioSource audioSource = InstantiateAudioSource(audioClip,type,linearVolume,isLooping);
        audioSource.spatialBlend = 0;
        audioSource.Play();
        return audioSource;
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
        currentMusic = PlayAudio2D(music,AudioType.Music,0f,true);
    }


    /// <summary>
    /// Resume the music player if paused
    /// </summary>
    public void ResumeMusic()
    {
        if (!currentMusic)
            return;

        if (currentMusic.isPlaying)
            return;
        
        currentMusic.UnPause();
    }


    /// <summary>
    /// Pause the currently playing music 
    /// </summary>
    public void PauseMusic()
    {
        if (!currentMusic)
            return;
        
        currentMusic.Pause();
    }
    

    /// <summary>
    /// Indefinitely Stop the current Music
    /// </summary>
    public void StopMusic()
    {
        if (!currentMusic)
            return;
        
        currentMusic.Stop();
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

using System;
using System.Collections.Generic;
using Audio;
using Events;
using Player;
using UnityEngine;
using UnityEngine.Audio;
using Random = UnityEngine.Random;

public class AudioManager : MonoBehaviour
{
    [HideInInspector] public static AudioManager Instance { get; private set; }


    [Header("Audio Mixers")] [SerializeField]
    private AudioMixer audioMixer;

    [SerializeField] private AudioMixerGroup masterMixerGroup;
    [SerializeField] private AudioMixerGroup musicMixerGroup;
    [SerializeField] private AudioMixerGroup sfxMixerGroup;
    [SerializeField] private string masterMixerVolumeName = "MasterVolume";
    [SerializeField] private string musicMixerVolumeName = "MusicVolume";
    [SerializeField] private string sfxMixerVolumeName = "SFXVolume";

    [Header("Music")] [SerializeField] private AudioClip floor3Music;
    [SerializeField] private AudioClip floor2AndAHalfMusic;

    [SerializeField] private AudioClip floor2Music;

    //[SerializeField] private AudioClip mainMenuMusic;
    private AudioSource currentMusic;

    [Header("SFX")] [SerializeField] private AudioClip questStarted;
    [SerializeField] private AudioClip questComplete;
    [SerializeField] private AudioClip questProgress;
    [SerializeField] private AudioClip levelUp;
    [SerializeField] private float minVolume;
    [SerializeField] private float maxVolume;
    [SerializeField] private float minPitch = 0.98f;
    [SerializeField] private float maxPitch = 1.02f;

    [Header("Prefabs")] [SerializeField] private GameObject audioSourcePrefab;


    void Awake()
    {
        if (!Instance)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    private void Start()
    {
        QuestManager.Instance.OnQuestStarted += quets => PlayAudio2D(questStarted, AudioType.SFX, false);
        QuestManager.Instance.OnQuestProgressCompleted += quets => PlayAudio2D(questComplete, AudioType.SFX, false);
        EventsManager.Instance.OnUseQuestStation += () => PlayAudio2D(questProgress, AudioType.SFX, false);
        PlayerSkills.Instance.OnSkillLevelChanged += skill => PlayAudio2D(levelUp, AudioType.SFX, false);
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
    private AudioSource InstantiateAudioSource(AudioClip audioClip, AudioType type, bool isLoop,
        float linearVolume = 1f, float pitch = 1f)
    {
        AudioSource audioSource = Instantiate(audioSourcePrefab).GetComponent<AudioSource>();
        audioSource.outputAudioMixerGroup = GetMixerGroupFromAudioType(type);
        audioSource.clip = audioClip;
        audioSource.loop = isLoop;
        audioSource.pitch = pitch;
        audioSource.volume = linearVolume;
        return audioSource;
    }

    #endregion


    #region General Functions

    /// <summary>
    /// Plays an audio in 3D space at a position
    /// </summary>
    public AudioSource PlayAudio3D(AudioClip audioClip, AudioType type, Vector3 position, bool isLooping,
        float linearVolume = 1f, float pitch = 1f)
    {
        AudioSource audioSource = InstantiateAudioSource(audioClip, type, isLooping, linearVolume, pitch);
        audioSource.spatialBlend = 1;
        audioSource.gameObject.transform.position = position;
        audioSource.Play();
        return audioSource;
    }


    /// <summary>
    /// Plays an audio in mono space
    /// </summary>
    public AudioSource PlayAudio2D(AudioClip audioClip, AudioType type, bool isLooping, float linearVolume = 1f,
        float pitch = 1f)
    {
        AudioSource audioSource = InstantiateAudioSource(audioClip, type, isLooping, linearVolume, pitch);
        audioSource.spatialBlend = 0;
        audioSource.Play();

        if (type == AudioType.SFX && !isLooping)
            Destroy(audioSource.gameObject, audioClip.length);

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

        currentMusic = PlayAudio2D(music, AudioType.Music, true, 1f);
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

    #region SFX Functions

    public void PlaySfxRandomly(List<AudioClip> audioClips, AudioSource audioSource)
    {
        if (audioClips != null && audioSource != null)
        {
            AudioClip clip = audioClips[Random.Range(0, audioClips.Count)];
            audioSource.clip = clip;
            audioSource.pitch = Random.Range(minPitch, maxPitch);
            audioSource.volume = Random.Range(minVolume, maxVolume);
            if (!audioSource.isPlaying)
                audioSource.PlayOneShot(clip);
        }
    }

    public enum SfxType
    {
        Footstep,
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
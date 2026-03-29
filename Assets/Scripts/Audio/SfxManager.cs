using System;
using System.Collections.Generic;
using Events;
using UnityEngine;

namespace Audio
{
    public class SfxManager : MonoBehaviour
    {
        [HideInInspector] public static SfxManager Instance { get; private set; }
        
        [SerializeField] private List<SfxClip> sfxClips;
        
        [SerializeField] private float minVolume;
        [SerializeField] private float maxVolume;
        [SerializeField] private float minPitch = 0.98f;
        [SerializeField] private float maxPitch = 1.02f;
        
        void Awake()
        {
            if (!Instance) {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
        }

        private void Start()
        {
            QuestManager.Instance.OnQuestStarted += quets =>  PlaySfx(SfxType.AcceptQuest);
            QuestManager.Instance.OnQuestProgressCompleted += quets =>  PlaySfx(SfxType.CompleteQuest);
            EventsManager.Instance.OnLevelUp += () =>  PlaySfx(SfxType.SkillLevelUp);
            EventsManager.Instance.OnUseQuestStation += () =>  PlaySfx(SfxType.UseStation);
        }

        public void PlaySfx(SfxType sfxType)
        {
            if (sfxClips != null)
            {
                SfxClip sfx = FindSfxByType(sfxType);

                if (sfx != null && sfx.audioClips.Count > 0 && sfx.sfxSource != null)
                {
                    AudioClip clip = sfx.audioClips[
                        UnityEngine.Random.Range(0, sfx.audioClips.Count)
                    ];

                    AudioSource source = sfx.sfxSource;

                    source.pitch = UnityEngine.Random.Range(minPitch, maxPitch);
                    source.volume = UnityEngine.Random.Range(minVolume, maxVolume);
                    source.PlayOneShot(clip);
                }
            }
        }

        private SfxClip FindSfxByType(SfxType sfxType)
        {
            return sfxClips.Find(sfxClip => sfxClip.type == sfxType);
        }
    }

    [Serializable]
    public class SfxClip
    {
        public AudioSource sfxSource;
        public List<AudioClip> audioClips;
        public SfxType type;
    }

    public enum SfxType
    {
        AcceptQuest,
        CompleteQuest,
        UseStation,
        SkillLevelUp,
        EnemyDeath
    }
}
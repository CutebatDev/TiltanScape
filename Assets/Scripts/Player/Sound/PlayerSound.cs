using System.Collections.Generic;
using UnityEngine;

public class PlayerSound : MonoBehaviour
{
    [SerializeField] private AudioSource footstepSource;
    [SerializeField] public List<AudioClip> footstepSound;

    public void PlayFootstep()
    {
        AudioManager.Instance.PlaySfxRandomly(footstepSound, footstepSource);
    }
}
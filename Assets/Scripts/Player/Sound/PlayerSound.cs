using System.Collections.Generic;
using UnityEngine;

public class PlayerSound : MonoBehaviour
{
    [SerializeField] private AudioSource footstepSource;
    [SerializeField] public List<AudioClip> footstepSound;

    void PlayFootstep()
    {
        AudioClip clip = footstepSound[Random.Range(0, footstepSound.Count)];
        footstepSource.PlayOneShot(clip);
    }
}

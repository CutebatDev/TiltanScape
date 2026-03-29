using System.Collections.Generic;
using UnityEngine;

public class PlayerSound : MonoBehaviour
{
    [SerializeField] private AudioSource footstepSource;
    [SerializeField] public List<AudioClip> footstepSound;
    [SerializeField] private float minVolume;
    [SerializeField] private float maxVolume;
    [SerializeField] private float minPitch = 0.98f;
    [SerializeField] private float maxPitch = 1.02f;

    public void PlayFootstep()
    {
        AudioClip clip = footstepSound[Random.Range(0, footstepSound.Count)];
        footstepSource.pitch = Random.Range(minPitch, maxPitch);
        footstepSource.volume = Random.Range(minVolume, maxVolume);
        footstepSource.PlayOneShot(clip);
    }
}

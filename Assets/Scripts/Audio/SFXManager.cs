using UnityEngine;

public class SFXManager : MonoBehaviour
{
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioClip enemyDeathClip;

    public void PlayEnemyDeathSFX()
    {
        if (enemyDeathClip != null && sfxSource != null)
        {
            sfxSource.PlayOneShot(enemyDeathClip);
        }
    }
}

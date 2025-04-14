using UnityEngine;

public class GunAudioManager : MonoBehaviour
{
    public AudioSource shootSound;
    public AudioSource reloadSound;
    public AudioClip outOfAmmoClip;
    [Range(0.8f, 1.2f)] public float pitchVariationMin = 0.95f;
    [Range(0.8f, 1.2f)] public float pitchVariationMax = 1.05f;

    public void PlayShoot()
    {
        if (shootSound)
        {
            shootSound.pitch = Random.Range(pitchVariationMin, pitchVariationMax);
            shootSound.Play();
        }
    }

    public void PlayReload()
    {
        if (reloadSound)
        {
            reloadSound.pitch = Random.Range(pitchVariationMin, pitchVariationMax);
            reloadSound.Play();
        }
    }

    public void PlayOutOfAmmo()
    {
        if (outOfAmmoClip && shootSound)
        {
            shootSound.PlayOneShot(outOfAmmoClip);
        }
    }
}

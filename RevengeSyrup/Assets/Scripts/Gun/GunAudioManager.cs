using UnityEngine;

[DisallowMultipleComponent]
public class GunAudioManager : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] private AudioSource shootSound;
    [SerializeField] private AudioSource reloadSound;
    [SerializeField] private AudioClip outOfAmmoClip;

    [Range(0.8f, 1.2f)]
    [SerializeField] private float pitchVariationMin = 0.95f;
    [Range(0.8f, 1.2f)]
    [SerializeField] private float pitchVariationMax = 1.05f;

    public void PlayShoot()
    {
        if (shootSound)
        {
            Debug.Log("Playing shoot sound");
            shootSound.pitch = Random.Range(pitchVariationMin, pitchVariationMax);
            shootSound.Play();
        }
        else
        {
            Debug.LogWarning("Shoot sound AudioSource is not assigned.");
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

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            shootSound.Play();
        }
    }
}

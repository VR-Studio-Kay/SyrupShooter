using UnityEngine;
using System.Collections.Generic;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class ExplosiveBarrel : MonoBehaviour
{
    [Header("Explosion Settings")]
    public float explosionRadius = 5f;
    public int explosionDamage = 50;
    public float force = 500f;
    public LayerMask damageLayers;

    [Header("Effects")]
    public ParticleSystem explosionEffect;
    public AudioSource audioSource; // Made public as per your request
    public AudioClip explosionSound;
    public GameObject debrisPrefab;

    private bool hasExploded = false;

    private void Awake()
    {
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>(); // Ensure AudioSource is attached if not manually set
        }
        audioSource.spatialBlend = 1f; // Enable 3D spatial audio
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (hasExploded) return;

        if (collision.gameObject.CompareTag("Bullet"))
        {
            Explode();
        }
    }

    private void Explode()
    {
        hasExploded = true;

        if (explosionEffect)
        {
            Instantiate(explosionEffect, transform.position, Quaternion.identity);
            Debug.Log("[ExplosiveBarrel] Explosion VFX triggered.");
        }

        if (explosionSound && audioSource)
        {
            audioSource.PlayOneShot(explosionSound);
            Debug.Log("[ExplosiveBarrel] Explosion sound played.");
        }

        Collider[] hits = Physics.OverlapSphere(transform.position, explosionRadius, damageLayers);
        Debug.Log($"[ExplosiveBarrel] Detected {hits.Length} objects in explosion radius.");

        foreach (Collider hit in hits)
        {
            if (hit.TryGetComponent(out EnemyHealth health))
            {
                health.TakeDamage(explosionDamage);
                Debug.Log($"[ExplosiveBarrel] Damaged {hit.name}");
            }

            if (hit.attachedRigidbody != null)
            {
                hit.attachedRigidbody.AddExplosionForce(force, transform.position, explosionRadius);
                Debug.Log($"[ExplosiveBarrel] Applied explosion force to {hit.name}");
            }
        }

        // Optional debris spawning
        if (debrisPrefab)
        {
            for (int i = 0; i < 5; i++)
            {
                GameObject piece = Instantiate(debrisPrefab, transform.position + Random.insideUnitSphere * 0.5f, Random.rotation);
                if (piece.TryGetComponent(out Rigidbody rb))
                {
                    rb.AddExplosionForce(force * 0.5f, transform.position, explosionRadius);
                }
                Destroy(piece, 3f); // Clean up debris after a short duration
            }
        }

        // VR Haptics and camera shake
        ApplyHapticFeedback();
        ApplyCameraShake();

        Destroy(gameObject, 0.5f); // Allow time for effects to play before destroying
    }

    private void ApplyHapticFeedback()
    {
        List<InputDevice> devices = new List<InputDevice>();
        InputDevices.GetDevicesWithCharacteristics(InputDeviceCharacteristics.HeldInHand | InputDeviceCharacteristics.Controller, devices);

        foreach (var device in devices)
        {
            if (device.TryGetHapticCapabilities(out HapticCapabilities capabilities) && capabilities.supportsImpulse)
            {
                device.SendHapticImpulse(0u, 0.8f, 0.5f); // High intensity for explosion
                Debug.Log("[ExplosiveBarrel] Sent haptic feedback.");
            }
        }
    }

    private void ApplyCameraShake()
    {
        Transform cam = Camera.main.transform;
        if (cam != null)
        {
            Vector3 originalPos = cam.position;
            cam.position += Random.insideUnitSphere * 0.1f; // Shake effect
            StartCoroutine(ResetCameraPosition(cam, originalPos, 0.1f)); // Gradual reset after shake
        }
    }

    private IEnumerator ResetCameraPosition(Transform cam, Vector3 originalPos, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            cam.position = Vector3.Lerp(cam.position, originalPos, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        cam.position = originalPos; // Ensure final position is correct
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}

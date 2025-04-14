using UnityEngine;
using System.Collections;

[DisallowMultipleComponent]
public class GunUIManager : MonoBehaviour
{
    [Header("Out of Ammo UI")]
    [SerializeField] private GameObject outOfAmmoCanvas;
    [SerializeField] private float blinkInterval = 0.2f;
    [SerializeField] private int blinkCount = 4;

    private Coroutine currentCoroutine;

    public void ShowOutOfAmmo()
    {
        if (outOfAmmoCanvas == null)
        {
            Debug.LogWarning("OutOfAmmoCanvas not assigned.");
            return;
        }

        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
        }

        currentCoroutine = StartCoroutine(BlinkOutOfAmmo());
    }

    private IEnumerator BlinkOutOfAmmo()
    {
        for (int i = 0; i < blinkCount; i++)
        {
            outOfAmmoCanvas.SetActive(true);
            yield return new WaitForSeconds(blinkInterval);
            outOfAmmoCanvas.SetActive(false);
            yield return new WaitForSeconds(blinkInterval);
        }

        outOfAmmoCanvas.SetActive(false);
    }
}

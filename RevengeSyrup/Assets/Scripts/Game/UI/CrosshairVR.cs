using UnityEngine;

public class CrosshairVR : MonoBehaviour
{
    [SerializeField] private GameObject crosshairCanvas; // Assign in Inspector

    public void ShowCrosshair(bool state)
    {
        if (crosshairCanvas != null)
            crosshairCanvas.SetActive(state);
    }
}
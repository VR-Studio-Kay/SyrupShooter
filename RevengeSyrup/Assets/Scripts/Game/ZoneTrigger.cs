using UnityEngine;

public class ZoneTrigger : MonoBehaviour
{
    [SerializeField] private ZoneManager zoneManager;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log($"[ZoneTrigger] Player entered zone: {gameObject.name}");
            zoneManager?.StartZone();
            gameObject.SetActive(false);
        }
    }
}

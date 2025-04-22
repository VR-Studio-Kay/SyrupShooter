using UnityEngine;

public class ZoneTrigger : MonoBehaviour
{
    public ZoneManager zone;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            zone.ActivateZone();
        }
    }
}

using UnityEngine;

public class PlayerAutoMover : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private Vector3 scrollOffset;
    [SerializeField] private float scrollSpeed = 2f;

    private bool shouldScroll = false;
    private Vector3 targetPosition;

    public void MoveToNextZone()
    {
        targetPosition = player.position + scrollOffset;
        shouldScroll = true;
        Debug.Log($"[AutoScroller] Starting auto-scroll to: {targetPosition}");
    }

    private void Update()
    {
        if (shouldScroll)
        {
            player.position = Vector3.MoveTowards(player.position, targetPosition, scrollSpeed * Time.deltaTime);

            if (Vector3.Distance(player.position, targetPosition) < 0.1f)
            {
                shouldScroll = false;
                Debug.Log("[AutoScroller] Auto-scroll complete. Player reached target.");
            }
        }
    }
}

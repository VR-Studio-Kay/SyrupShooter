using UnityEngine;

public class PlayerAutoMover : MonoBehaviour
{
    public float moveSpeed = 2f;
    private bool isMoving = true;
    private Transform currentTarget;

    public void SetTarget(Transform target)
    {
        currentTarget = target;
        isMoving = true;
    }

    public void StopMovement()
    {
        isMoving = false;
    }

    public void ResumeMovement()
    {
        isMoving = true;
    }

    void Update()
    {
        if (isMoving && currentTarget != null)
        {
            transform.position = Vector3.MoveTowards(transform.position, currentTarget.position, moveSpeed * Time.deltaTime);

            if (Vector3.Distance(transform.position, currentTarget.position) < 0.1f)
            {
                isMoving = false;
                currentTarget = null;
            }
        }
    }
}

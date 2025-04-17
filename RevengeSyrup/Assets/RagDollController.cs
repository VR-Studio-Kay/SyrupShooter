using UnityEngine;

public class RagdollController : MonoBehaviour
{
    [SerializeField] private Rigidbody[] ragdollBodies;
    [SerializeField] private Collider[] ragdollColliders;
    [SerializeField] private Animator animator;

    private void Awake()
    {
        ToggleRagdoll(false);
    }

    public void ToggleRagdoll(bool enabled)
    {
        foreach (var rb in ragdollBodies)
        {
            rb.isKinematic = !enabled;
        }

        foreach (var col in ragdollColliders)
        {
            col.enabled = enabled;
        }

        if (animator != null)
            animator.enabled = !enabled;
    }

    public void AddForce(Vector3 direction, float force)
    {
        foreach (var rb in ragdollBodies)
        {
            rb.AddForce(direction * force, ForceMode.Impulse);
        }
    }
}

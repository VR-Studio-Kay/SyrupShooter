using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int damage = 10;
    public float lifetime = 5f;

    private void Start()
    {
        Destroy(gameObject, lifetime); // Destroy bullet after a set time to prevent memory leaks
    }

    private void OnTriggerEnter(Collider other)
    {
        EnemyAiTutorial enemy = other.GetComponent<EnemyAiTutorial>();
        if (enemy != null)
        {
            enemy.TakeDamage(damage);
            enemy.ChangeColor(Color.yellow); // Change enemy color on hit
            Destroy(gameObject); // Destroy bullet on impact
        }
    }
}

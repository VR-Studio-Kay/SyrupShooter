using UnityEngine;
using System.Collections.Generic;

public class ZoneManager : MonoBehaviour
{
    [SerializeField] private List<EnemyAIController> enemies;
    [SerializeField] private PlayerAutoMover scroller;

    public void StartZone()
    {
        Debug.Log($"[ZoneManager] Zone started: {gameObject.name}");
        if (enemies.Count == 0)
        {
            Debug.Log("[ZoneManager] No enemies in zone — auto-scrolling now.");
            scroller?.MoveToNextZone();
        }
    }

    private void Start()
    {
        foreach (var enemy in enemies)
        {
            if (enemy != null)
            {
                enemy.OnEnemyKilled += CheckEnemiesLeft;
                Debug.Log($"[ZoneManager] Subscribed to enemy: {enemy.name}");
            }
        }
    }

    private void CheckEnemiesLeft()
    {
        enemies.RemoveAll(e => e == null);
        Debug.Log($"[ZoneManager] Enemy killed. {enemies.Count} remaining.");

        if (enemies.Count == 0)
        {
            Debug.Log("[ZoneManager] All enemies defeated — triggering auto-scroll.");
            scroller?.MoveToNextZone();
        }
    }

    private void OnDisable()
    {
        foreach (var enemy in enemies)
        {
            if (enemy != null)
                enemy.OnEnemyKilled -= CheckEnemiesLeft;
        }
    }
}

using UnityEngine;
using System.Collections.Generic;

public class ZoneManager : MonoBehaviour
{
    public List<GameObject> enemies;
    public Transform nextZone;
    public PlayerAutoMover autoMover;

    private bool isZoneActive = false;

    void Start()
    {
        foreach (var enemy in enemies)
        {
            if (enemy != null)
            {
                EnemyAIController ai = enemy.GetComponent<EnemyAIController>();
                if (ai != null)
                    ai.OnEnemyKilled += CheckEnemiesLeft;
            }
        }
    }
    public void ActivateZone()
    {
        isZoneActive = true;
        autoMover.StopMovement();
    }

    private void CheckEnemiesLeft()
    {
        enemies.RemoveAll(e => e == null);

        if (enemies.Count == 0)
        {
            autoMover.SetTarget(nextZone);
        }
    }
}

using System.Collections.Generic;
using UnityEngine;
using System.Collections;

/* 
    ------------------- DreamGSoft -------------------

    Hi, This is Goutamraj, Thank you for Checking the code! I hope you find it useful. Enjoy!

    --------------------------------------------------
 */

namespace DreamGSoft.Enemy
{
    public class EnemySpawner
    {
        private EnemyManager enemyManager;

        public EnemySpawner(EnemyManager enemyManager)
        {
            this.enemyManager = enemyManager;
        }

        private int spawnIdCounter = 0;
        private Dictionary<int, Coroutine> activeSpawns = new Dictionary<int, Coroutine>();

        // Call this to schedule a spawn
        public int ScheduleEnemySpawn(EnemyType type, Vector3 position, float delay)
        {
            int spawnId = spawnIdCounter++;
            Coroutine coroutine = enemyManager.StartCoroutine(SpawnEnemyWithDelay(spawnId, type, position, delay));
            activeSpawns.Add(spawnId, coroutine);
            return spawnId;
        }

        // Cancel specific spawn by ID
        public void CancelSpawn(int spawnId)
        {
            if (activeSpawns.TryGetValue(spawnId, out Coroutine coroutine))
            {
                enemyManager.StopCoroutine(coroutine);
                activeSpawns.Remove(spawnId);
            }
        }

        // Cancel all pending spawns
        public void CancelAllSpawns()
        {
            foreach (var coroutine in activeSpawns.Values)
            {
                enemyManager.StopCoroutine(coroutine);
            }
            activeSpawns.Clear();
        }

        // Coroutine to handle delayed spawn
        private IEnumerator SpawnEnemyWithDelay(int spawnId, EnemyType type, Vector3 position, float delay)
        {
            yield return new WaitForSeconds(delay);
            SpawnEnemy(type, position);
            activeSpawns.Remove(spawnId); // Clean up after spawn
        }

        private void SpawnEnemy(EnemyType type, Vector3 position)
        {
            //Telling EnemyFactory to spawn an enemy
            enemyManager.enemyFactory.CreateEnemy(type, position);
        }
    }
}

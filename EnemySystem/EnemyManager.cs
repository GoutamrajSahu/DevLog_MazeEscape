using DreamGSoft.Controller;
using DreamGSoft.ProceduralMazeGeneation;
using System.Collections.Generic;
using UnityEngine;

/* 
    ------------------- DreamGSoft -------------------

    Hi, This is Goutamraj, Thank you for Checking the code! I hope you find it useful. Enjoy!

    --------------------------------------------------
 */

namespace DreamGSoft.Enemy
{
    public class EnemyManager : MonoBehaviour
    {
        [SerializeField] public Transform enemiesHolder;

        [Header("EnemyPrefabs")]
        [SerializeField] public GameObject guardEnemy;
        [SerializeField] public GameObject dogEnemy;

        [Header("Class References")]
        public EnemySpawner enemySpawner;
        public EnemyFactory enemyFactory;

        private HashSet<IEnemy> activeEnemies = new HashSet<IEnemy>();

        private void Start()
        {
            enemySpawner = new EnemySpawner(this);
            enemyFactory = new EnemyFactory(this);
            GameController.OnSpawnEnemies += CreateSomeDummyEnemies;
            GameController.OnResult += OnResult;
        }

        private void OnDisable()
        {
            GameController.OnSpawnEnemies -= CreateSomeDummyEnemies;
            GameController.OnResult -= OnResult;
        }

        #region On Spawn Enemies
        public void CreateSomeDummyEnemies()
        {
           
            //Collecting cells to spawn Enemies
            List<MazeCell> cellsToSpawnEnemies = MazeGenerator.instance.GetCellsWhereEnemiesAreAllowedToSpawn();
            Debug.Log("CreateSomeDummyEnemies: cellsToSpawnEnemiesCount: "+ cellsToSpawnEnemies.Count); 
            foreach (MazeCell cell in cellsToSpawnEnemies)
            {
                enemySpawner.ScheduleEnemySpawn(EnemyType.Guard, cell.transform.position + new Vector3(0, 0.91f, 0), 0);
            }
        }
        #endregion

        #region On Result
        public void OnResult()
        {
            //things to be done once result is out.
            CancleAllOnGoingEnemySpawns();
            ClearAllEnemies();
        }

        //Cancling all Enemy spawn coroutines.
        public void CancleAllOnGoingEnemySpawns()
        {
            enemySpawner.CancelAllSpawns();
        }
        #endregion

        #region Enemy Registration Functionalities
        public void RegisterEnemy(IEnemy enemy)
        {
            if(enemy != null)
            {
                activeEnemies.Add(enemy);
            }
        }

        public void UnregisterEnemy(IEnemy enemy)
        {
            if(enemy != null)
            {
                activeEnemies.Remove(enemy);
            }
        }

        //Clearing all existing enemies in map.
        public void ClearAllEnemies()
        {
            List<IEnemy> tempList = new List<IEnemy>(activeEnemies); // to avoid list modification during loop.
            foreach (IEnemy enemy in tempList)
            {
                if(enemy != null)
                {
                    Destroy(enemy.gameObject); // need to return to pool once object pool implemented.
                }
            }
        }
        #endregion
    }
}

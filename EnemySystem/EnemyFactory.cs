using UnityEngine;

/* 
    ------------------- DreamGSoft -------------------

    Hi, This is Goutamraj, Thank you for Checking the code! I hope you find it useful. Enjoy!

    --------------------------------------------------
 */

namespace DreamGSoft.Enemy
{
    public class EnemyFactory
    {
        public EnemyManager enemyManager;

        public EnemyFactory(EnemyManager enemyManager)
        {
            this.enemyManager = enemyManager;
        }

        //CreateEnemy Function
        public IEnemy CreateEnemy(EnemyType enemyType, Vector3 position)
        {
            GameObject enemyObj = null;

            switch (enemyType)
            {
                case EnemyType.Guard:
                    enemyObj = EnemyManager.Instantiate(enemyManager.guardEnemy, position, Quaternion.identity, enemyManager.enemiesHolder);
                    break;
                case EnemyType.Dog:
                    enemyObj = EnemyManager.Instantiate(enemyManager.dogEnemy, position, Quaternion.identity, enemyManager.enemiesHolder);
                    break;
                case EnemyType.None:
                    break;
                default:
                    Debug.Log("Type Not Available");
                    break;
            }

            //Initializing Enemy
            IEnemy enemy = enemyObj.GetComponent<IEnemy>();
            if(enemy != null)
            {
                enemy.InitializeEnemy(enemyManager);
            }

            return enemy;
        }
    }
}

public enum EnemyType
{
    None,
    Guard,
    Dog,
    Common,
}

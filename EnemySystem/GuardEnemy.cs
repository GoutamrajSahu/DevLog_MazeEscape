using UnityEngine;
using UnityEngine.UI;

/* 
    ------------------- DreamGSoft -------------------

    Hi, This is Goutamraj, Thank you for Checking the code! I hope you find it useful. Enjoy!

    --------------------------------------------------
 */

namespace DreamGSoft.Enemy
{
    public class GuardEnemy : IEnemy
    {
        [SerializeField] Slider healthBar;

        public override void InjectManager(EnemyManager manager) //Note: Its not a recursion, as its calling base class InjectManager not this.
        {
            base.InjectManager(manager);
        }

        public override void InitializeEnemy(EnemyManager enemyManager)
        {
            type = data.enemyType;
            health = data.health;
            damagePow = data.damagePow;

            //Registering the Enemy to EnemyManager class to keep track of it.
            InjectManager(enemyManager);
            RegisterSelf();

            Debug.Log("Initialize GuardEnemy=>");
        }
         
        public override void TakeDamage(float damage)
        {
            Debug.Log("TakeDamage=> GuardEnemy");
            health -= damage;
            healthBar.value = health/data.health;

            if(health <= 0)
            {
                //Unregistering the Enemy from EnemyManager
                UnregisterSelf();

                //Destroying the enemy. Note: once objectPool implementation is done add the enemy back to pool instead of destroying it.
                Destroy(gameObject);
            }
        }
    }
}

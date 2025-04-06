using UnityEngine;

/* 
    ------------------- DreamGSoft -------------------

    Hi, This is Goutamraj, Thank you for Checking the code! I hope you find it useful. Enjoy!

    --------------------------------------------------
 */

namespace DreamGSoft.Enemy
{
    public abstract class IEnemy : MonoBehaviour
    {
        [SerializeField] public EnemyData data;
        [SerializeField] public EnemyType type;
        [SerializeField] public float health;
        [SerializeField] public float damagePow;

        protected EnemyManager enemyManager;

        //Use Note: InjectManager, RegisterSelf and UnregisterSelf if enemy need to register, else can ignore incase if its a minion enemy of Boss Enemy where the minions are handeled by boss.
        public virtual void InjectManager(EnemyManager manager)
        {
            this.enemyManager = manager;
        }
        protected void RegisterSelf()
        {
            enemyManager?.RegisterEnemy(this);
        }
        protected void UnregisterSelf()
        {
            enemyManager?.UnregisterEnemy(this);
        }

        public abstract void InitializeEnemy(EnemyManager manager);
        public abstract void TakeDamage(float damage);
    }
}

using UnityEngine;
using System.Collections.Generic;
using DreamGSoft.Enemy;
using UnityEngine.Pool;
/* 
    ------------------- DreamGSoft -------------------

    Hi, This is Goutamraj, Thank you for Checking the code! I hope you find it useful. Enjoy!

    --------------------------------------------------
 */
namespace DreamGSoft.Pool
{
    public class EnemyPool : MonoBehaviour
    {
        public static EnemyPool instance;
        [SerializeField] private Transform pool;

        // Dictionary to hold pools for each each type
        private Dictionary<EnemyType, ObjectPool<GameObject>> enemyPool = new Dictionary<EnemyType, ObjectPool<GameObject>>();

        private void Awake()
        {
            instance = this;
        }

        // Function to get a pool for a specific enemyPrefabRef type
        public GameObject GetEnemyObj(EnemyType enemyType, GameObject enemyPrefabRef) //<==enemyPrefabRef is required to create new pool dynamically, in case of unavailability.
        {
            //Check if the object pool havng the specific enemy ready or not, if not create the pool.
            if (!enemyPool.ContainsKey(enemyType))
            {
                //Create and add the pool to the dictionary.
                enemyPool.Add(enemyType, CreateNewPool(enemyPrefabRef));
            }

            // Retrieve a enemy from the pool
            return enemyPool[enemyType].Get();
        }

        // Function to return a enemyPrefabRef to the pool
        public void ReturnEnemyObj(EnemyType weaponType, GameObject enemyPrefabRef)
        {
            if (enemyPool.ContainsKey(weaponType))
            {
                enemyPool[weaponType].Release(enemyPrefabRef);
            }
            else
            {
                Destroy(enemyPrefabRef);// Fallback if the pool doesn't exist
            }
        }

        public ObjectPool<GameObject> CreateNewPool(GameObject enemyPrefabRef)
        {
            return new ObjectPool<GameObject>(
            createFunc: () => Instantiate(enemyPrefabRef),
            actionOnGet: OnGet,
            actionOnRelease: OnRelease,
            actionOnDestroy: OnDestroyPooledObject,
            collectionCheck: true,
            defaultCapacity: 10,
            maxSize: 300
            );
        }

        private void OnGet(GameObject obj)
        {
            //obj.SetActive(true);
        }

        private void OnRelease(GameObject obj)
        {
            //obj.SetActive(false);
            obj.transform.SetParent(pool);
        }

        private void OnDestroyPooledObject(GameObject obj)
        {
            Destroy(obj);
        }
    }
}

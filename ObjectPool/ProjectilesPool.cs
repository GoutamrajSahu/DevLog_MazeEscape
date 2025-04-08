using UnityEngine;
using System.Collections.Generic;
using DreamGSoft.Weapon;
using UnityEngine.Pool;

namespace DreamGSoft.Pool
{
    public class ProjectilesPool : MonoBehaviour
    {
        public static ProjectilesPool instance;
        [SerializeField] private Transform pool;

        // Dictionary to hold pools for each bullet type
        private Dictionary<WeaponType, ObjectPool<GameObject>> projectilePool = new Dictionary<WeaponType, ObjectPool<GameObject>>();

        private void Awake()
        {
            instance = this;
        }

        // Function to get a pool for a specific projectilePrefabRef type
        public GameObject GetProjectile(WeaponType weaponType, GameObject projectilePrefabRef) //<==projectilePrefabRef is required to create new pool dynamically, in case of unavailability.
        {
            //Check if the object pool havng the specific projectile ready or not, if not create the pool.
            if (!projectilePool.ContainsKey(weaponType))
            {
                //Create and add the pool to the dictionary.
                projectilePool.Add(weaponType, CreateNewPool(projectilePrefabRef));
            }

            // Retrieve a bullet from the pool
            return projectilePool[weaponType].Get();
        }

        // Function to return a projectilePrefabRef to the pool
        public void ReturnProjectile(WeaponType weaponType, GameObject projectilePrefabRef)
        {
            if (projectilePool.ContainsKey(weaponType))
            {
                projectilePool[weaponType].Release(projectilePrefabRef);
            }
            else
            {
                Destroy(projectilePrefabRef);// Fallback if the pool doesn't exist
            }
        }

        public ObjectPool<GameObject> CreateNewPool(GameObject projectilePrefabRef)
        {
            return new ObjectPool<GameObject>(
            createFunc: () => Instantiate(projectilePrefabRef),
            actionOnGet: OnGet,
            actionOnRelease: OnRelease,
            actionOnDestroy: OnDestroyPooledObject,
            collectionCheck: true,
            defaultCapacity: 10,
            maxSize: 100
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

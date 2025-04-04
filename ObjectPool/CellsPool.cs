using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Pool;

/* 
------------------- DreamGSoft -------------------

Thank you for checkingout the code! I hope you find it useful in your projects. Enjoy!

--------------------------------------------------
*/

namespace DreamGSoft.Pool
{
    public class CellsPool : MonoBehaviour
    {
        public static CellsPool instance;
        [SerializeField] private Transform pool;
        [SerializeField] GameObject cellPrefab;
        private ObjectPool<GameObject> cellsPool;
        private List<GameObject> activeCells = new List<GameObject>();

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }
            instance = this;
        }

        public void InitializePool()
        {
            cellsPool = new ObjectPool<GameObject>(
            createFunc: () => Instantiate(cellPrefab),
            actionOnGet: OnGet,
            actionOnRelease: OnRelease,
            actionOnDestroy: OnDestroyPooledObject,
            collectionCheck: true,
            defaultCapacity: 10,
            maxSize: 10000
            );
        }

        // Function to get a Cell
        public GameObject GetCell()
        {
            // Retrieve a cell from the pool
            return cellsPool.Get();
        }

        // Function to return a cell to the pool
        public void Return(GameObject cellRef)
        {
            cellsPool.Release(cellRef);
        }

        private void OnGet(GameObject obj)
        {
            obj.SetActive(true);
            activeCells.Add(obj);
        }

        private void OnRelease(GameObject obj)
        {
            //Resetting cells
            obj.GetComponent<ProceduralMazeGeneation.MazeCell>().ResetMe();
            obj.SetActive(false);
            obj.transform.SetParent(pool);
            activeCells.Remove(obj);
        }

        private void OnDestroyPooledObject(GameObject obj)
        {
            Destroy(obj);
        }

        public void ReturnAllTakkenObjectsBackToPool()
        {
            while(activeCells.Count > 0)
            {
                Return(activeCells[0]);
            }
        }
    }
}

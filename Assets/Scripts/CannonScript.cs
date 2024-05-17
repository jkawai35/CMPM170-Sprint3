using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonScript : MonoBehaviour
{
    public static CannonScript SharedInstance;
    private List<GameObject> pooledObjects = new List<GameObject>();
    //public GameObject objectToPool;
    public int amountToPool;

    public Transform FirePoint;
    public GameObject ProjectilePrefab;
    float shootInterval = 1f;
    float lastShoot = 0f;
    //shotsPool = new ObjectPool<ProjectilePrefab>(createFunc: () => new ProjectilePrefab("PooledShot"), actionOnGet: (obj) => obj.SetActive(true), actionOnRelease: (obj) => obj.SetActive(false), actionOnDestroy: (obj) => Destroy(obj), collectionChecks: false, defaultCapacity: 20, maxPoolSize: 20);
    // Start is called before the first frame update

    void Awake()
    {
        SharedInstance = this;
    }

    void Start()
    {
        GameObject tmp;
        Vector3 spawnPoint = new Vector3(0, 0, 0);
        for (int i = 0; i < amountToPool; i++)
        {
            tmp = Instantiate(ProjectilePrefab, spawnPoint, Quaternion.Euler(0, 0, 0), FirePoint);
            tmp.SetActive(false);
            pooledObjects.Add(tmp);
        }
    }
    
    public GameObject GetPooledObject()
    {
        for(int i = 0; i< amountToPool; i++)
        {
            if(!pooledObjects[i].activeInHierarchy)
            {
                return pooledObjects[i];
            }
        }
        return null;
    }

    /*IEnumerator ShootTimer()
    {

    }*/

    // Update is called once per frame
    void Update()
    {
        lastShoot += Time.deltaTime;
        if (lastShoot >= shootInterval)
        {
            Debug.Log("shooting");
            GameObject projectile = CannonScript.SharedInstance.GetPooledObject();
            if (projectile != null) {
                projectile.transform.position = FirePoint.position;
                projectile.transform.rotation = FirePoint.rotation;
                projectile.SetActive(true);
                lastShoot = 0f;
            }
            //Instantiate(ProjectilePrefab, FirePoint.position, FirePoint.rotation);
            //ProjectilePrefab newShot = pool.Get();
        }
    }
}

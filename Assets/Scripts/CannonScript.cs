using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonScript : MonoBehaviour
{
    public Transform FirePoint;
    public GameObject ProjectilePrefab;
    float shootInterval = 3f;
    float lastShoot = 0f;
    //shotsPool = new ObjectPool<ProjectilePrefab>(createFunc: () => new ProjectilePrefab("PooledShot"), actionOnGet: (obj) => obj.SetActive(true), actionOnRelease: (obj) => obj.SetActive(false), actionOnDestroy: (obj) => Destroy(obj), collectionChecks: false, defaultCapacity: 20, maxPoolSize: 20);
    // Start is called before the first frame update
    void Start()
    {
        
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
            Instantiate(ProjectilePrefab, FirePoint.position, FirePoint.rotation);
            //ProjectilePrefab newShot = pool.Get();
            lastShoot = 0f;
        }
    }
}

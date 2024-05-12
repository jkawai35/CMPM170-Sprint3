using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonScript : MonoBehaviour
{
    public Transform FirePoint;
    public GameObject ProjectilePrefab;
    float shootInterval = 3f;
    float lastShoot = 0f;
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
            lastShoot = 0f;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour
{
    public Transform FirePoint;
    public GameObject ProjectilePrefab;

    // Update is called once per frame
    void Update()
    {
        //timer
        {
            TurretShoot();
        }
        
    }

    void TurretShoot ()
    {
        Instantiate(ProjectilePrefab, FirePoint.position, FirePoint.rotation);
    }
}

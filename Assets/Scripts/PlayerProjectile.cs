using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerProjectile : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collider){ //If fireball hits enemy or ground, deactivate it
        this.GetComponent<Rigidbody2D>().velocity = Vector3.zero;
        if(collider.gameObject.layer == 7){ //If Layer == "Ground"
            this.gameObject.SetActive(false);
        } else if(collider.gameObject.layer == 9){ //If Layer == "Enemies"
            this.gameObject.SetActive(false);
        }
    }
}

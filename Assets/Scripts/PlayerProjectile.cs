using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerProjectile : MonoBehaviour
{
    void Update(){
        if(this.gameObject.transform.position.x >= 10 || this.gameObject.transform.position.x <= -10){ //If fireball goes off screen, deactivate and reset velocity to 0
            this.GetComponent<Rigidbody2D>().velocity = Vector3.zero;
            this.gameObject.SetActive(false);
        }
    }
    private void OnCollisionEnter2D(Collision2D collider){ //If fireball hits enemy or ground, deactivate it
        this.GetComponent<Rigidbody2D>().velocity = Vector3.zero;
        if(collider.gameObject.layer == 7){ //If Layer == "Ground"
            this.gameObject.SetActive(false);
        } 
        // else if(collider.gameObject.layer == "Enemy"){
        //     this.gameObject.SetActive(false);
        // }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public Rigidbody2D rb;
    public float speed;
    private float startPos;
    
    void Start(){
        startPos = this.gameObject.transform.position.x;
    }
    void Update(){
        rb.velocity = transform.right*speed*-1;
        if(this.gameObject.transform.position.x >= startPos+30 || this.gameObject.transform.position.x <= startPos-30){ //If fireball goes too far away from the cannon, deactivate and reset velocity to 0
            this.GetComponent<Rigidbody2D>().velocity = Vector3.zero;
            this.gameObject.SetActive(false);
        }
    }
    private void OnCollisionEnter2D(Collision2D collider){ //If fireball hits enemy or ground, deactivate it
        this.GetComponent<Rigidbody2D>().velocity = Vector3.zero;
        if(collider.gameObject.layer == 7){ //If Layer == "Ground"
            Debug.Log("cannon projectile hit ground");
            gameObject.SetActive(false);
        } /*else if(collider.gameObject.layer == 6){
            gameObject.SetActive(false);
        }*/
    }

}

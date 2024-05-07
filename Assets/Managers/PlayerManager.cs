using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private float playerSpeed, playerDrag;
    private float horizontalMovement, verticalMovement;
    private bool isGrounded;

    // Start is called before the first frame update
    void Start()
    {
        playerSpeed = 5f;
        verticalMovement = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        horizontalMovement = Input.GetAxis("Horizontal");
        verticalMovement = Input.GetAxis("Vertical");

        if(Mathf.Abs(horizontalMovement) > 0){
            rb.velocity = new Vector2(horizontalMovement * playerSpeed, rb.velocity.y);
        }

        if(Mathf.Abs(verticalMovement) > 0){
            rb.velocity = new Vector2(rb.velocity.x, verticalMovement * playerSpeed);
        }
    }

    void FixedUpdate(){
        checkIfGrounded();
        if(isGrounded){
            rb.velocity *= playerDrag;
        }
    }

    private void checkIfGrounded(){

    }
}

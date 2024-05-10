using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour //Script is based on Code Class - 2D Player Movement in Unity: https://www.youtube.com/watch?v=0-c3ErDzrh8
{

    enum PlayerState {Idle, Move, Jump} //The set of states for an FSM

    PlayerState currentState;
    private bool currentStateCompleted;

    [SerializeField] private Rigidbody2D rb;
    //[SerializeField] private Animator animator; FOR WHEN ANIMATIONS ARE ADDED
    [SerializeField] private float playerMoveSpeed, playerJumpSpeed, playerAcceleration;
    [Range(0f, 1f)] [SerializeField] private float playerDrag;
    [SerializeField] private BoxCollider2D groundCheckCollider;
    [SerializeField] private LayerMask groundMask;
    private float horizontalMovement, moveIncrement, totalHorizontalSpeed;
    private bool isGrounded;

    // Start is called before the first frame update
    void Start()
    {
        playerMoveSpeed = 5f;
        playerJumpSpeed = 7.5f;
        playerAcceleration = 1f;
        playerDrag = 0.9f;
    }

    // Update is called once per frame
    void Update()
    {
        getPlayerInput();
        if(Input.GetKey(KeyCode.J) && isGrounded){ //Player will jump if J is down
            playerJump();
        }

        if(currentStateCompleted == true){
            DetermineNewState();
        }
        UpdateState();
    }

    //Fixed Update is called at specific rates defined by the Unity editor
    void FixedUpdate(){
        checkIfGrounded();
        implementFriction();
        movePlayer();
    }

    private void DetermineNewState(){
        currentStateCompleted = false;
        if(isGrounded == true){ //Determining current state
            if(horizontalMovement == 0){
                currentState = PlayerState.Idle;
                StartIdleState(); //Calling Entry Function
            } else{
                currentState = PlayerState.Move;
                StartMoveState();
            }
        } else{
            currentState = PlayerState.Jump;
            StartJumpState();
        }
    }

    void UpdateState(){
        switch (currentState){
            case PlayerState.Idle:
                UpdateIdle();
                break;
            case PlayerState.Move:
                UpdateMove();
                break;
            case PlayerState.Jump:
                UpdateJump();
                break;
        }
    }

    private void StartIdleState(){
        //animator.Play("Idle"); FOR ANIMATOR
    }

    private void UpdateIdle(){
        if(horizontalMovement != 0){ //Exit conditions
            currentStateCompleted = true;
        }
    }

    private void StartMoveState(){
        //animator.Play("Move"); FOR ANIMATOR
    }

    private void UpdateMove(){
        if(horizontalMovement == 0 || !isGrounded){
            currentState = PlayerState.Idle;
            currentStateCompleted = true;
        }
    }

    private void StartJumpState(){
        //animator.Play("Jump"); FOR ANIMATOR
    }

    private void UpdateJump(){
        if(isGrounded){
            currentStateCompleted = true;
        }
    }

    private void getPlayerInput(){ //Adjusts horizontal movement force depending on player input
        if(Input.GetKey(KeyCode.R)){ //Player will move Right when R is pressed
            horizontalMovement = 1f;
        } else if(Input.GetKey(KeyCode.L)){ //Player will move Left when L is pressed
            horizontalMovement = -1f;
        } else{
            horizontalMovement = 0f;
        }
    }

    private void movePlayer(){
        if(horizontalMovement != 0){ //Apply movement if movement force != 0
            moveIncrement = horizontalMovement * playerAcceleration; //Applying acceleration if movement key is still being pressed
            totalHorizontalSpeed = Mathf.Clamp(rb.velocity.x + moveIncrement, -playerMoveSpeed, playerMoveSpeed);

            rb.velocity = new Vector2(totalHorizontalSpeed, rb.velocity.y);
        } else{ //Stop movement if no correct movement key is being pressed
            rb.velocity = new Vector2(0f, rb.velocity.y);
        }
    }

    private void playerJump(){ //Adding y-force to player
        rb.velocity = new Vector2(rb.velocity.x, playerJumpSpeed);
    }

    private void checkIfGrounded(){ //If ground collider is overlapping objects in Ground Layer, make is Grounded true
        isGrounded = Physics2D.OverlapAreaAll(groundCheckCollider.bounds.min, groundCheckCollider.bounds.max, groundMask).Length > 0;
    }

    private void implementFriction(){ //Adding Friction to player if touching ground and no movement is being inputed
        if(isGrounded && horizontalMovement == 0 && rb.velocity.y <= 0){
            rb.velocity *= playerDrag;
        }
    }
}

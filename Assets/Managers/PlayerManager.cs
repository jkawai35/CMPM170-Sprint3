using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Script is based on Code Class - 2D Player Movement in Unity: https://www.youtube.com/watch?v=0-c3ErDzrh8
//And Code Class - Build your own State Machines!: https://www.youtube.com/watch?v=-jkT4oFi1vk
public class PlayerManager : MonoBehaviour 
{
    //PLAYER MANAGER VARIABLES
    enum PlayerState {Idle, Move, Jump, Fire, Dash} //The set of states for an FSM
    PlayerState currentState;
    private bool currentStateCompleted; //Boolean value to determine when to start new state

    [SerializeField] private Rigidbody2D rb;
    //[SerializeField] private Animator animator; FOR WHEN ANIMATIONS ARE ADDED
    [SerializeField] private float playerMoveSpeed, playerJumpSpeed, playerAcceleration;
    [Range(0f, 1f)] [SerializeField] private float playerDrag;
    [SerializeField] private BoxCollider2D groundCheckCollider;
    [SerializeField] private LayerMask groundMask;
    private float horizontalMovement, moveIncrement, totalHorizontalSpeed, firingCounter, dashingCounter;
    private bool isGrounded, isFiring, isDashing, dashReady;
    private string direction;

    //INITIAL FUNCTIONS
    void Start()
    {
        playerMoveSpeed = 5f;
        playerJumpSpeed = 7.5f;
        playerAcceleration = 1f;
        playerDrag = 0.9f;
        
        direction = "right";
        isFiring = false; 
        isDashing = false;
        dashReady = true;
    }

    void Update()
    {
        if(isDashing != true){ //Checks if currently dashing so that player doesn't flip during dash
            getPlayerInput(); //Determine Player Left-Right Movement Input
        }
        if(Input.GetKey(KeyCode.J) && isGrounded){ //Player will jump if J is down
            playerJump();
        }
        if(Input.GetKeyDown(KeyCode.F) && isFiring == false){ //Fire projectile if player presses F
            playerFire();
        }
        if(Input.GetKeyDown(KeyCode.D) && isDashing == false && dashReady == true){ //Dash if player presses D
            playerDash();
        }

        if(currentStateCompleted == true){ //If state is complete, switch to next state
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


    //STATE MACHINE FUNCTIONS
    private void DetermineNewState(){ //Check each possible state and switch to them based on current stats
        currentStateCompleted = false;
        
        //Determining current state
        if(isFiring == true){
            currentState = PlayerState.Fire;
            StartFireState();
        } else if(isDashing == true){
            currentState = PlayerState.Dash;
            StartDashState();
        } else if(isGrounded == true){
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
        switch (currentState){ //Switch runs different things depending on the currentState variable
            case PlayerState.Idle:
                UpdateIdle();
                break;
            case PlayerState.Move:
                UpdateMove();
                break;
            case PlayerState.Jump:
                UpdateJump();
                break;
            case PlayerState.Fire:
                UpdateFire();
                break;
            case PlayerState.Dash:
                UpdateDash();
                break;
        }
    }

    //INDIVIDUAL STATE FUNCTIONS
    private void StartIdleState(){
        Debug.Log("Idle State");
        //animator.Play("Idle"); FOR ANIMATOR
    }

    private void UpdateIdle(){
        if(horizontalMovement != 0 || isGrounded == false || isFiring == true || isDashing == true){ //Exit conditions
            currentStateCompleted = true;
        }
    }

    private void StartMoveState(){
        Debug.Log("Move State");
        //animator.Play("Move"); FOR ANIMATOR
    }

    private void UpdateMove(){
        float xVelocity = rb.velocity.x; //To enable staying within Move State until velocity == 0f
        //animator.speed = Mathf.Abs(xVelocity)/5f;

        if(Mathf.Abs(xVelocity) < 0.1f || isGrounded == false || isFiring == true || isDashing == true){
            //currentState = PlayerState.Idle;
            currentStateCompleted = true;
        }
    }

    private void StartJumpState(){
        Debug.Log("Jump State");
        //animator.Play("Jump"); FOR ANIMATOR
    }

    private void UpdateJump(){
        if(isGrounded == true || isFiring == true || isDashing == true){
            currentStateCompleted = true;
        }
    }

    private void StartFireState(){
        Debug.Log("Fire State");
        //animator.Play("Fire"); FOR ANIMATOR
        firingCounter = 0f;
        Debug.Log("Fire State Start");
    }

    private void UpdateFire(){
        firingCounter += Time.deltaTime;
        if(firingCounter >= 1.0f){ //End fire state after animation complete
            isFiring = false;
            Debug.Log("Fire State End");
            currentStateCompleted = true;
        }
    }

    private void StartDashState(){
        Debug.Log("Dash State");
        //animator.Play("Dash"); FOR ANIMATOR
        dashingCounter = 0f;
        dashReady = false;
    }

    private void UpdateDash(){
        dashingCounter += Time.deltaTime;
        if(dashingCounter >= 0.25f){
            StartCoroutine(DashCooldown());
            isDashing = false;
            horizontalMovement = 0f;
            currentStateCompleted = true;
        }
    }

    //MOVEMENT & INPUT FUNCTIONS
    private void getPlayerInput(){ //Adjusts horizontal movement force depending on player input
        if(Input.GetKey(KeyCode.R)){ //Player will move Right when R is pressed
            direction = "right";
            horizontalMovement = 1f;
        } else if(Input.GetKey(KeyCode.L)){ //Player will move Left when L is pressed
            direction = "left";
            horizontalMovement = -1f;
        } else{
            horizontalMovement = 0f;
        }
    }

    private void movePlayer(){
        if((horizontalMovement != 0 && currentState == PlayerState.Dash)){ //Apply dash movement
            rb.velocity = new Vector2(horizontalMovement, 0f);
        }else if(horizontalMovement != 0 && currentState != PlayerState.Dash){ //Apply movement if movement force != 0 and not currently dashing
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

    private void playerFire(){
        isFiring = true;
        Debug.Log("fire" + direction); //For Debugging
    }

    private void playerDash(){
        isDashing = true;
        if(direction == "right"){
            horizontalMovement = 10f;
        } else{ //direction == "left"
            horizontalMovement = -10f;
        }
    }

    IEnumerator DashCooldown(){
        yield return new WaitForSeconds(1f); //Cooldown is 1 second
        dashReady = true;
    }
}

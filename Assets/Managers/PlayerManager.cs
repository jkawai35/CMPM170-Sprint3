using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Script is based on Code Class - 2D Player Movement in Unity: https://www.youtube.com/watch?v=0-c3ErDzrh8
//And Code Class - Build your own State Machines!: https://www.youtube.com/watch?v=-jkT4oFi1vk
public class PlayerManager : MonoBehaviour 
{
    //PLAYER MANAGER VARIABLES
    enum PlayerState {Idle, Move, Jump, Fire, Dash, Block, Hurt} //The set of states for an FSM
    PlayerState currentState;
    private bool currentStateCompleted; //Boolean value to determine when to start new state

    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Animator animator; //FOR WHEN ANIMATIONS ARE ADDED
    [SerializeField] private float playerMoveSpeed, playerJumpSpeed, playerAcceleration;
    [Range(0f, 1f)] [SerializeField] private float playerDrag;
    [SerializeField] private BoxCollider2D groundCheckCollider;
    [SerializeField] private LayerMask groundMask;
    private float horizontalMovement, moveIncrement, totalHorizontalSpeed, firingCounter, dashingCounter, blockCounter, hurtCounter;
    private bool isGrounded, isFiring, isDashing, dashReady, canDoubleJump, isBlocking, isHurt;
    private string direction;
    private int playerHealth;

    //INITIAL FUNCTIONS
    void Start()
    {
        playerMoveSpeed = 5f;
        playerJumpSpeed = 7.8f;
        playerAcceleration = 1f;
        playerDrag = 0.9f;
        
        playerHealth = 3;
        direction = "right";
        isFiring = false; 
        isDashing = false;
        dashReady = true;
        canDoubleJump = false;
        isBlocking = false;
        isHurt = false;
    }

    void Update()
    {
        if(isHurt == false && playerHealth > 0){ //Player Inputs will only work if the player currently isn't hurt and still has life
            if(isDashing != true){ //Checks if currently dashing so that player doesn't flip during dash
                getPlayerInput(); //Determine Player Left-Right Movement Input
            }
            if(Input.GetKey(KeyCode.J) && isGrounded){ //Player will jump if J is down
                playerJump();
            }
            if(Input.GetKeyDown(KeyCode.F) && isFiring == false && isDashing == false && isBlocking == false){ //Fire projectile if player presses F
                playerFire();
            }
            if(Input.GetKeyDown(KeyCode.D) && isDashing == false && dashReady == true  && isFiring == false && isBlocking == false){ //Dash if player presses D
                playerDash();
            } 
            if(Input.GetKeyDown(KeyCode.B) && isBlocking == false && isFiring == false && isDashing == false){ //Block if player presses B
                playerBlock();
            }
        }

        if(Input.GetKeyDown(KeyCode.H)){ //TEMP FUNCTION TO TEST HURT STATE
            playerHurt();
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
        updateSpriteDirection();
    }


    //STATE MACHINE FUNCTIONS
    private void DetermineNewState(){ //Check each possible state and switch to them based on current stats
        currentStateCompleted = false; //Resetting to false to enable accessibility to next state
        
        //Determining current state
        if(isHurt == true){
            currentState = PlayerState.Hurt;
            StartHurtState();
        } else if(isFiring == true){
            currentState = PlayerState.Fire;
            StartFireState();
        } else if(isDashing == true){
            currentState = PlayerState.Dash;
            StartDashState();
        } else if(isBlocking == true){
            currentState = PlayerState.Block;
            StartBlockState();
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
            case PlayerState.Block:
                UpdateBlock();
                break;
            case PlayerState.Hurt:
                UpdateHurt();
                break;
        }
    }

    //INDIVIDUAL STATE FUNCTIONS
    private void StartIdleState(){
        Debug.Log("Idle State");
        animator.Play("Player_Idle");
    }

    private void UpdateIdle(){
        if(horizontalMovement != 0 || isGrounded == false || isFiring == true || isDashing == true || isBlocking == true){ //Exit conditions
            currentStateCompleted = true;
        }
    }

    private void StartMoveState(){
        Debug.Log("Move State");
        animator.Play("Player_Move");
    }

    private void UpdateMove(){
        float xVelocity = rb.velocity.x; //To enable staying within Move State until velocity == 0f
        //animator.speed = Mathf.Abs(xVelocity)/5f;

        if(Mathf.Abs(xVelocity) < 0.1f || isGrounded == false || isFiring == true || isDashing == true || isBlocking == true){
            //currentState = PlayerState.Idle;
            currentStateCompleted = true;
        }
    }

    private void StartJumpState(){
        Debug.Log("Jump State");
        animator.Play("Player_Dash"); //DASH AND JUMP SHARE SAME ANIMATION
    }

    private void UpdateJump(){
        if(Input.GetKeyDown(KeyCode.X) && canDoubleJump == true){ //Jump again if X is pressed while currently jumping
            playerJump();
            canDoubleJump = false;
        }
        if(isGrounded == true || isFiring == true || isDashing == true || isBlocking == true){
            if(isGrounded == true){ //Only reset double jump ability if ground touched
                canDoubleJump = true;
            }
            currentStateCompleted = true;
        }
    }

    private void StartFireState(){
        Debug.Log("Fire State");
        animator.Play("Player_Attack");
        firingCounter = 0f;
        Debug.Log("Fire State Start");
    }

    private void UpdateFire(){
        firingCounter += Time.deltaTime;
        if(firingCounter >= 0.1f){ //End fire state after animation complete
            isFiring = false;
            Debug.Log("Fire State End");
            currentStateCompleted = true;
        }
    }

    private void StartDashState(){
        Debug.Log("Dash State");
        animator.Play("Player_Dash"); //FOR ANIMATOR
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

    private void StartBlockState(){
        Debug.Log("Block State");
        animator.Play("Player_Block");
        blockCounter = 0f;
        Debug.Log("Blocking Start");
    }

    private void UpdateBlock(){
        blockCounter += Time.deltaTime;
        if(blockCounter >= 1.0f){ //End block state after 1 second
            Debug.Log("Blocking End");
            isBlocking = false;
            currentStateCompleted = true;
        }
    }

    private void StartHurtState(){
        Debug.Log("Hurt State");
        animator.Play("Player_Hurt");
        hurtCounter = 0f;
        Debug.Log("Hurt Start");
    }

    private void UpdateHurt(){
        hurtCounter += Time.deltaTime;
        if(hurtCounter >= 1.0f){
            Debug.Log("Hurt End");
            isHurt = false;
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

    private void updateSpriteDirection(){ //Flip Sprite to face left or right depending on current direction value
        if(direction == "right"){
            transform.localScale = new Vector2(1, 1);
        } else{ //direction == "left"
            transform.localScale = new Vector2(-1, 1);
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
        AmmoManager.instance.Fire(direction); //Calling AmmoManager Instance to Fire
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
        yield return new WaitForSeconds(1.5f); //Cooldown is 1.5 second
        dashReady = true;
    }

    private void playerBlock(){
        isBlocking = true;
    }

    private void playerHurt(){ //TEMP FUNCTION CAN BE REPLACED WITH ONCOLLISIONENTER
        playerHealth -= 1; //Reduce health by 1
        horizontalMovement = 0f; //Stop all horizontal movement
        isHurt = true; //Enable switching to hurt state
        currentStateCompleted = true;
    }
}

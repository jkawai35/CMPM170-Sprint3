using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collider){
        if(collider.gameObject.layer == 6){ //If player reaches the door, end game and set playerWin to true
            GameManager.instance.gameOver(true);
        }
    }
}

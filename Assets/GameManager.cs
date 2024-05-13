using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    private bool isGameOver, playerWin;

    void Awake(){
        if(instance == null){
            instance = this;
        } else{
            Destroy(this);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        isGameOver = false;
        playerWin = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool getIsGameOver(){
        return isGameOver;
    }

    public bool getPlayerWin(){
        return playerWin;
    }

    public void gameOver(bool playerWinStatus){
        isGameOver = true;
        if(playerWinStatus == true){
            playerWin = true;
        }
    }
}

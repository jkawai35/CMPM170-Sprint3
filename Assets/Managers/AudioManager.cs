using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    [SerializeField] private AudioSource playerAudioSource;
    [SerializeField] private AudioClip jumpSound, hurtSound, fireSound, dashSound, shieldSound;

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
        
    }

    public void playPlayerSound(string chosenAudio){
        if(chosenAudio == "jump"){
            playerAudioSource.clip = jumpSound;
        } else if(chosenAudio == "hurt"){
            playerAudioSource.clip = hurtSound;
        } else if(chosenAudio == "fire"){
            playerAudioSource.clip = fireSound;
        } else if(chosenAudio == "dash"){
            playerAudioSource.clip = dashSound;
        } else if(chosenAudio == "shield"){
            playerAudioSource.clip = shieldSound;
        }
        playerAudioSource.Play();
    }
}

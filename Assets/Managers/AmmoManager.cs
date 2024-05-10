using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoManager : MonoBehaviour
{
    public static AmmoManager instance;
    private List<GameObject> fireballList = new List<GameObject>();
    [SerializeField] private GameObject fireballObj;
    [SerializeField] private Transform fireballParent;
    [SerializeField] private float fireballSpeed = 20f;
    [SerializeField] private Transform fireballSpawnpoint;
    private float directionMultiple = 1f;

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
        for(int i = 0; i < 10; i++){ //Instantiating Fireballs into Scene
            Vector3 spawnPoint = new Vector3(0, 0, 0);
            GameObject fireball = Instantiate(fireballObj, spawnPoint, Quaternion.Euler(0, 0, 0), fireballParent);
            fireball.SetActive(false);
            fireballList.Add(fireball);
        }
    }

    public void Fire(string direction){
        GameObject chosenFireball = getFireball();
        if(chosenFireball != null){
            chosenFireball.transform.position = fireballSpawnpoint.position; //Changing position of Fireball into Scene
            chosenFireball.SetActive(true); //Setting to Active
            if(direction == "right"){
                directionMultiple = 1f;
            } else{
                directionMultiple = -1f;
            }
            chosenFireball.GetComponent<Rigidbody2D>().velocity = new Vector2(directionMultiple * fireballSpeed, 0f); //Sending Fireball in Direction
        }
    }

    private GameObject getFireball(){ //Finding first inactive Fireball Object in Scene and Returning it
        for(int i = 0; i < fireballList.Count; i++){
            if(!fireballList[i].activeInHierarchy){
                return fireballList[i];
            }
        }
        return null;
    }
}

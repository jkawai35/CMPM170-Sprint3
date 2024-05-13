using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    //Based on How to Make Camera Follow In UNITY 2D - https://www.youtube.com/watch?v=FXqwunFQuao
    //and Unity 2D Platformer Tutorial 8 - How To Create 2D Camera Bounds - https://www.youtube.com/watch?v=Fqht4gyqFbo
    [SerializeField] private float followSpeed = 2f;
    [SerializeField] private Transform target;
    [SerializeField] private float xOffset, yOffset; //To add offset from player
    [SerializeField] private Vector2 minPosition, maxPosition; //For Camera Bounds

    // Update is called once per frame
    void FixedUpdate()
    {
        //Acquring new position for camera depending on target location
        Vector3 newPosition = new Vector3(target.position.x + xOffset, target.position.y + yOffset, -10f); //-10f because that is the default position of Cameras
        
        //Checking if new position is within bounds
        Vector3 boundPosition = new Vector3(Mathf.Clamp(newPosition.x, minPosition.x, maxPosition.x), Mathf.Clamp(newPosition.y, minPosition.y, maxPosition.y), -10f); //Mathf.Clamp checks first argument and ensures if it is within min and max
        
        //Updating position
        transform.position = Vector3.Slerp(transform.position, boundPosition, followSpeed * Time.deltaTime); //Follow target position
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private float followSpeed = 2f;
    [SerializeField] private Transform target;
    [SerializeField] private float xOffset, yOffset; //To add offset from player

    // Update is called once per frame
    void Update()
    {
        Vector3 newPosition = new Vector3(target.position.x + xOffset, target.position.y + yOffset, -10f); //-10f because that is the default position of Cameras
        transform.position = Vector3.Slerp(transform.position, newPosition, followSpeed * Time.deltaTime);
    }
}

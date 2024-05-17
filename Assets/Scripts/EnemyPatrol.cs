using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPatrol : MonoBehaviour
{
    public GameObject pointA;
    public GameObject pointB;
    private Rigidbody2D rb;
    private Transform currentPoint;
    public float speed;
    // Start is called before the first frame update
    public bool bumpCooldown = false;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentPoint = pointB.transform;
        rb.velocity = new Vector2(speed, 0);
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 point = currentPoint.position - transform.position;
        /* if (currentPoint == pointB.transform)
        {
            rb.velocity = new Vector2(speed, 0);
        }
        else
        {
            rb.velocity = new Vector2(-speed, 0);
        } */

        if(Vector2.Distance(transform.position, currentPoint.position) < 0.5f && currentPoint == pointB.transform)
        {
            Debug.Log("moving towards point A");
            flip();
            currentPoint = pointA.transform;
        }
        else if(Vector2.Distance(transform.position, currentPoint.position) < 0.5f && currentPoint == pointA.transform)
        {
            Debug.Log("moving towards point B");
            flip();
            currentPoint = pointB.transform;
        }
    }

    private void OnCollisionEnter2D(Collision2D collider)
    {
        Debug.Log("enemy collided");
        flip();
        if (currentPoint == pointA.transform)
        {
            currentPoint = pointB.transform;
        } else {
            currentPoint = pointA.transform;
        }
        /*if (bumpCooldown == false)
        {
            Debug.Log("turning around");
            StartCoroutine(bump());
        }*/
    }

    /*IEnumerator bump()
    {
        Debug.Log("starting bump coroutine");
        flip();
        bumpCooldown = true;
        yield return new WaitForSeconds(1);
        bumpCooldown = false;
        Debug.Log("bump cooldown over");
    }*/

    void flip()
    {
        Debug.Log("flipping");
        Vector3 localScale = transform.localScale;
        localScale.x *= -1;
        transform.localScale = localScale;
        rb.velocity = new Vector2 (rb.velocity.x*-1, 0);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(pointA.transform.position, 0.5f);
        Gizmos.DrawWireSphere(pointB.transform.position, 0.5f);
        Gizmos.DrawLine(pointA.transform.position, pointB.transform.position);
    }
}

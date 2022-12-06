using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehavior : MonoBehaviour
{
    public GameObject playerRef;

    // Enemy Movement
    public float speed;
    public float stoppingDistance;
    public float retreatDistance;

    private Rigidbody rb;

    // Enemy Field Of View
    public float radius;
    [Range(0,360)]
    public float angle;

    public LayerMask targetMask;
    public LayerMask obstacleMask;

    public bool canSeePlayer;

    private void Start()
    {
        playerRef = GameObject.FindGameObjectWithTag("Player");
        StartCoroutine(FOVRoutine());

        rb = this.GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if (canSeePlayer)
        {
            Vector3 lookVector = playerRef.transform.position - transform.position;
            lookVector.y = transform.position.y;
            Quaternion rot = Quaternion.LookRotation(lookVector);
            transform.rotation = Quaternion.Slerp(transform.rotation, rot, 1);

            StartCoroutine(EnemyMovement());
        }
        

    }

    private IEnumerator FOVRoutine()
    {
        float delay = .2f;
        WaitForSeconds wait = new WaitForSeconds(delay);

        while (true)
        {
            yield return wait;
            FOVCheck();
        }
    }


    private void FOVCheck()
    {
        Collider[] rangeChecks = Physics.OverlapSphere(transform.position, radius, targetMask);

        if(rangeChecks.Length != 0)
        {
            Transform target = rangeChecks[0].transform;
            Vector3 directionToTarget = (target.position - transform.position).normalized;

            if (Vector3.Angle(transform.forward, directionToTarget) < angle / 2)
            {
                float distanceToTarget = Vector3.Distance(transform.position, target.position);

                if(!Physics.Raycast(transform.position, directionToTarget, distanceToTarget, obstacleMask))
                {
                    canSeePlayer = true;
                } else
                {
                    canSeePlayer = false;
                }

            } else
            {
                canSeePlayer = false;
            }
        } else if (canSeePlayer)
        {
            canSeePlayer = false;
        }
    }

    private IEnumerator EnemyMovement()
    {
        float delay = .5f;
        WaitForSeconds wait = new WaitForSeconds(delay);

        if (Vector3.Distance(transform.position, playerRef.transform.position) > stoppingDistance)
        {
            yield return wait;
            transform.position = Vector3.MoveTowards(transform.position, playerRef.transform.position, speed * Time.deltaTime);
        }
        else if (Vector3.Distance(transform.position, playerRef.transform.position) < stoppingDistance && Vector3.Distance(transform.position, playerRef.transform.position) > retreatDistance)
        {
            yield return wait;
            transform.position = this.transform.position;
        }
        else if (Vector3.Distance(transform.position, playerRef.transform.position) < retreatDistance)
        {
            yield return wait;
            transform.position = Vector3.MoveTowards(transform.position, playerRef.transform.position, -speed * Time.deltaTime);
        }

    }


    

   
}

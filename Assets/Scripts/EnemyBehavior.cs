using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehavior : MonoBehaviour
{
    public GameObject playerRef;

    [Header("Enemy Movement Settings")]
    public float speed;
    public float stoppingDistance;
    public float retreatDistance;

    private Rigidbody rb;

    [Header("Enemy Field Of View Settings")]
    [Range(0, 100)] //Adjust the range/slider based on the size of your character
    public float radius;
    [Range(0,360)]
    public float angle;

    [Header("Enemy Responding Time Settings")]
    [Range(0, 1)] public float chasingDelay;
    [Range(0, 1)] public float stoppingDelay;
    [Range(0, 1)] public float retreatDelay;

    [Header("Layer Masks")]
    [Tooltip("Set Target Mask for your Player Character in order for Enemy to detect")]
    public LayerMask targetMask; 
    [Tooltip("Set Obstacle Mask for Obstacle Object in order to block Enemy FOV")]
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
  
        WaitForSeconds chasingWaitTime = new WaitForSeconds(chasingDelay);
        WaitForSeconds stoppingWaitTime = new WaitForSeconds(stoppingDelay);
        WaitForSeconds retreatWaitTime = new WaitForSeconds(retreatDelay);

        if (Vector3.Distance(transform.position, playerRef.transform.position) > stoppingDistance)
        {
            yield return chasingWaitTime;
            transform.position = Vector3.MoveTowards(transform.position, playerRef.transform.position, speed * Time.deltaTime);
        }
        else if (Vector3.Distance(transform.position, playerRef.transform.position) < stoppingDistance && Vector3.Distance(transform.position, playerRef.transform.position) > retreatDistance)
        {
            yield return stoppingWaitTime;
            transform.position = this.transform.position;
        }
        else if (Vector3.Distance(transform.position, playerRef.transform.position) < retreatDistance)
        {
            yield return retreatWaitTime;
            transform.position = Vector3.MoveTowards(transform.position, playerRef.transform.position, -speed * Time.deltaTime);
        }

    }


    

   
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuardBehavior : MonoBehaviour
{
    public Light spotlight;
    public float viewDistance;
    public Transform waypointHolder;
    public float speed = 4f;
    public float waitTime = 3f;
    public float turnSpeed = 90f;
    public LayerMask viewMask;

    private float viewAngle;
    private Transform player;
    private Color originalSpotlightColor;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        originalSpotlightColor = spotlight.color;

        viewAngle = spotlight.spotAngle;
        Vector3[] waypoints = new Vector3[waypointHolder.childCount];
        for ( int i = 0; i < waypoints.Length; i++)
        {
            waypoints[i] = waypointHolder.GetChild(i).position;
            //sets the waypoint height equal to the guards height, making it so the guard won't randomly move up and down if waypoints are different heights
            waypoints[i] = new Vector3(waypoints[i].x, transform.position.y, waypoints[i].z);
        }
        StartCoroutine(Patrol(waypoints));
        
    }

    private void Update()
    {
        if (CanSeePlayer()) 
        {
            spotlight.color = Color.red;
        }
        else
        {
            spotlight.color = originalSpotlightColor;
        }
    }

    /// <summary>
    /// Uses gizmos to help set up guard patrolling paths in the editor
    /// </summary>
    private void OnDrawGizmos()
    {
        Vector3 startPosition = waypointHolder.GetChild(0).position;
        Vector3 previousPosition = startPosition;

        foreach (Transform waypoint in waypointHolder)
        {
            Gizmos.DrawSphere(waypoint.position, 0.3f);
            Gizmos.DrawLine(previousPosition, waypoint.position);
            previousPosition = waypoint.position;
        }
        Gizmos.DrawLine(previousPosition, startPosition);

        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, transform.forward * viewDistance);
    }

    /// <summary>
    /// Looping Logic for when guards should patrol their routes and player hasn't been spotted
    /// </summary>
    /// <param name="waypoints"></param>
    /// <returns></returns>
    IEnumerator Patrol(Vector3[] waypoints)
    {
        transform.position = waypoints[0];

        //we start at the first waypoint so we want our target to be the next in line
        int targetWaypointNum = 1;
        Vector3 targetWaypointLoc = waypoints[targetWaypointNum];

        transform.LookAt(targetWaypointLoc);

        while (true)
        {
            //moves towards the next waypoint
            transform.position = Vector3.MoveTowards(transform.position, targetWaypointLoc, speed * Time.deltaTime);

            //once waypoint is reached
            if (transform.position == targetWaypointLoc)
            {
                //once the waypoint array length is reached, targetWaypointNum will be set to 0 using the modulus operator
                targetWaypointNum = (targetWaypointNum + 1) % waypoints.Length;

                //set new target waypoint to move towards
                targetWaypointLoc = waypoints[targetWaypointNum];

                //wait before moving to next location, makes sense but is also necessary to avoid an infinite loop crash even though it technically is an infinite loop
                yield return new WaitForSeconds(waitTime);
                yield return StartCoroutine(Turn(targetWaypointLoc));
            }
            yield return null;
        }
    }

    /// <summary>
    /// Used to determine where the guard should look before moving to the next patrol point
    /// </summary>
    /// <param name="lookTarget"></param>
    /// <returns></returns>
    IEnumerator Turn(Vector3 lookTarget)
    {
        //gets the direction the guard should rotate towrds before moving to the next waypoint
        Vector3 dirToLook = (lookTarget - transform.position).normalized;
        float targetAngle = 90 - Mathf.Atan2 (dirToLook.z, dirToLook.x) * Mathf.Rad2Deg;

        //if the guard is not turned towards the correct angle, turn towards next waypoint
        while (Mathf.Abs(Mathf.DeltaAngle(transform.eulerAngles.y, targetAngle)) > 0.05f)
        {
            float angle = Mathf.MoveTowardsAngle(transform.eulerAngles.y, targetAngle, turnSpeed * Time.deltaTime);
            transform.eulerAngles = Vector3.up * angle;
            yield return null;
        }
    }

    private bool CanSeePlayer()
    {
        if (Vector3.Distance(transform.position, player.transform.position) < viewDistance)
        {
            Vector3 dirToPlayer = (player.position - transform.position).normalized;
            float angleBetweenGuardAndPlayer = Vector3.Angle(transform.forward, dirToPlayer);
            if (angleBetweenGuardAndPlayer < viewAngle / 2f)
            {
                if (!Physics.Linecast(transform.position, player.position, viewMask))
                {
                    return true;
                }
            }
        }
        return false;
    }

}

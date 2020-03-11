using System.Collections.Generic;
using UnityEngine;

public class AI_Kart : MonoBehaviour
{
    public Transform[] waypoints;
    public float moveSpeed;
    public int waypointIndex = 0;

    private void Start()
    {
        transform.position = waypoints[waypointIndex].transform.position;
        Debug.Log(waypoints[waypointIndex].transform.position);
    }
    private void Update()
    {
        MoveIA();
    }
    public void MoveIA()
    {

        transform.position = Vector3.MoveTowards(transform.position,
        waypoints[waypointIndex].transform.position, moveSpeed * Time.deltaTime);
        Debug.Log("A");
        if (this.transform.position == waypoints[waypointIndex].transform.position)
        {
            Debug.Log("B");
            waypointIndex += 1;
        }
        if (this.waypointIndex == waypoints.Length)
        {
            waypointIndex = 0;
            Debug.Log("C");
        }
    }
}

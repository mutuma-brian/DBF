using System.Collections;
using UnityEngine;

public class AIController : MonoBehaviour
{
    public float accel = 0.8f;        // Rate of acceleration.
    public float inertia = 0.9f;     // Amount of velocity retained during slowing down.
    public float speedLimit = 10.0f;  // Maximum speed.
    public float minSpeed = 1.0f;    // Speed threshold for slowing down.
    public float stopTime = 1.0f;     // Time to pause at waypoints.

    private float currentSpeed = 0.0f;  // Current speed.
    private int functionState = 0;      // Function state: 0 for Accel, 1 for Slow.
    private bool accelState = false;    // Accel state.
    private bool slowState = false;    // Slow state.

    public float rotationDamping = 6.0f; // Rotation damping.
    public bool smoothRotation = true;  // Use smooth rotation.

    public Transform[] waypoints;       // Array of waypoints.
    private int WPindexPointer = 0;     // Index of the current waypoint.
    private Transform waypoint;          // Current waypoint.

    void Start()
    {
        functionState = 0;
    }

    void Update()
    {
        if (functionState == 0)
        {
            Accell();
        }
        if (functionState == 1)
        {
            StartCoroutine(Slow());
        }
        waypoint = waypoints[WPindexPointer];
    }

    // Accel function
    void Accell()
    {
        if (!accelState)
        {
            accelState = true;
            slowState = false;
        }

        if (waypoint)
        {
            if (smoothRotation)
            {
                Quaternion rotation = Quaternion.LookRotation(waypoint.position - transform.position);
                transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * rotationDamping);
            }
        }

        currentSpeed = currentSpeed + accel * accel;
        transform.Translate(0, 0, Time.deltaTime * currentSpeed);

        if (currentSpeed >= speedLimit)
        {
            currentSpeed = speedLimit;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Waypoint")) // Assuming waypoints have the "Waypoint" tag.
        {
            functionState = 1;
            WPindexPointer++;

            if (WPindexPointer >= waypoints.Length)
            {
                WPindexPointer = 0;
            }
        }
    }

    // Slow coroutine
    IEnumerator Slow()
    {
        if (!slowState)
        {
            accelState = false;
            slowState = true;
        }

        currentSpeed = currentSpeed * inertia;
        transform.Translate(0, 0, Time.deltaTime * currentSpeed);

        if (currentSpeed <= minSpeed)
        {
            currentSpeed = 0.0f;
            yield return new WaitForSeconds(stopTime);
            functionState = 0;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Marble : MonoBehaviour
{
    private const float VelocityMaxMagnitude = 80f;

    private const float PowerUpDuration = 5f;

    public int GemCount { get; private set; }

    private int gemIncrementAmount = 1;

    private Rigidbody rb;

    private bool winner;

    private readonly Vector3 rotateVelocity = new Vector3(85f, 0f, 0f);

    private void Awake()
    {
        winner = false;
        rb = GetComponent<Rigidbody>();
    }

    // Start is called before the first frame update
    void Start()
    {
        AddRandomForce();
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void FixedUpdate()
    {
        if (!GameManager.Instance.IsSimActive)
        {
            return;
        }

        var velocity = rb.velocity;
        var velocityMagnitude = velocity.magnitude;

        if (velocityMagnitude == 0)
        {
            return;
        }

        if (velocityMagnitude < VelocityMaxMagnitude)
        {
            var forceMultiplier = VelocityMaxMagnitude / velocityMagnitude;
            rb.AddForce(velocity.normalized * forceMultiplier);
        }

        //if (winner)
        //{
        //    var deltaRotation = Quaternion.Euler(rotateVelocity * Time.fixedDeltaTime);
        //    rb.MoveRotation(rb.rotation * deltaRotation);
        //}
    }

    private void OnCollisionEnter(Collision collision)
    {
        //if (collision.gameObject.CompareTag("Barrier"))
        //{
        //    AddForceTowardsCenter();
        //    return;
        //}

        //TODO: Probably do a switch expression on the tag and call the correpsonding function

        //gem, call UpdateScore which will also call a correpsonding method in gameManager
        //
    }

    private void AddRandomForce()
    {
        //Get random direction
        var direction = new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f));

        //Add force to move cell
        rb.AddForce(direction.normalized * 450f);
    }









    public void DisplayWinner()
    {
        rb.velocity = Vector3.zero;
        rb.rotation = Quaternion.identity;
        rb.useGravity = false;
        winner = true;

        var mainCam = Camera.main.transform;
        transform.localScale = new Vector3(3f, 3f, 3f);
        transform.position = mainCam.position + mainCam.forward * 3;
    }
}

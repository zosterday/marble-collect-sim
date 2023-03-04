using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Marble : MonoBehaviour
{
    private const float YLowerBound = -1.5f;

    private const float centerRotationOffset = 37.5f;

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
        if (transform.position.y < YLowerBound)
        {
            GameManager.Instance.RemoveMarble(gameObject);
        }
    }

    private void FixedUpdate()
    {
        if (winner)
        {
            var deltaRotation = Quaternion.Euler(rotateVelocity * Time.fixedDeltaTime);
            rb.MoveRotation(rb.rotation * deltaRotation);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Barrier"))
        {
            AddForceTowardsCenter();
            return;
        }

        AddRandomForce();
    }

    private void AddRandomForce()
    {
        //Get random direction
        var direction = new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f));

        //Get random speed
        var speed = Random.Range(250f, 600f);

        //Add force to move cell
        rb.AddForce(direction.normalized * speed);
    }

    private void AddForceTowardsCenter()
    {
        //Get random direction
        var direction = Vector3.zero - new Vector3(transform.position.x, 0, transform.position.z);

        //Get random speed
        var speed = Random.Range(250f, 600f);

        //Add force to move cell
        rb.AddForce(direction.normalized * speed);
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

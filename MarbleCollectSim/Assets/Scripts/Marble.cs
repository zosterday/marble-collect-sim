using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class Marble : MonoBehaviour
{
    private const string GemTag = "Gem";

    private const string CollectionBoostTag = "CollectionBoost";

    private const string SpeedBoostTag = "SpeedBoost";

    private const string SizeMultiplierTag = "SizeMultiplier";

    private const float PowerUpDuration = 6f;

    private const float SpeedBoostAmount = 45f;

    private readonly Vector3 rotateVelocity = new Vector3(0f, 65f, 0f);

    private float velocityMaxMagnitude = 80f;

    public int GemCount { get; private set; }

    private int gemIncrementAmount = 1;

    private Rigidbody rb;

    private bool winner;

    private void Awake()
    {
        winner = false;
        rb = GetComponent<Rigidbody>();
    }

    void Start()
    {
        AddRandomForce();
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

        if (velocityMagnitude < velocityMaxMagnitude)
        {
            var forceMultiplier = velocityMaxMagnitude / velocityMagnitude;
            rb.AddForce(velocity.normalized * forceMultiplier);
        }
    }

    private void AddRandomForce()
    {
        var direction = new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f));

        rb.AddForce(direction.normalized * 450f);
    }

    private void OnTriggerEnter(Collider other)
    {
        switch (other.tag)
        {
            case GemTag:
                ConsumeGem();
                break;

            case CollectionBoostTag:
                StartCoroutine(BoostCollectionAmount());
                break;

            case SizeMultiplierTag:
                StartCoroutine(MultiplySize());
                break;

            case SpeedBoostTag:
                StartCoroutine(BoostSpeed());
                break;
        }

        Destroy(other.gameObject);
    }

    private void ConsumeGem()
    {
        GemCount += gemIncrementAmount;
        GameManager.Instance.UpdateGemCount(gameObject, GemCount);
    }

    private IEnumerator BoostCollectionAmount()
    {
        gemIncrementAmount += 1;

        yield return new WaitForSeconds(PowerUpDuration);

        gemIncrementAmount -= 1;
    }

    private IEnumerator MultiplySize()
    {
        transform.localScale *= 2f;

        yield return new WaitForSeconds(PowerUpDuration);

        transform.localScale *= 0.5f;
    }

    private IEnumerator BoostSpeed()
    {
        velocityMaxMagnitude += SpeedBoostAmount;

        yield return new WaitForSeconds(PowerUpDuration);

        velocityMaxMagnitude -= SpeedBoostAmount;
    }

    public void EndSimulation()
    {
        if (winner)
        {
            var deltaRotation = Quaternion.Euler(rotateVelocity * Time.fixedDeltaTime);
            rb.MoveRotation(rb.rotation.normalized * deltaRotation);
            return;
        }

        rb.velocity = Vector3.zero;
        rb.rotation = Quaternion.identity;
        rb.freezeRotation = true;
    }

    public void DisplayWinner()
    {
        var mainCam = Camera.main.transform;

        rb.velocity = Vector3.zero;
        rb.rotation = mainCam.rotation;
        rb.useGravity = false;

        winner = true;

        transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        transform.position = mainCam.position + mainCam.forward * 3;
    }
}

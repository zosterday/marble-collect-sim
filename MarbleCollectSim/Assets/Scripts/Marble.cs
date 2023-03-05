using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Marble : MonoBehaviour
{
    private const string GemTag = "Gem";

    private const string CollectionBoostTag = "CollectionBoost";

    private const string SpeedBoostTag = "SpeedBoost";

    private const string SizeMultiplierTag = "SizeMultiplier";

    private const float PowerUpDuration = 8f;

    private const float SpeedBoostAmount = 45f;

    private float velocityMaxMagnitude = 80f;

    private int gemCount;

    private int gemIncrementAmount = 1;

    private Rigidbody rb;

    private bool winner;

    private readonly Vector3 rotateVelocity = new Vector3(85f, 0f, 0f);

    private void Awake()
    {
        winner = false;
        rb = GetComponent<Rigidbody>();
    }

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

        if (velocityMagnitude < velocityMaxMagnitude)
        {
            var forceMultiplier = velocityMaxMagnitude / velocityMagnitude;
            rb.AddForce(velocity.normalized * forceMultiplier);
        }

        //if (winner)
        //{
        //    var deltaRotation = Quaternion.Euler(rotateVelocity * Time.fixedDeltaTime);
        //    rb.MoveRotation(rb.rotation * deltaRotation);
        //}
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
        gemCount += gemIncrementAmount;

        //TODO: Call gamemaneger method to update the gemcount over there
    }

    //private void ConsumeCollectionBoost(GameObject collectionBoostObj)
    //{
    //    StartCoroutine(BoostCollectionAmount());
    //    Destroy(collectionBoostObj);
    //}

    //private void ConsumeSizeMultiplier(GameObject sizeMultiplierObj)
    //{
    //    StartCoroutine(MultiplySize());
    //    Destroy(sizeMultiplierObj);
    //}

    //private void ConsumeSpeedBoost(GameObject speedBoostObj)
    //{
    //    StartCoroutine(BoostSpeed());
    //    Destroy(speedBoostObj);
    //}

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

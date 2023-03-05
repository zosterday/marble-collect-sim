using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeaderboardIcon : MonoBehaviour
{
    public int GemCount { get; set; }

    private readonly Vector3 rotateVelocity = new Vector3(0f, 85f, 0f);

    private readonly Vector3 screenOffset = new Vector3(50f, 60f, 0f);

    private readonly Vector3 placementYOffset = new Vector3(0f, -30f, 0f);

    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        transform.localScale = new Vector3(25f, 25f, 25f);
    }

    private void Start()
    {
        rb.velocity = Vector3.zero;
        rb.rotation = Quaternion.identity;
    }

    private void FixedUpdate()
    {
        var deltaRotation = Quaternion.Euler(rotateVelocity * Time.fixedDeltaTime);
        rb.MoveRotation(rb.rotation * deltaRotation);
    }

    public void SetLeaderboardPosition(int placement)
    {
        transform.localPosition = Vector3.zero + screenOffset + placementYOffset * placement;

        gameObject.SetActive(true);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeaderboardIcon : MonoBehaviour
{
    public int GemCount { get; set; }

    private readonly Vector3 rotateVelocity = new Vector3(0f, 85f, 0f);

    private readonly Vector3 firstScreenOffset = new Vector3(-2.5f, 2.434f, 0f);

    private readonly Vector3 secondScreenOffset = new Vector3(-2.55f, 2.264f, 0f);

    private readonly Vector3 thirdScreenOffset = new Vector3(0f, 0f, 0f);

    private readonly Vector3 fourthScreenOffset = new Vector3(0f, 0f, 0f);

    private readonly Vector3 fifthScreenOffset = new Vector3(0f, 0f, 0f);

    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        transform.localScale = new Vector3(0.16f, 0.16f, 0.16f);
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
        //var mainCam = Camera.main.transform;

        //var screenOffset = Vector3.zero;

        //switch (placement)
        //{
        //    case 1:
        //        screenOffset = firstScreenOffset;
        //        break;

        //    case 2:
        //        screenOffset = secondScreenOffset;
        //        break;

        //    case 3:
        //        screenOffset = thirdScreenOffset;
        //        break;

        //    case 4:
        //        screenOffset = fourthScreenOffset;
        //        break;

        //    case 5:
        //        screenOffset = fifthScreenOffset;
        //        break;
        //}
        //transform.position = mainCam.position + (mainCam.forward * 5) + screenOffset;

        transform.localPosition = Vector3.zero;

        gameObject.SetActive(true);
    }

    public void DisplayWinner()
    {
        var mainCam = Camera.main.transform;

        transform.localScale = new Vector3(3f, 3f, 3f);
        transform.position = mainCam.position + mainCam.forward * 3;
    }
}

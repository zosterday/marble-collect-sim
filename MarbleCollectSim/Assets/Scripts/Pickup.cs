using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour
{
    private const float YRotation = 45f;

    // Update is called once per frame
    void Update()
    {
        var rotation = new Vector3(0f, YRotation, 0f);
        transform.Rotate(rotation * Time.deltaTime);
    }
}

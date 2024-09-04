using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follower : MonoBehaviour
{
    public Transform followTransform;

    public bool followRotation = false;

    // Update is called once per frame
    void LateUpdate()
    {
        transform.position = followTransform.position;
        if (followRotation)
            transform.rotation = followTransform.rotation;
    }
}

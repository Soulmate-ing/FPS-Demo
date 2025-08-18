using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTarget : MonoBehaviour
{
    public Transform target;
    public Vector3 Offset;
    private void Update()
    {
        transform.position = target.position + Offset;
    }
}

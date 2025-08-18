using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowYRotation : MonoBehaviour
{
    public Transform target;
    private void Update()
    {
        transform.eulerAngles = new Vector3(0, target.eulerAngles.y, 0);
    }
}

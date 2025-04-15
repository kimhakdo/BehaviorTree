using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackingCamera : MonoBehaviour
{
    public Transform target;
    private void LateUpdate()
    {
        if (target == null) return;
        transform.position = target.position + new Vector3(0, 0, -10);
    }
}

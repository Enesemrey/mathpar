using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        var controller = other.GetComponent<Controller>();
        if (controller == null) return;
        var pos = transform.position;
        pos.y = 10;
        controller.checkPoint = pos;
    }
}

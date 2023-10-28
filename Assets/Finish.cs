using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Finish : MonoBehaviour
{
    public GameObject Fog;
    void OnTriggerEnter(Collider other)
    {
        var controller = other.GetComponent<Controller>();
        if (controller != null)
        {
            controller.Finish();
            if(controller is PlayerController)
                Fog.SetActive(true);
        }
    }
}

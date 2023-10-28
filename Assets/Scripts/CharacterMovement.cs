using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    public float ZSpeed;
    public float YSpeed = 0;


    // Start is called before the first frame update
    void Start()
    {

    }



    public void ApplyMovement()
    {
        var pos = transform.position;
        pos += ZSpeed * Time.fixedDeltaTime * Vector3.forward;
        transform.position = pos;
    }
    public void ApplyMovementSlow()
    {
        var pos = transform.position;
        pos += ZSpeed * Time.fixedDeltaTime * Vector3.forward * 0.75f;
        transform.position = pos;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour
{
    public bool Direction;
    public float Speed = 3;
    float _angle;
    // Update is called once per frame
    void Update()
    {
        if(GameManager.Instance.IsGameOver) return;
        _angle += Time.deltaTime * (Direction ? 10 : -10) * Speed;
        transform.rotation = Quaternion.Euler(0, _angle, 0);
    }
}

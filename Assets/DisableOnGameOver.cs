using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class DisableOnGameOver : MonoBehaviour
{

    void Update()
    {
        if (GameManager.Instance.IsGameOver)
            GetComponent<CinemachineVirtualCamera>().Follow = null;
    }
}

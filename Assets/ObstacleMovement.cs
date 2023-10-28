using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class ObstacleMovement : MonoBehaviour
{
    public GateMovement Movement;
    public float MovementSpeed = 5;

    private PlayerController _player;
    // Start is called before the first frame update
    void Start()
    {
        _player = FindObjectOfType<PlayerController>();
        var sequence = DOTween.Sequence();
        switch (Movement)
        {
            case GateMovement.Static:
                return;
            case GateMovement.Horizontal:
                sequence.Append(transform.DOMoveX(5, Mathf.Abs(transform.position.x - 5) / MovementSpeed).SetEase(Ease.Linear));
                sequence.Append(transform.DOMoveX(-5, 10 / MovementSpeed).From(5).SetEase(Ease.Linear).SetLoops(1000000, LoopType.Yoyo));
                break;
            case GateMovement.Vertical:
                sequence.Append(transform.DOMoveY(20, 10 / MovementSpeed)).SetRelative().SetEase(Ease.Linear).SetLoops(1000000, LoopType.Yoyo);
                break;
            default:
                return;

        }
    }

    void Update()
    {
        if (_player.transform.position.z - 5 > transform.position.z)
        {
            transform.DOScale(Vector3.zero, 0.1f);
            enabled = false;
        }
    }
}

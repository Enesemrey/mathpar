using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : Controller
{



    [SerializeField] private float _goodGatePosition;

    protected override void Start()
    {
        base.Start();
        _goodGatePosition = float.PositiveInfinity;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!GameManager.Instance.IsPlaying)
        {
            Stop();
            return;
        }
        if (IsFinished) return;
        if (IsFalling) return;
        //if (IsFinished && GetComponent<CharacterStack>().Count > 0)
        //{
        //    Move();
        //}

        //if (_stopped)
        //{
        //    _stopTimer -= Time.deltaTime;
        //    if (_stopTimer < 0)
        //    {
        //        Move(true);
        //        return;
        //    }
        //    return;
        //}

        var ray = new Ray(transform.position + Vector3.up, Vector3.forward);
        if (transform.position.z > _goodGatePosition)
            _goodGatePosition = float.PositiveInfinity;
        if (Physics.Raycast(ray, out var hit1, 100, LayerMask.GetMask("Default")))
        {
            if (hit1.transform.CompareTag("Gate"))
            {
                var gate = hit1.transform.GetComponent<Gate>();
                if ((gate.GateType == GateType.Multiply || gate.GateType == GateType.Add) && hit1.point.z < _goodGatePosition && gate.GateMovement != GateMovement.Static)
                {
                    _goodGatePosition = hit1.point.z;
                }
            }
        }


        if (Physics.Raycast(ray, out var hit, 3, LayerMask.GetMask("Default", "Obstacle")))
        {
            if (hit.transform.CompareTag("Gate"))
            {
                var gate = hit.transform.GetComponent<Gate>();
                if ((gate.GateType == GateType.Divide || gate.GateType == GateType.Subtract) && gate.GateMovement != GateMovement.Static)
                {
                    Stop();
                }
                else
                {
                    Move();
                }
            }
            else if (hit.transform.CompareTag("Obstacle"))
            {
                Stop();
            }
            else
            {
                Move();
            }
        }
        else
        {
            Move();
        }
    }

    private void Stop()
    {
        _animator.SetBool("IsMoving", false);
    }

    private void Move()
    {
        var ray = new Ray(transform.position + Vector3.up, Vector3.forward);
        if (_goodGatePosition - transform.position.z < 10)
        {
            var isHit = Physics.Raycast(ray, out var hit, 11, LayerMask.GetMask("Default"));
            if (!isHit)
            {
                Stop();
                _animator.SetBool("IsMoving", false);
                return;
            }
            var gate = hit.transform.GetComponent<Gate>();
            if (gate == null || gate.GateType == GateType.Divide || gate.GateType == GateType.Subtract)
            {
                Stop();
                _animator.SetBool("IsMoving", false);
                return;
            }
        }
        _animator.SetBool("IsMoving", true);
        var player = FindObjectOfType<PlayerController>();
        if (transform.position.z <= player.transform.position.z + 20)
            GetComponent<CharacterMovement>().ApplyMovement();
        else
            GetComponent<CharacterMovement>().ApplyMovementSlow();

    }
}
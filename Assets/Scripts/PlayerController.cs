using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using MoreMountains.NiceVibrations;
using UnityEngine;

public class PlayerController : Controller
{

    public float AutoPilot;


    // Update is called once per frame
    void FixedUpdate()
    {
        if (IsFinished || IsFalling)
        {
            //if (Input.GetMouseButton(0))
            //{
            //    _animator.SetBool("IsMoving", true);
            //}
            //else
            //{
            //    _animator.SetBool("IsMoving", false);
            //}
            return;
        }

        AutoPilot -= Time.deltaTime;
        if (Input.GetMouseButton(0) || AutoPilot > 0)
        {
            GetComponent<CharacterMovement>().ApplyMovement();
            _animator.SetBool("IsMoving", true);
        }
        else
        {
            _animator.SetBool("IsMoving", false);
        }
    }

    public override void Fall()
    {
        base.Fall();
        HapticsManager.TriggerHaptic(HapticTypes.HeavyImpact,1);
    }
}

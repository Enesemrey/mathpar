using System.Collections;
using System.Collections.Generic;
using MoreMountains.NiceVibrations;
using UnityEngine;

public class HapticsManager : MonoBehaviour
{
    private static float _cooldown;

    private static HapticTypes _hapticType;
    // Update is called once per frame
    void Update()
    {
        _cooldown -= Time.deltaTime;
    }

    public static void TriggerHaptic(HapticTypes type, float cooldownTime)
    {
        if (_cooldown > 0 && type == _hapticType)
            return;
        _hapticType = type;
        MMVibrationManager.Haptic(type);
        _cooldown = cooldownTime;
    }

}

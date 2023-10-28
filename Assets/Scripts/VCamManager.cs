using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using DG.Tweening;
using UnityEngine;

public class VCamManager : MonoBehaviour
{
    public static VCamManager Instance;
    private CinemachineVirtualCamera[] _cams;
    private CinemachineVirtualCamera _currentCam;

    [SerializeField] private ParticleSystem _confetti;

    private Transform _oldfollow;

    // Start is called before the first frame update
    void Awake()
    {
        Instance = this;
        _cams = FindObjectsOfType<CinemachineVirtualCamera>();
        var maxPriority = _cams.Max(m => m.Priority);
        _currentCam = _cams.FirstOrDefault(m => m.Priority == maxPriority);
    }

    public void SetCam(CinemachineVirtualCamera vCam)
    {
        foreach (var cam in _cams)
        {
            cam.Priority = 0;
        }
        vCam.Priority = 1;
        _currentCam = vCam;
    }

    public void PlayConfetti()
    {
        if (_confetti != null)
        {
            _confetti.gameObject.SetActive(true);
            _confetti.Play(true);
        }
    }

    public void StopFollow()
    {
        _oldfollow = _currentCam.Follow;
        _currentCam.Follow = null;
    }

    public void DoFollow()
    {
        _currentCam.Follow = _oldfollow;
    }
}

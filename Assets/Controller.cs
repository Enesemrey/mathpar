using System.Collections;
using Cinemachine;
using DG.Tweening;
using UnityEngine;

public class Controller : MonoBehaviour
{
    [SerializeField] protected Animator _animator;
    public CinemachineVirtualCamera FinalCam;
    public Vector3 checkPoint;
    private float _originalX;

    public bool IsFinished = false;
    public bool IsFalling = false;
    public GameObject RingEffect;

    protected virtual void Start()
    {
        _originalX = transform.position.x;
        checkPoint = transform.position;
    }
    public void Finish()
    {
        IsFinished = true;
        StartCoroutine(FinishRoutine());
    }

    private IEnumerator FinishRoutine()
    {
        if (FinalCam != null)
        {
            VCamManager.Instance.SetCam(FinalCam);
        }
        _animator.SetBool("IsMoving", false);
        var characterStack = GetComponent<CharacterStack>();
        if (RingEffect != null)
            RingEffect.SetActive(false);
        if (characterStack.Count < 1)
        {
            _animator.SetBool("IsMoving", false);
            yield break;
        }
        yield return characterStack.ThrowStack();
    }

    public virtual void Fall()
    {
        if (IsFalling) return;
        StartCoroutine(FallRoutine());
    }

    private IEnumerator FallRoutine()
    {
        IsFalling = true;
        if (RingEffect != null)
            RingEffect.SetActive(false);
        _animator.enabled = false;
        var parts = GetComponentsInChildren<Rigidbody>();
        foreach (var part in parts)
        {
            part.isKinematic = false;
            part.collisionDetectionMode = CollisionDetectionMode.Continuous;
        }

        var characterStack = GetComponent<CharacterStack>();
        characterStack.IsThrown = true;
        characterStack.StacksPlaced = true;
        foreach (var obj in characterStack.StackObjects.ToArray())
        {
            obj.transform.parent = null;
        }

        if (this is PlayerController)
        {
            VCamManager.Instance.StopFollow();
        }
        yield return new WaitForSeconds(3);
        if (RingEffect != null)
            RingEffect.SetActive(true);
        _animator.enabled = true;
        parts = GetComponentsInChildren<Rigidbody>();
        foreach (var part in parts)
        {
            if (part == null) continue;
            part.collisionDetectionMode = CollisionDetectionMode.Discrete;
            part.isKinematic = true;
            part.velocity = Vector3.zero;
            part.angularVelocity = Vector3.zero;
        }

        characterStack.IsThrown = false;
        characterStack.StacksPlaced = false;
        checkPoint.x = _originalX;
        transform.position = checkPoint;

        if (this is PlayerController)
            VCamManager.Instance.DoFollow();
        yield return characterStack.Reset();
        IsFalling = false;
        //if (this is PlayerController)
        //    GameManager.Instance.Fail();
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Obstacle")) return;
        Fall();
    }
}
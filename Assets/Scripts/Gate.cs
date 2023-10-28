using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using MoreMountains.NiceVibrations;
using TMPro;
using UnityEngine;

public class Gate : MonoBehaviour
{
    public int Amount = 2;
    public GateType GateType;
    public GateMovement GateMovement;
    public bool IsActivated;
    public bool IsFinished;
    public GameObject GateMesh;
    public TextMeshProUGUI GateText;
    public Material GoodMaterial;
    public Material BadMaterial;
    public float MovementSpeed = 5;
    List<CharacterStack> _stacks;

    void OnValidate()
    {
        Apply(false);
    }
    void Awake()
    {
        _stacks = new List<CharacterStack>();
        Apply();
    }

    void Update()
    {
        GateText.gameObject.SetActive(transform.forward.z < 0);
    }
    void OnTriggerEnter(Collider other)
    {
        if (GameManager.Instance.IsGameOver) return;
        if (transform.forward.z > 0) return;
        if (!other.CompareTag("Character")) return;

        var characterStack = other.GetComponent<CharacterStack>();
        if (_stacks.Contains(characterStack)) return;
        _stacks.Add(characterStack);
        if (other.GetComponent<PlayerController>() != null)
            HapticsManager.TriggerHaptic(HapticTypes.MediumImpact, 0.25f);
        switch (GateType)
        {
            case GateType.Add:
                StartCoroutine(characterStack.AddRoutine(Amount));
                break;
            case GateType.Subtract:
                StartCoroutine(characterStack.RemoveRoutine(Amount));
                break;
            case GateType.Divide:
                StartCoroutine(characterStack.DivideRoutine(Amount));
                break;
            case GateType.Multiply:
                StartCoroutine(characterStack.MultiplyRoutine(Amount));
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

    }

    IEnumerator Start()
    {
        var sequence = DOTween.Sequence();
        switch (GateMovement)
        {
            case GateMovement.Static:
                yield break;
            case GateMovement.Horizontal:
                sequence.Append(transform.DOMoveX(5, Mathf.Abs(transform.position.x - 5) / MovementSpeed).SetEase(Ease.Linear));
                sequence.Append(transform.DOMoveX(-5, 10 / MovementSpeed).From(5).SetEase(Ease.Linear).SetLoops(1000000, LoopType.Yoyo));
                break;
            case GateMovement.Vertical:
                sequence.Append(transform.DOMoveY(10, 10 / MovementSpeed)).SetRelative().SetEase(Ease.Linear).SetLoops(1000000, LoopType.Yoyo);
                var _player = FindObjectOfType<PlayerController>();
                yield return new WaitUntil(() => _player.transform.position.z - 2 > transform.position.z);
                sequence.Kill();
                GateMovement = GateMovement.Static;
                transform.DOMoveY(10, 5 / MovementSpeed);
                break;
            default:
                yield break;

        }


    }
    public void FinishGate()
    {
        IsFinished = true;
        GetComponent<Collider>().enabled = false;
    }

    void ActivateGate()
    {
        transform.DOScale(Vector3.one, 0.25f).From(Vector3.zero);
        GateMesh.SetActive(true);
        IsActivated = true;
    }

    public void DestroyGate()
    {
        transform.DOScale(Vector3.zero, 0.25f).OnComplete(() => Destroy(gameObject));
        IsActivated = false;
    }

    public void Apply(bool activate = true)
    {
        //if(activate)
        //ActivateGate();
        var renderer = GateMesh.GetComponent<Renderer>();
        switch (GateType)
        {
            case GateType.Add:
                GateText.text = "+" + Amount;
                renderer.material = GoodMaterial;
                break;
            case GateType.Subtract:
                GateText.text = "-" + Amount;
                renderer.material = BadMaterial;
                break;
            case GateType.Divide:
                renderer.material = BadMaterial;
                GateText.text = "1/" + Amount;
                break;
            case GateType.Multiply:
                renderer.material = GoodMaterial;
                GateText.text = "x" + Amount;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public void Remove(CharacterStack characterStack)
    {
        if (_stacks.Contains(characterStack))
            _stacks.Remove(characterStack);
    }
}

public enum GateType
{
    Add, Subtract, Divide, Multiply
}

public enum GateMovement
{
    Static, Horizontal, Vertical
}
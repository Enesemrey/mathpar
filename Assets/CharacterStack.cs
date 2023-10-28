using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using DG.Tweening;
using MoreMountains.NiceVibrations;
using TMPro;
using UnityEngine;

public class CharacterStack : MonoBehaviour
{
    public int StartStackCount = 10;
    public Queue<GameObject> StackObjects;
    public GameObject StackObjectPrefab;
    public Transform StackPosition;
    public int Count;
    public Vector3 FinalPos;
    public bool IsThrown;
    public TextMeshProUGUI StackText;
    public Animator _animator;
    private Vector3 _startPos;
    private Vector3 _lastPos;
    public List<GameObject> PlacedPieces;

    IEnumerator Start()
    {
        PlacedPieces = new List<GameObject>();
        _animator = GetComponentInChildren<Animator>();
        StackObjects = new Queue<GameObject>();
        Count = StartStackCount;
        for (int i = 0; i < StartStackCount; i++)
        {
            yield return AddToStack(0.05f);
        }
    }

    void FixedUpdate()
    {
        if (GetComponent<Controller>().IsFalling) return;
        if (StackText != null)
        {
            StackText.gameObject.SetActive(Count > 0 && !IsThrown);
            StackText.text = StackObjects.Count.ToString();
            //if (Count > 0)
            //    StackText.transform.position = StackObjects.Peek().transform.position - StackText.transform.forward;
        }

        if (IsThrown)
        {
            if (StacksPlaced)
                return;
            if (Input.GetMouseButton(0) && GetComponent<PlayerController>() != null)
            {
                var d = (FinalPos - transform.position);
                if (d.z <= 0)
                {
                    transform.position = FinalPos;
                    return;
                }
                var dir = d.normalized;
                transform.position += dir * Time.fixedDeltaTime * d.sqrMagnitude * 4;
            }
            else if (GetComponent<PlayerController>() == null)
            {
                var d = (FinalPos - transform.position);
                if (d.z <= 0)
                {
                    transform.position = FinalPos;
                    return;
                }
                var dir = d.normalized;
                transform.position += dir * Time.fixedDeltaTime * d.sqrMagnitude * 4;
            }
            return;
        }

        var array = StackObjects.Reverse().ToArray();
        var movement = transform.position.z - _lastPos.z;
        for (var i = 0; i < array.Length; i++)
        {
            var stackObject = array[i];
            var targetPos = StackPosition.transform.position + StackPosition.transform.up * 0.1f * i -
                            Vector3.forward * movement * (0.05f / array.Length) * i * i;
            var currentPos = stackObject.transform.position;
            currentPos.y = targetPos.y;
            stackObject.transform.position = currentPos;
            var d = targetPos - stackObject.transform.position;
            var stackPiece = stackObject.GetComponent<StackPiece>();
            stackPiece.velocity += d * Time.deltaTime * 10;
            var v = stackPiece.velocity;
            v.z = Mathf.Clamp(v.z, -0.01f * i * i, 0.01f * i * i);
            stackPiece.velocity = v;
        }

        _lastPos = transform.position;
        var pos = transform.position;
        var remainder = pos.z % 0.25f;
        pos.z -= remainder;
        var ray = new Ray(pos + Vector3.up, Vector3.down);
        if (!Physics.Raycast(ray, 2, LayerMask.GetMask("Ground")))
        {
            if (StackObjects.Count < 1)
            {
                GetComponent<Controller>().Fall();
                return;
            }
            Count--;
            StartCoroutine(RemoveFromStack(0.05f, false));
            var obj = Instantiate(StackObjectPrefab, pos, Quaternion.identity);
            PlacedPieces.Add(obj);
            obj.GetComponent<StackPiece>().Wobble();
            var playerController = GetComponent<PlayerController>();
            if (playerController != null)
            {
                HapticsManager.TriggerHaptic(HapticTypes.Selection, .1f);
                playerController.AutoPilot = 0.15f;
            }
        }
    }

    public IEnumerator AddToStack(float time = 0.1f)
    {
        var obj = Instantiate(StackObjectPrefab,
            StackPosition.transform.position /*+ StackPosition.transform.up * 0.1f * StackObjects.Count*/, StackPosition.rotation);
        obj.transform.parent = StackPosition;
        obj.transform.DOScale(Vector3.one, time).From(Vector3.zero);
        StackObjects.Enqueue(obj);


        yield return new WaitForSeconds(time);
    }

    public IEnumerator RemoveFromStack(float time = 0.1f, bool isGate = true)
    {
        if (StackObjects.Count < 2 && isGate) yield break;
        var obj = StackObjects.Dequeue();
        obj.transform.DOScale(Vector3.zero, time);
        yield return new WaitForSeconds(time);
        Destroy(obj);
    }

    public IEnumerator MultiplyRoutine(int amount)
    {

        var target = (amount - 1) * Count;
        Count += target;

        for (int i = 0; i < target; i++)
        {
            yield return AddToStack(0.05f);
        }
    }

    public IEnumerator DivideRoutine(int amount)
    {
        var target = Count - Count / amount;
        Count -= target;
        for (int i = 0; i < target; i++)
        {
            yield return RemoveFromStack(0.05f);
        }
        if (Count < 1) Count = 1;

    }
    public IEnumerator AddRoutine(int amount)
    {
        Count += amount;
        for (int i = 0; i < amount; i++)
        {
            yield return AddToStack(0.05f);
        }
    }

    public IEnumerator RemoveRoutine(int amount)
    {
        Count -= amount;
        for (int i = 0; i < amount; i++)
        {
            yield return RemoveFromStack(0.05f);
        }

        if (Count < 1) Count = 1;
    }

    public IEnumerator ThrowStack()
    {
        if (IsThrown) yield break;
        IsThrown = true;
        var time = 0.02f;
        var array = StackObjects.ToArray();
        _startPos = transform.position;
        var count = Count;
        GameManager.Instance.MaxCount = Mathf.Max(count, GameManager.Instance.MaxCount);
        FinalPos = _startPos/* + (StackObjects.Count - 1) * new Vector3(0, 0.43f, 1) + Vector3.forward * .5f*/;
        int i = 0;
        while (StackObjects.Count > 0)
        {
            if (GetComponent<PlayerController>() != null)
            {
                HapticsManager.TriggerHaptic(HapticTypes.Selection, .1f);
                _animator.SetBool("IsMoving", Input.GetMouseButton(0));
                yield return new WaitUntil(() => Input.GetMouseButton(0));
            }
            else
            {
                _animator.SetBool("IsMoving", true);
            }
            var stackObject = StackObjects.Dequeue();
            stackObject.transform.parent = null;
            stackObject.transform.rotation = Quaternion.identity;
            var pos = _startPos + i * new Vector3(0, 0.43f, 1);
            i++;
            FinalPos = pos;
            var stackPiece = stackObject.GetComponent<StackPiece>();
            stackPiece.velocity = Vector3.zero;
            stackObject.transform.DOMove(pos, time).OnComplete(() =>
            {
                stackPiece.Wobble();
            });
            //transform.DOMove(FinalPos, 0.03f).SetEase(Ease.Linear);
            yield return new WaitForSeconds(time);
        }
        StacksPlaced = true;
        _animator.SetBool("IsMoving", true);
        transform.DOMove(FinalPos + Vector3.forward * .5f, 0.2f).SetEase(Ease.Linear);
        yield return new WaitForSeconds(0.2f);
        _animator.SetBool("IsMoving", false);
        Count = 0;
        if (GetComponent<PlayerController>() != null)
        {
            //VCamManager.Instance.StopFollow();
            StartCoroutine(GameManager.Instance.FinishLevel());
        }
        else
        {
            yield return new WaitUntil(() => GameManager.Instance.IsGameOver);
        }
        _animator.transform.DORotate(new Vector3(0, 180, 0), 0.5f);
        _animator.SetBool("Win", GameManager.Instance.IsWin(count));
        _animator.SetBool("Finish", true);
    }

    public bool StacksPlaced;

    public IEnumerator Reset()
    {
        foreach (var placedPiece in PlacedPieces)
        {
            Destroy(placedPiece);
        }
        PlacedPieces.Clear();
        foreach (var stackObject in StackObjects)
        {
            Destroy(stackObject.gameObject);
        }
        StackObjects.Clear();
        Count = StartStackCount;
        for (int i = 0; i < StartStackCount; i++)
        {
            yield return AddToStack(0.05f);
        }

        foreach (var stackObject in StackObjects)
        {
            stackObject.GetComponent<StackPiece>().velocity = Vector3.zero;
        }
        foreach (var gate in FindObjectsOfType<Gate>())
        {
            gate.Remove(this);
        }

    }
}
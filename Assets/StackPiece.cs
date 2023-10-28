using DG.Tweening;
using MoreMountains.NiceVibrations;
using UnityEngine;

public class StackPiece : MonoBehaviour
{
    public Transform Mesh;
    public ParticleSystem Effect;
    public ParticleSystem Effect2;
    public Vector3 velocity;
    public Sequence seq;
    void Start()
    {
        Wobble(false);
    }

    public void Wobble(bool effect = true)
    {

        seq?.Kill();
        seq = DOTween.Sequence();
        seq.Append(Mesh.transform.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack, 5).From(0));
        if (!effect) return;
        if (Effect != null)
        {
            Effect.gameObject.SetActive(true);
            Effect.Play(true);
        }
        else if (Effect2 != null)
        {
            Effect2.gameObject.SetActive(true);
            Effect2.Play(true);
        }
    }
    void FixedUpdate()
    {
        transform.position += velocity * Time.fixedDeltaTime;
    }

}
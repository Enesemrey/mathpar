using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    
    public TextMeshProUGUI LevelText;

    public Image Fader;
    

    public RectTransform TapToStart;
    public RectTransform PreGameUI;
    public RectTransform InGameUI;
    public RectTransform RetryButton;
    public RectTransform NextButton;
    public RectTransform WinIcon;
    public RectTransform LoseIcon;

    private Sequence tapToStartSequence;
    void Awake()
    {
        if (Instance != null)
            Destroy(Instance.gameObject);
        Instance = this;
        InGameUI.gameObject.SetActive(false);
        tapToStartSequence = EnableAndBounce(TapToStart,bounceAmount:0.1f);
    }

    void Update()
    {
        LevelText.text = "Level " + GameManager.Instance.CurrentLevel;
        
    }
    public void DisableTapToStart()
    {
        tapToStartSequence.Kill();
        PreGameUI.transform.DOScale(Vector3.zero, 0.5f);
        InGameUI.gameObject.SetActive(true);
    }

    public void EnableRetryButton()
    {
        LevelText.rectTransform.DOScale(Vector3.zero, 0.25f);
        Fader.DOColor(new Color(0, 0, 0, 0.6f), 0.5f);
        EnableAndBounce(RetryButton);
        EnableAndBounce(LoseIcon, bounceAmount:0);
    }

    public void EnableNextButton()
    {
        LevelText.rectTransform.DOScale(Vector3.zero, 0.25f);
        Fader.DOColor(new Color(0, 0, 0, 0.6f), 0.5f);
        EnableAndBounce(NextButton);
        EnableAndBounce(WinIcon, bounceAmount: 0);
    }

    public Sequence EnableAndBounce(RectTransform rect, float enableDuration = 0.5f, float loopDuration = 1, float bounceAmount = 0.2f)
    {
        var sequence = DOTween.Sequence();
        rect.gameObject.SetActive(true);
        sequence.Append(rect.DOScale(Vector3.one, enableDuration).From(Vector3.zero))
            .Append(rect.DOScale(Vector3.one * (1 + bounceAmount), loopDuration).SetLoops(100, LoopType.Yoyo));
        sequence.Play();
        return sequence;
    }

    public IEnumerator FadeInOut(Action callback)
    {
        Fader.DOColor(Color.black, 1);
        yield return new WaitForSeconds(1);
        callback.Invoke();
        Fader.DOColor(Color.clear, 1);
    }

    public void BounceOnce(RectTransform rect, float duration = 0.25f)
    {
        rect.DOScale(Vector3.one * 1.2f, duration).SetLoops(2, LoopType.Yoyo);
    }
}

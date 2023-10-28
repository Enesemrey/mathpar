using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager instance;

    public bool playerReacted = true, repeat = false;
    private Action noReactionCallback;
    [SerializeField] private float timer;
    private float timeToShowTutorial;
    private Func<bool> checkPlayerReact;
    public Animator handAnimator;
    [SerializeField] private GameObject dragImage;
    [SerializeField] private TextMeshProUGUI tutorialMessage;
    public bool tutorialMessageOnScreen;
    [SerializeField] private UnityEngine.UI.Image highlightImage;

    private Coroutine playerReactCor;

    private void Awake()
    {
        //Singleton
        if (instance) Destroy(instance.gameObject);
        instance = this;
    }

    private void OnEnable()
    {
        handAnimator.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (playerReacted) return;

        if (checkPlayerReact != null)
            if (checkPlayerReact.Invoke())
            {
                timer = 0;
                playerReacted = true;
                //if (playerReactCor == null) playerReactCor = StartCoroutine(CheckRepeat());

                return;
            }

        if (timer >= timeToShowTutorial)
        {
            NoReactiontutorial();
            return;
        }

        timer += Time.deltaTime;
    }

    public void SetNoReactionTimer(float value, Action _noReactionCallback, Func<bool> _checkPlayerReact = null, bool _repeat = false)
    {
        timeToShowTutorial = value;
        checkPlayerReact = _checkPlayerReact;
        noReactionCallback = _noReactionCallback;
        playerReacted = false;
        repeat = _repeat;
        Debug.Log(playerReacted);
    }

    private void NoReactiontutorial()
    {
        if(noReactionCallback != null) noReactionCallback.Invoke();
        playerReacted = true;
    }

    
    public void PlayHandAnim(string _tutorialAnimationName, Func<bool> stopTutorialCallback, string _tutorialMessage = null, bool showDragImage = false)
    {
        handAnimator.gameObject.SetActive(true);
        if (showDragImage) dragImage.SetActive(true);
        if (_tutorialMessage != null)
        {
            if (tutorialMessageOnScreen) SetTutorialMessage(_tutorialMessage);
            PopUpTutorialMessage(_tutorialMessage, true);
        }
        StartCoroutine(AnimCor());

        IEnumerator AnimCor()
        {
            yield return new WaitForEndOfFrame();
            handAnimator.Play(_tutorialAnimationName);
            yield return new WaitUntil(stopTutorialCallback);
            StopHandAnimation();
            if (showDragImage) dragImage.SetActive(false);
            CloseTutorialMessage();
            timer = 0f;
            if (repeat) playerReacted = false;
            Debug.Log(playerReacted);
        }
    }
    

    public void StopHandAnimation()
    {
        handAnimator.gameObject.SetActive(false);
    }

    private IEnumerator CheckRepeat()
    {
        yield return new WaitUntil(() => !checkPlayerReact.Invoke());
        playerReacted = false;
        playerReactCor = null;
        Debug.Log(playerReacted);
    }

    public void ClearTutorial()
    {
        handAnimator.gameObject.SetActive(false);
        CloseTutorialMessage();
        timer = 0f;
        checkPlayerReact = null;
        noReactionCallback = null;
        playerReacted = true;
        repeat = false;
        if (playerReactCor != null) StopCoroutine(instance.playerReactCor);
        playerReactCor = null;
    }

    public void PopUpTutorialMessage(string _message, bool popUpForce = false)
    {
        if (!tutorialMessageOnScreen)
        {
            tutorialMessage.SetText(_message);
            tutorialMessage.gameObject.SetActive(true);
            tutorialMessage.transform.parent.GetComponent<Animator>().Play("UIPopUp");
            tutorialMessageOnScreen = true;
        }
        else
        {
            tutorialMessage.SetText(_message);
            if (popUpForce) tutorialMessage.transform.parent.GetComponent<Animator>().Play("UIPopUp");
        }
    }

    public void SetTutorialMessage(string _message)
    {
        tutorialMessage.text = _message;
    }

    public void CloseTutorialMessage()
    {
        tutorialMessage.transform.parent.GetComponent<Animator>().Play("UIPopDown");
    }

    public void Highlight(Vector3 worldPos, bool orientateToHandAnim = false, bool alignToHandPos = false, float scale = 1f)
    {
        Animator a = highlightImage.GetComponent<Animator>();
        if (orientateToHandAnim) StartCoroutine(Wait());
        else
        {
            highlightImage.gameObject.SetActive(true);
            highlightImage.transform.localScale = Vector3.one * scale;
            highlightImage.transform.position = Camera.main.WorldToScreenPoint(worldPos);
            
            a.Play("UIPopUp");
        }

        IEnumerator Wait()
        {
            yield return new WaitUntil(() => handAnimator.gameObject.activeSelf);

            highlightImage.transform.parent.localScale = Vector3.one * scale;
            highlightImage.transform.position = Camera.main.WorldToScreenPoint(worldPos);
            highlightImage.gameObject.SetActive(true);
            a.Play("higligUI");

            yield return 0;

            if (orientateToHandAnim)
            {
                if (handAnimator.gameObject.activeSelf)
                {
                    float handAnimLeng = handAnimator.GetCurrentAnimatorClipInfo(0)[0].clip.length;
                    float leng = a.GetCurrentAnimatorClipInfo(0)[0].clip.length;

                    if (handAnimLeng > 0) a.speed = leng / handAnimLeng;
                }
                else
                {
                    highlightImage.gameObject.SetActive(false);
                }
            }

            if (alignToHandPos)
            {
                if (handAnimator.gameObject.activeSelf)
                    highlightImage.transform.position = Camera.main.WorldToScreenPoint(handAnimator.transform.position);
                else
                    highlightImage.gameObject.SetActive(false);
            }
        }
    }

    public void CloseHighlight()
    {
        if(highlightImage) highlightImage.gameObject.SetActive(false);
    }
}

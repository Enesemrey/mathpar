using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // Statics
    public const float version = 0.011f;
    public const int firstSceneIndex = 1;
    public const int tutorialIndex = 1;
    public const string TutorialHash = "TutorialPlayed";
    public const string LevelIndexHash = "LevelIndex";
    public const string LevelHash = "Level";
    public static GameManager Instance;

    //Unity Events
    public UnityEvent GameStarted;
    public UnityEvent OnLevelFail;
    public UnityEvent OnLevelWin;

    //Game Flags
    public bool IsPlaying;
    public bool IsGameOver;
    private CharacterStack[] _characterStacks;
    public int MaxCount = int.MinValue;

    private bool _tutorialPlayed
    {
        get => PlayerPrefs.GetInt(TutorialHash + version, 0) == 1;
        set => PlayerPrefs.SetInt(TutorialHash + version, value ? 1 : 0);
    }
    private int _currentLevelIndex
    {
        get => PlayerPrefs.GetInt(LevelIndexHash + version, _tutorialPlayed ? tutorialIndex : firstSceneIndex);
        set => PlayerPrefs.SetInt(LevelIndexHash + version, value);
    }

    public int CurrentLevel
    {
        get => PlayerPrefs.GetInt(LevelHash + version, 1);
        set => PlayerPrefs.SetInt(LevelHash + version, value);
    }




    // Start is called before the first frame update
    void Awake()
    {
        Cursor.visible = false;
        Debug.Log("Index: " + _currentLevelIndex);
        Debug.Log("Level: " + CurrentLevel);
#if !UNITY_EDITOR
        if (_currentLevelIndex != SceneManager.GetActiveScene().buildIndex)
            SceneManager.LoadScene(_currentLevelIndex);
#endif

        if (Instance != null)
            Destroy(Instance.gameObject);
        Instance = this;
        if (_currentLevelIndex == tutorialIndex)
            _tutorialPlayed = true;

        Analytics.LevelStarted(_currentLevelIndex);
    }

    IEnumerator Start()
    {
        yield return new WaitUntil(() => Input.GetMouseButtonDown(0));
        IsPlaying = true;
        GameStarted.Invoke();
        _characterStacks = GameObject.FindObjectsOfType<CharacterStack>();
        var controllers = GameObject.FindObjectsOfType<Controller>();
        yield return new WaitUntil(() => _characterStacks.All(m => m.Count <= 0) && controllers.All(m => m.IsFinished));
        if (IsGameOver) yield break;
        //StartCoroutine(FinishLevel());
    }

    public IEnumerator FinishLevel()
    {
        IsPlaying = false;
        IsGameOver = true;
        VCamManager.Instance.PlayConfetti();
        yield return new WaitForSeconds(3);
        Win();
    }

    // Update is called once per frame


    public void Fail()
    {
        IsPlaying = false;
        IsGameOver = true;
        Analytics.LevelFailed(_currentLevelIndex);
        OnLevelFail.Invoke();
    }
    public void Win()
    {
        IsPlaying = false;
        IsGameOver = true;
        Analytics.LevelPassed(_currentLevelIndex);
        OnLevelWin.Invoke();
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void NextLevel()
    {
        CurrentLevel++;
        var isRandomLevel = CurrentLevel > SceneManager.sceneCountInBuildSettings - firstSceneIndex;
        if (isRandomLevel)
        {
            _currentLevelIndex = Random.Range(firstSceneIndex, SceneManager.sceneCountInBuildSettings);
        }
        else
        {
            _currentLevelIndex++;
        }
        Debug.Log("Next Index: " + _currentLevelIndex);
        Debug.Log("Next Level: " + CurrentLevel);
        SceneManager.LoadScene(_currentLevelIndex);
    }

    public bool IsWin(int count)
    {
        return MaxCount <= count;
    }
}

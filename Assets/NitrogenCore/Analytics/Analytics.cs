using UnityEngine;
using System.Collections.Generic;


public class Analytics : MonoBehaviour
{
    public static Analytics instance;

    private void Awake()
    {
        if (instance != null & instance != this)
        {
            Destroy(this.gameObject);
            Debug.LogWarning("(Analytics) Duplicate Analytics object. Are you loading EntryScene twice?");
        }

        else
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);

            Debug.Log("(Analytics) Begin init.");

            StartGame();

        }
    }

    private void StartGame()
    {
        Debug.Log("(Analytics) Begin game.");

        if (!Application.isEditor)
        {
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = 60;
        }

        else
        {
            Application.targetFrameRate = 999;
        }

        UnityEngine.SceneManagement.SceneManager.LoadScene(2);
    }

   
    #region Logging
    public static void LevelPassed(int index)
    {
        string strIndex = index.ToString();

        if (instance != null)
        {
            Debug.Log("(Analytics) LevelPassed: " + strIndex);
        }

        else
        {
            Debug.Log("(Analytics - Dummy) LevelPassed: " + strIndex);
        }
    }

    public static void LevelFailed(int index)
    {
        string strIndex = index.ToString();

        if (instance != null)
        {

           

            Debug.Log("(Analytics) LevelFailed: " + strIndex);
        }

        else
        {
            Debug.Log("(Analytics - Dummy) LevelFailed: " + strIndex);
        }
    }

    public static void LevelStarted(int index)
    {
        string strIndex = index.ToString();

        if (instance != null)
        {
           
            Debug.Log("(Analytics) LevelStarted: " + strIndex);
        }

        else
        {
            Debug.Log("(Analytics - Dummy) LevelStarted: " + strIndex);
        }
    }

    public static void LogSession(int sceneIndex, string state, float duration)
    {
        string log = $"Level: {sceneIndex} - State: {state} - Duration:{duration}";

        if (instance != null)
        {

           
            Debug.Log("(Analytics) " + log);
        }

        else
        {
            Debug.Log("(Analytics - Dummy) " + log);
        }
    }
    #endregion
}
using System.Reflection.Emit;
using UnityEngine;

public class Cheater : MonoBehaviour
{
    private bool visible = false;
    [Header("Level Indexes")]
    public int minLevelIndex;
    public int maxLevelIndex;

    [Header("Tap Settings")]
    public int tapCount = 10;
    public float maxTapDelay = 3f;

    [Header("Misc.")]
    [Tooltip("Poll frames by this amount for fps and frametime calculation")]
    public int frameCount = 3;
    public Texture2D labelBackground;

    private float xMargin;
    private float yMargin;

    private int currentTapCount = 0;

    private GUIStyle buttonStlye;
    private GUIStyle labelStyle;
    private GUIStyle fpsLabelStyle;
    private GUIStyle areaStyle;

    private int selector;
    private float tapTimeCounter = 0;

    private int currentFrameCount = 0;
    private float accumulatedTime = 0;
    private float fps = 0;
    private float avgFrameTime = 0;

    void Start()
    {
        selector = minLevelIndex;

        xMargin = Screen.width * 0.8f;
        yMargin = Screen.height * 0.2f;

        buttonStlye = new GUIStyle("button");
        buttonStlye.fontSize = (int)(Screen.height * 0.025f);

        labelStyle = new GUIStyle("label");
        labelStyle.fontSize = (int)(Screen.height * 0.025f);
        labelStyle.alignment = TextAnchor.MiddleCenter;
        labelStyle.normal.textColor = Color.red;

        fpsLabelStyle = new GUIStyle("label");
        fpsLabelStyle.fontSize = (int)(Screen.height * 0.0175f);
        fpsLabelStyle.alignment = TextAnchor.MiddleCenter;
        fpsLabelStyle.normal.textColor = Color.blue;

        areaStyle = new GUIStyle();
        areaStyle.normal.background = labelBackground;
    }

    void Update()
    {
        if (!visible)
        {
            tapTimeCounter = tapTimeCounter + Time.deltaTime;

            if (Input.GetMouseButtonDown(0))
            {
                if ((Input.mousePosition.x > xMargin) & (Input.mousePosition.y < yMargin))
                {
                    currentTapCount++;
                    tapTimeCounter = 0f;

                    if (currentTapCount == tapCount)
                    {
                        visible = true;
                        currentTapCount = 0;
                    }
                }
            }

            if (tapTimeCounter > maxTapDelay)
            {
                currentTapCount = 0;
            }
        }

        currentFrameCount++;

        if (currentFrameCount > frameCount)
        {
            currentFrameCount = 0;
            avgFrameTime = accumulatedTime / frameCount;
            fps = 1 / avgFrameTime;
            accumulatedTime = 0;
        }

        else
        {
            accumulatedTime = accumulatedTime + Time.unscaledDeltaTime;
        }
    }

    private void OnGUI()
    {
        if (visible)
        {
            float width = Screen.width * 0.3f;
            float height = Screen.height * 0.6f;

            float buttonHeight = Screen.height * 0.1f;

            //
            // Scene Hopping
            //
            GUILayout.BeginArea(new Rect((Screen.width / 2) - (width / 2), (Screen.height / 2) - (height / 2), width, height), areaStyle);

            GUILayout.BeginVertical();
            GUILayout.Label("Go To: " + selector.ToString(), labelStyle, GUILayout.Height(buttonHeight));

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("+", buttonStlye, GUILayout.Height(buttonHeight)))
            {
                if (selector < maxLevelIndex) selector++;
            }

            if (GUILayout.Button("-", buttonStlye, GUILayout.Height(buttonHeight)))
            {
                if (selector > minLevelIndex) selector--;
            }
            GUILayout.EndHorizontal();

            GUILayout.Label("\n", labelStyle, GUILayout.Height(buttonHeight));

            if (GUILayout.Button("Go To Stage", buttonStlye, GUILayout.Height(buttonHeight)))
            {
                GoToScene(selector);
            }

            if (GUILayout.Button("Close", buttonStlye, GUILayout.Height(buttonHeight)))
            {
                Close();
            }
            
            GUILayout.EndVertical();
            GUILayout.EndArea();

            //
            // FPS Counter
            //
            GUILayout.BeginArea(new Rect((Screen.width / 2) - (width / 2), Screen.height * 0.05f, width, height / 5), areaStyle);
            GUILayout.BeginVertical();

            GUILayout.Label("FPS: " + fps.ToString("0.00"), fpsLabelStyle);
            GUILayout.Label("Avg. Frame Time (ms): " + (avgFrameTime * 1000).ToString("0.000"), fpsLabelStyle);

            GUILayout.EndVertical();
            GUILayout.EndArea();
        }
    }

    private void GoToScene(int index)
    {
        Close();
        //implement scene hopping logic below here//
        throw new System.NotImplementedException();
    }

    private void Close()
    {
        visible = false;
    }
}

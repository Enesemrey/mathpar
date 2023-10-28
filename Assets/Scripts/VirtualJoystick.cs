using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class VirtualJoystick : MonoBehaviour, IPointerUpHandler, IPointerDownHandler, IDragHandler
{
    [Header("ATTRIBUTES")]
    [SerializeField] private JoystickType type = JoystickType.Std;

    // When not tapped, makes joystick return to default zero position
    [SerializeField] bool springedJoystick = true;

    // Do not show joystick images
    [SerializeField] bool hidden = false;

    // Calculates axis
    [SerializeField] bool calculateAxis_4 = false;
    [SerializeField] bool calculateAxis_8 = false;

    // Limit range of joystick tip from center of pad
    [SerializeField] private float padRange = 150f;

    [Header("INDICATORS"), Space]
    public bool joystickInUse = false;
    public bool joystickTapped = false;
    public bool joystickDragging = false;
    public Axis_4 axis_4 = Axis_4.Zero;
    public Axis_8 axis_8 = Axis_8.Zero;
    [SerializeField] private Vector3 input;
    [SerializeField] private float joystickAngle;

    [Header("REFERENCES"), Space]
    [SerializeField] private Image joystickImage;
    [SerializeField] private Image padImage;
    public static VirtualJoystick instance;
    private Camera mainCam;
    private Image touchPanel;

    public enum JoystickType { Std, Floating, Fixed };

    private void Awake()
    {
        //Singleton
        if (instance != null) Destroy(instance);
        instance = this;

        mainCam = Camera.main;
        touchPanel = GetComponent<Image>();
    }

    private void OnEnable()
    {
        if (type != JoystickType.Fixed)
        {
            padImage.gameObject.SetActive(false);
            joystickImage.gameObject.SetActive(false);
        }

        joystickDragging = joystickInUse = joystickTapped = false;
        input = Vector3.zero;
    }

    private void FixedUpdate()
    {
        if (type == JoystickType.Floating)
        {
            if ((joystickImage.transform.position - padImage.transform.position).magnitude > padRange)
                padImage.rectTransform.anchoredPosition = Vector2.Lerp(padImage.rectTransform.anchoredPosition, joystickImage.rectTransform.anchoredPosition, 0.05f);
        }
    }


    public virtual void OnPointerDown(PointerEventData ped)
    {
        Vector2 pos = Vector2.zero;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(touchPanel.rectTransform, ped.position, ped.pressEventCamera, out pos))
        {
            if (!hidden)
            {
                padImage.gameObject.SetActive(true);
                joystickImage.gameObject.SetActive(true);
            }

            if (type != JoystickType.Fixed)
            {
                padImage.rectTransform.anchoredPosition = pos;
                joystickImage.rectTransform.anchoredPosition = pos;
            }
            else
            {
                Vector2 dif = pos - padImage.rectTransform.anchoredPosition;
                if (dif.magnitude > padRange)
                {
                    Vector2 newPos = padImage.rectTransform.anchoredPosition + Vector2.ClampMagnitude(dif, padRange);
                    joystickImage.rectTransform.anchoredPosition = newPos;
                }
                else
                {
                    joystickImage.rectTransform.anchoredPosition = pos;
                }
            }

            joystickTapped = joystickInUse = true;
            joystickDragging = false;
        }
    }


    public virtual void OnDrag(PointerEventData ped)
    {
        Vector2 dragPos = default;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(touchPanel.rectTransform, ped.position, ped.pressEventCamera, out dragPos))
        {
            Vector2 dif = dragPos - padImage.rectTransform.anchoredPosition;
            float magnitude = dif.magnitude;
            if (type == JoystickType.Fixed || type != JoystickType.Floating)
            {
                if (magnitude > padRange)
                {
                    dragPos = padImage.rectTransform.anchoredPosition + Vector2.ClampMagnitude(dif, padRange);
                    if (calculateAxis_4) GetAxis_4();
                    if (calculateAxis_8) GetAxis_8();
                }
            }

            joystickImage.rectTransform.anchoredPosition = dragPos;

            //Result input
            input = (joystickImage.rectTransform.anchoredPosition - padImage.rectTransform.anchoredPosition) / padRange;
            joystickAngle = Mathf.Atan2(input.y, input.x);

            joystickDragging = true;
        }
    }

    public virtual void OnPointerUp(PointerEventData ped)
    {
        joystickTapped = joystickDragging = false;
        joystickAngle = 0f;

        if (springedJoystick)
        {
            joystickImage.rectTransform.anchoredPosition = padImage.rectTransform.anchoredPosition;
            input = Vector3.zero;
            axis_4 = Axis_4.Zero;
            axis_8 = Axis_8.Zero;
        }

        if (type != JoystickType.Fixed)
        {
            joystickInUse = false;
            padImage.gameObject.SetActive(false);
            joystickImage.gameObject.SetActive(false);
        }
    }

    public Vector3 GetJoystickDirection()
    {
        return input;
    }

    /// <summary>
    /// Returns angle from right in radians ( -PI --> PI )
    /// </summary>
    /// <returns></returns>
    public float GetJoystickAngleInRadian()
    {
        return joystickAngle;
    }

    /// <summary>
    /// Returns angle from right in degrees ( 0 --> 359 )
    /// </summary>
    /// <returns></returns>
    public float GetJoystickAngleInDegrees()
    {
        joystickAngle = ((joystickAngle * Mathf.Rad2Deg) + 360f) % 360f;
        return joystickAngle;
    }

    public Vector3 GetDirectionOnGround()
    {
        Quaternion camRot = instance.mainCam.transform.rotation;
        Quaternion groundRot = new Quaternion(0f, camRot.y, 0f, camRot.w);
        Quaternion difRot = Quaternion.RotateTowards(camRot, groundRot, 360f);

        return difRot * instance.input;
    }

    public Axis_8 GetAxis_8()
    {
        Axis_8 axis = Axis_8.Zero;
        float pi = Mathf.PI;
        if (joystickAngle <= pi * 0.125f && joystickAngle > -pi * 0.125f) axis = Axis_8.Right;
        else if (joystickAngle <= pi * 0.375f && joystickAngle > pi * 0.125f) axis = Axis_8.UpRight;
        else if (joystickAngle <= pi * 0.625f && joystickAngle > pi * 0.375f) axis = Axis_8.Up;
        else if (joystickAngle <= pi * 0.875f && joystickAngle > pi * 0.625f) axis = Axis_8.UpLeft;
        else if (joystickAngle <= pi * -0.625f && joystickAngle > pi * -0.875f) axis = Axis_8.DownLeft;
        else if (joystickAngle <= pi * -0.375f && joystickAngle > pi * -0.625f) axis = Axis_8.Down;
        else if (joystickAngle <= pi * -0.125f && joystickAngle > pi * -0.375f) axis = Axis_8.DownRight;
        else if (joystickAngle > pi * 0.875f || joystickAngle <= pi * -0.875f) axis = Axis_8.Left;
        axis_8 = axis;
        return axis;
    }

    public Axis_4 GetAxis_4()
    {
        Axis_4 axis = Axis_4.Zero;
        float pi = Mathf.PI;
        if (joystickAngle <= pi * 0.25f && joystickAngle > -pi * 0.25f) axis = Axis_4.Right;
        else if (joystickAngle <= pi * 0.75f && joystickAngle > pi * 0.25f) axis = Axis_4.Up;
        else if (joystickAngle <= pi * -0.25f && joystickAngle > pi * -0.75f) axis = Axis_4.Down;
        else if (joystickAngle > pi * 0.75f || joystickAngle <= pi * -0.75f) axis = Axis_4.Left;
        axis_4 = axis;
        return axis;
    }

    public enum Axis_8
    {
        Zero,
        Up,
        UpRight,
        Right,
        DownRight,
        Down,
        DownLeft,
        Left,
        UpLeft,
    }

    public enum Axis_4
    {
        Zero,
        Up,
        Right,
        Down,
        Left,
    }
}


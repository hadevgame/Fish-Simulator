using System;
using System.Collections;
using System.Collections.Generic;
using UCExtension;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ControlPositionJoyStick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    [SerializeField] protected Joystick _mJoyStick;

    [SerializeField] bool disableWhenNotUse;

    [SerializeField] GameObject tutorial;

    protected RectTransform JoyStickParent;

    protected RectTransform JoyStickRect;

    protected EventSystem eventSystem;

    protected Canvas _mCanvas;

    protected RectTransform _mCanvasRect;

    protected Camera _mCamera;

    public static Action<Vector2> OnChangeMoveInput;

    public static Action OnStartTouch;

    protected RectTransform rect;

    // Start is called before the first frame update
    #region Unity Build-in
    private void Awake()
    {
        JoyStickParent = GetComponent<RectTransform>();
        SetMainJoyStick(_mJoyStick);
    }

    public void SetMainJoyStick(Joystick joystick)
    {
        if (_mJoyStick)
        {
            _mJoyStick.OnPointerUpCallBack -= OnPointerUpCallback;
            _mJoyStick.OnPointerDownCallBack -= OnPointerDownCallback;
            _mJoyStick.OnDragCallBack -= OnDragCallback;
        }
        _mJoyStick = joystick;
        JoyStickRect = _mJoyStick.GetComponent<RectTransform>();
        _mCamera = Camera.main;
        eventSystem = FindObjectOfType<EventSystem>();
        _mCanvas = GetComponentInParent<Canvas>();
        _mCanvasRect = _mCanvas.GetComponentInParent<RectTransform>();
        _mJoyStick.OnPointerUpCallBack += OnPointerUpCallback;
        _mJoyStick.OnPointerDownCallBack += OnPointerDownCallback;
        _mJoyStick.OnDragCallBack += OnDragCallback;
        Init();
        _mJoyStick.SetUp();
        _mJoyStick.gameObject.SetActive(!disableWhenNotUse);
    }

    private void Update()
    {
        float xAxis = Input.GetAxis("Horizontal");
        float yAxis = Input.GetAxis("Vertical");
        Vector2 moveInput = new Vector2(xAxis, yAxis);
        moveInput.Normalize();
        if (moveInput.sqrMagnitude < 0.1f)
        {
            moveInput = new Vector2(_mJoyStick.Horizontal, _mJoyStick.Vertical);
        }
        OnChangeMoveInput?.Invoke(moveInput);
    }
    private void OnDisable()
    {
        _mJoyStick.OnPointerUp(new PointerEventData(eventSystem));
        _mJoyStick.gameObject.SetActive(!disableWhenNotUse);
    }

    #endregion
    protected virtual void Init()
    {

    }
    protected virtual void OnPointerUpCallback()
    {

    }
    protected virtual void OnPointerDownCallback()
    {

    }
    protected virtual void OnDragCallback()
    {

    }

    public virtual void OnPointerUp(PointerEventData eventData)
    {
        _mJoyStick.OnPointerUp(eventData);
        _mJoyStick.gameObject.SetActive(!disableWhenNotUse);
    }

    public virtual void OnPointerDown(PointerEventData eventData)
    {
        OnStartTouch?.Invoke();
        Vector3 screenPos = eventData.pressPosition;
        JoyStickRect.anchoredPosition = GetAnchorPos(JoyStickParent, eventData.pointerCurrentRaycast.screenPosition);
        _mJoyStick.OnPointerDown(eventData);
        _mJoyStick.gameObject.SetActive(true);
        if (tutorial)
        {
            tutorial.SetActive(false);
        }
    }

    public virtual void OnDrag(PointerEventData eventData)
    {
        _mJoyStick.OnDrag(eventData);
        float x = _mJoyStick.Horizontal;
        float y = _mJoyStick.Vertical;
    }

    public Vector2 GetAnchorPos(RectTransform UIElementParent, Vector3 screenPos)
    {
        Vector2 anchoredPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(UIElementParent, screenPos, _mCanvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : _mCamera, out anchoredPos);
        return anchoredPos;
    }

    private void OnApplicationFocus(bool focus)
    {
        OnPointerUp(new PointerEventData(EventSystem.current));
    }

    private void OnApplicationPause(bool pause)
    {
        OnPointerUp(new PointerEventData(EventSystem.current));
    }

}

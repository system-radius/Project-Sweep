using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class TouchManager : MonoBehaviour
{
    [SerializeField]
    private Camera worldCamera;

    private PlayerInput input;
    private InputAction tapAction;
    private InputAction holdAction;
    private InputAction pauseAction;
    private InputAction resumeAction;
    private InputAction restartAction;

    private InputAction primaryTouchContact;
    private InputAction secondaryTouchContact;

    private InputAction moveCamera;

    public InputAction primaryTouchPosition;
    public InputAction secondaryTouchPosition;
    //private InputAction touchPressAction;

    public event Action<Vector3> OnTriggerTap;
    public event Action<Vector3> OnTriggerHold;
    public event Action OnTriggerPause;
    public event Action OnTriggerResume;
    public event Action OnTriggerRestart;
    public event Action OnZoomStart;
    public event Action OnZoomEnd;

    public event Action<Vector2> OnCameraMove;

    private bool activeGameplayControls = true;
    private bool zooming = false;
    private bool startTrackDrag = false;
    private bool dragging = false;

    private Vector2 prevTouchPoint = Vector2.zero;

    private void Awake()
    {
        input = GetComponent<PlayerInput>();
        tapAction = input.actions["Tap"];
        holdAction = input.actions["Hold"];

        primaryTouchContact = input.actions["PrimaryTouchContact"];
        primaryTouchPosition = input.actions["PrimaryTouchPosition"];

        pauseAction = input.actions["Pause"];
        resumeAction = input.actions["Resume"];
        restartAction = input.actions["Restart"];

        secondaryTouchContact = input.actions["SecondaryTouchContact"];
        secondaryTouchPosition = input.actions["SecondaryTouchPosition"];

        moveCamera = input.actions["MoveCamera"];
    }

    private void OnEnable()
    {
        tapAction.performed += OnScreenTap;
        holdAction.performed += OnScreenHold;
        pauseAction.performed += TriggerPause;
        resumeAction.performed += TriggerResume;
        restartAction.performed += TriggerRestart;

        primaryTouchContact.started += StartMovementTracking;
        primaryTouchContact.canceled += StopMovementTracking;
        primaryTouchPosition.performed += TrackCameraMovement;

        secondaryTouchContact.started += ZoomStart;
        secondaryTouchContact.canceled += ZoomEnd;
    }

    private void OnDisable()
    {
        tapAction.performed -= OnScreenTap;
        holdAction.performed -= OnScreenHold;
        pauseAction.performed -= TriggerPause;
        resumeAction.performed -= TriggerResume;
        restartAction.performed -= TriggerRestart;

        primaryTouchContact.started -= StartMovementTracking;
        primaryTouchContact.canceled -= StopMovementTracking;
        primaryTouchPosition.performed -= TrackCameraMovement;

        secondaryTouchContact.started -= ZoomStart;
        secondaryTouchContact.canceled -= ZoomEnd;
    }

    private void StartMovementTracking(InputAction.CallbackContext context)
    {
        if (!activeGameplayControls || zooming) return;
        //prevTouchPoint = worldCamera.ScreenToWorldPoint(primaryTouchPosition.ReadValue<Vector2>());
        prevTouchPoint = primaryTouchPosition.ReadValue<Vector2>();
        startTrackDrag = true;
    }

    private void StopMovementTracking(InputAction.CallbackContext context)
    {
        startTrackDrag = false;
        if (!dragging || !activeGameplayControls || zooming) return;
        dragging = false;
        ActivateGameplayControls();
    }

    private void TrackCameraMovement(InputAction.CallbackContext context)
    {
        if (!startTrackDrag) return;

        float movementSpeed = 25f;
        //Vector2 touchPoint = worldCamera.ScreenToWorldPoint(primaryTouchPosition.ReadValue<Vector2>());
        Vector2 touchPoint = primaryTouchPosition.ReadValue<Vector2>();
        //Vector2 delta = new Vector2((touchPoint.x - prevTouchPoint.x) * movementSpeed, (touchPoint.y - prevTouchPoint.y) * movementSpeed);
        Vector2 screenDelta = touchPoint - prevTouchPoint;
        if (screenDelta.sqrMagnitude < 100f) return;
        Vector2 delta = (worldCamera.ScreenToWorldPoint(touchPoint) - worldCamera.ScreenToWorldPoint(touchPoint - screenDelta)) * movementSpeed;

        // Check the current delta with threshold to avoid accidental drags when the intent is to tap/hold.
        if (startTrackDrag && !dragging) dragging = true;

        // If the flag is set, proceed to actually moving the camera.
        if (!dragging) return;

        prevTouchPoint = touchPoint;
        OnCameraMove?.Invoke(delta);
    }

    private void ZoomStart(InputAction.CallbackContext context)
    {
        if (!activeGameplayControls) return;
        zooming = true;
        OnZoomStart?.Invoke();
    }

    private void ZoomEnd(InputAction.CallbackContext context)
    {
        if (!zooming) return;
        ActivateGameplayControls();
        OnZoomEnd?.Invoke();
    }

    private void TriggerPause(InputAction.CallbackContext context)
    {
        OnTriggerPause?.Invoke();
        activeGameplayControls = false;
    }

    private void TriggerResume(InputAction.CallbackContext context)
    {
        OnTriggerResume?.Invoke();
        ActivateGameplayControls();
    }

    private void TriggerRestart(InputAction.CallbackContext context)
    {
        OnTriggerRestart?.Invoke();
        ActivateGameplayControls();
    }

    private void OnScreenTap(InputAction.CallbackContext context)
    {
        if (!activeGameplayControls || zooming || dragging) return;
        Vector3 position = worldCamera.ScreenToWorldPoint(primaryTouchPosition.ReadValue<Vector2>());
        position.z = 0;
        //Debug.Log("[TAP] Performed @: " + position);
        OnTriggerTap?.Invoke(position);
    }

    private void OnScreenHold(InputAction.CallbackContext context)
    {
        if (!activeGameplayControls || zooming || dragging) return;
        Vector3 position = worldCamera.ScreenToWorldPoint(primaryTouchPosition.ReadValue<Vector2>());
        position.z = 0;
        //Debug.Log("[HOLD] Performed @: " + position);
        OnTriggerHold?.Invoke(position);
    }

    private void ActivateGameplayControls()
    {
        StartCoroutine(ActivateGameplayControlsCoroutine());
    }

    IEnumerator ActivateGameplayControlsCoroutine()
    {
        yield return new WaitForSeconds(0.1f);
        activeGameplayControls = true;
        dragging = zooming = false;
    }
}

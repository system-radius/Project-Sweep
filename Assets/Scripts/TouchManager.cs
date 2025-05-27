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
    private InputAction secondaryTouchContact;

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


    private bool activeGameplayControls = true;
    private bool zooming = false;

    private void Awake()
    {
        input = GetComponent<PlayerInput>();
        tapAction = input.actions["Tap"];
        holdAction = input.actions["Hold"];
        primaryTouchPosition = input.actions["Position"];

        pauseAction = input.actions["Pause"];
        resumeAction = input.actions["Resume"];
        restartAction = input.actions["Restart"];

        secondaryTouchContact = input.actions["SecondaryTouchContact"];
        secondaryTouchPosition = input.actions["SecondaryTouchPosition"];
    }

    private void OnEnable()
    {
        tapAction.performed += OnScreenTap;
        holdAction.performed += OnScreenHold;
        pauseAction.performed += TriggerPause;
        resumeAction.performed += TriggerResume;
        restartAction.performed += TriggerRestart;

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

        secondaryTouchContact.started -= ZoomStart;
        secondaryTouchContact.canceled -= ZoomEnd;
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
        if (!activeGameplayControls || zooming) return;
        Vector3 position = worldCamera.ScreenToWorldPoint(primaryTouchPosition.ReadValue<Vector2>());
        position.z = 0;
        //Debug.Log("[TAP] Performed @: " + position);
        OnTriggerTap?.Invoke(position);
    }

    private void OnScreenHold(InputAction.CallbackContext context)
    {
        if (!activeGameplayControls || zooming) return;
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
        zooming = false;
    }
}

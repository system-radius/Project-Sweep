using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class TouchManager : MonoBehaviour
{
    [SerializeField]
    private Camera worldCamera;

    private PlayerInput input;
    private InputAction tapAction;
    private InputAction holdAction;
    private InputAction touchPosition;
    private InputAction pauseAction;
    private InputAction resumeAction;
    private InputAction restartAction;

    private InputAction secondaryTouchContact;
    private InputAction secondaryTouchPosition;
    //private InputAction touchPressAction;

    public event Action<Vector3> OnTriggerTap;
    public event Action<Vector3> OnTriggerHold;
    public event Action OnTriggerPause;
    public event Action OnTriggerResume;
    public event Action OnTriggerRestart;

    private void Awake()
    {
        input = GetComponent<PlayerInput>();
        tapAction = input.actions["Tap"];
        holdAction = input.actions["Hold"];
        touchPosition = input.actions["Position"];

        pauseAction = input.actions["Pause"];
        resumeAction = input.actions["Resume"];
        restartAction = input.actions["Restart"];
    }

    private void OnEnable()
    {
        tapAction.performed += OnScreenTap;
        holdAction.performed += OnScreenHold;
        pauseAction.performed += TriggerPause;
        resumeAction.performed += TriggerResume;
        restartAction.performed += TriggerRestart;
    }

    private void OnDisable()
    {
        tapAction.performed -= OnScreenTap;
        holdAction.performed -= OnScreenHold;
        pauseAction.performed -= TriggerPause;
        resumeAction.performed -= TriggerResume;
        restartAction.performed -= TriggerRestart;
    }

    private void TriggerPause(InputAction.CallbackContext context)
    {
        //Debug.Log("Game is paused!");
        OnTriggerPause?.Invoke();
    }

    private void TriggerResume(InputAction.CallbackContext context)
    {
        OnTriggerResume?.Invoke();
    }

    private void TriggerRestart(InputAction.CallbackContext context)
    {
        OnTriggerRestart?.Invoke();
    }

    private void OnScreenTap(InputAction.CallbackContext context)
    {
        Vector3 position = worldCamera.ScreenToWorldPoint(touchPosition.ReadValue<Vector2>());
        position.z = 0;
        //Debug.Log("[TAP] Performed @: " + position);
        OnTriggerTap?.Invoke(position);
    }

    private void OnScreenHold(InputAction.CallbackContext context)
    {
        Vector3 position = worldCamera.ScreenToWorldPoint(touchPosition.ReadValue<Vector2>());
        position.z = 0;
        //Debug.Log("[HOLD] Performed @: " + position);
        OnTriggerHold?.Invoke(position);
    }

}

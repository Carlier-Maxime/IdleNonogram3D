using UnityEngine;
using UnityEngine.InputSystem;

public class ClickDetector : MonoBehaviour
{
    [SerializeField]
    private InputActionReference clickAction;
    [SerializeField]
    private float threshold = 0.0025f;
    private Vector2 _startPos;

    private void OnEnable()
    {
        clickAction.action.Enable();
        clickAction.action.started += OnClickStart;
        clickAction.action.canceled += OnClickPerformed;
    }

    private void OnClickStart(InputAction.CallbackContext obj)
    {
        _startPos = Utils.NormalizeSreenCoords(Player.P1.posAction.action.ReadValue<Vector2>());
    }

    private void OnClickPerformed(InputAction.CallbackContext obj)
    {
        var pos = Utils.NormalizeSreenCoords(Player.P1.posAction.action.ReadValue<Vector2>());
        if ((pos-_startPos).magnitude > threshold) return;
        Debug.Log("Click detected "+_startPos);
    }

    private void OnDisable()
    {
        clickAction.action.Disable();
        clickAction.action.started -= OnClickStart;
        clickAction.action.canceled -= OnClickPerformed;
    }
}
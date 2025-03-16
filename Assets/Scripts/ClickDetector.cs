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
        Player.P1.posAction.action.performed += OnPerformStartPos;
    }

    private void OnPerformStartPos(InputAction.CallbackContext obj)
    {
        _startPos = Utils.NormalizeSreenCoords(Player.P1.posAction.action.ReadValue<Vector2>());
        Player.P1.posAction.action.performed -= OnPerformStartPos;
    }

    private void OnClickPerformed(InputAction.CallbackContext obj)
    {
        Player.P1.posAction.action.performed -= OnPerformStartPos;
        var rawPos = Player.P1.posAction.action.ReadValue<Vector2>();
        if (_startPos == Vector2.zero) _startPos = Utils.NormalizeSreenCoords(rawPos);
        var pos = Utils.NormalizeSreenCoords(rawPos);
        if ((pos - _startPos).magnitude > threshold)
        {
            _startPos = Vector2.zero;
            return;
        }
        var screenPos = Utils.UnNormalizeScreenCoords(_startPos);
        _startPos = Vector2.zero;
        var ray = GetComponent<Camera>().ScreenPointToRay(screenPos);
        RaycastHit hit;
        if (!Physics.Raycast(ray, out hit)) return;
        var cell = hit.collider.GetComponentInParent<Cell>();
        if (!cell) return;
        cell.DestroyCell(true);
    }

    private void OnDisable()
    {
        clickAction.action.Disable();
        clickAction.action.started -= OnClickStart;
        clickAction.action.canceled -= OnClickPerformed;
    }
}
using UnityEngine;

public class Cell : MonoBehaviour
{
    private bool _isPure;
    public void SetPure(bool isPure)
    {
        _isPure = isPure;
        transform.localScale *= _isPure ? 1f : 0.25f;
    }

    public void Destroy()
    {
        Destroy(gameObject);
    }
}
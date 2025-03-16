using UnityEngine;

public class Cell : MonoBehaviour
{
    [SerializeField]
    private GameObject particleBreakPrefab;
    private bool _isPure;
    public void SetPure(bool isPure)
    {
        _isPure = isPure;
        //transform.localScale *= _isPure ? 1f : 0.25f;
    }

    public void Destroy()
    {
        if (particleBreakPrefab)
        {
            var effect = Instantiate(particleBreakPrefab, transform.position, Quaternion.identity);
            Destroy(effect, 0.5f);
        }
        Destroy(gameObject);
    }
}
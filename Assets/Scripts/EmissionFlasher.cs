using UnityEngine;
using System.Collections;

public class EmissionFlasher : MonoBehaviour
{
    private static readonly int EmissionColor = Shader.PropertyToID("_EmissionColor");
    public Color startColor = Color.yellow;
    public Color endColor = Color.orange;
    public float transitionTime = 1f;
    public float delayBetweenFlashes = 1f;

    private Renderer rend;
    private MaterialPropertyBlock propBlock;

    private void Awake()
    {
        rend = GetComponent<Renderer>();
        propBlock = new MaterialPropertyBlock();
        
        rend.GetPropertyBlock(propBlock);
        propBlock.SetColor(EmissionColor, startColor);
        rend.SetPropertyBlock(propBlock);
        
        StartCoroutine(FlashCoroutine());
    }

    private IEnumerator FlashCoroutine()
    {
        while (true)
        {
            yield return StartCoroutine(LerpColor(startColor, endColor, transitionTime));
            yield return new WaitForSeconds(delayBetweenFlashes);
        }
        // ReSharper disable once IteratorNeverReturns
    }

    private IEnumerator LerpColor(Color fromColor, Color toColor, float duration)
    {
        var startTime = Time.time;
        var elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed = Time.time - startTime;
            var t = Mathf.Clamp01(elapsed / duration);
            var currentColor = Color.Lerp(fromColor, toColor, t);
            
            rend.GetPropertyBlock(propBlock);
            propBlock.SetColor(EmissionColor, currentColor);
            rend.SetPropertyBlock(propBlock);

            yield return null;
        }
        
        rend.GetPropertyBlock(propBlock);
        propBlock.SetColor(EmissionColor, toColor);
        rend.SetPropertyBlock(propBlock);
        
        var temp = startColor;
        startColor = toColor;
        endColor = temp;
    }
}
using System.Runtime.CompilerServices;
using Unity.Mathematics;
using UnityEngine;

public class Ore : MonoBehaviour
{
    [SerializeField]
    private GameObject cellPrefab;
    [SerializeField]
    private int width = 5;
    [SerializeField]
    private int height = 5;
    [SerializeField]
    private int depth = 5;
    [SerializeField]
    private float density = 0.5f;

    private void Start()
    {
        if (!cellPrefab.GetComponent<Cell>()) Debug.LogError("The Prefab used not content a component Cell.");

        var seed = UnityEngine.Random.Range(0, 100000);
        Debug.Log("Ore seed : "+seed);
        var seedOffset = new float3(seed*0.01f, seed*0.01f, seed*0.01f);
        var startPosition = transform.position - new Vector3(ShiftOf(width), ShiftOf(height), ShiftOf(depth));
        for (var x = 0; x < width; x++)
        {
            for (var y = 0; y < height; y++)
            {
                for (var z = 0; z < depth; z++)
                {
                    if ((noise.snoise(seedOffset + new float3(x, y, z))+1)*0.5f > density) continue;
                    var spawnPosition = startPosition + new Vector3(x, y, z);
                    Instantiate(cellPrefab, spawnPosition, Quaternion.identity, transform);
                }
            }
        }
        return;

        // ReSharper disable once PossibleLossOfFraction
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static float ShiftOf(int x) => x/2 - ((x&1)==0 ? 0.5f : 0f);
    }
}
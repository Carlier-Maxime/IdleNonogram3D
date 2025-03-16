using System.Runtime.CompilerServices;
using Unity.Mathematics;
using UnityEngine;

public class Ore : MonoBehaviour
{
    public Color gridColor1;
    public Color gridColor2;
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
    private Cell[,,] _cells;

    private void Start()
    {
        if (!cellPrefab.GetComponent<Cell>()) Debug.LogError("The Prefab used not content a component Cell.");
    
        _cells = new Cell[width, height, depth];
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
                    var spawnPosition = startPosition + new Vector3(x, y, z);
                    _cells[x,y,z] = Instantiate(cellPrefab, spawnPosition, Quaternion.identity, transform).GetComponent<Cell>();
                    _cells[x,y,z].SetPure((noise.snoise(seedOffset + new float3(x, y, z))+1)*0.5f < density);
                }
            }
        }
        var lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.colorGradient = new Gradient
        {
            colorKeys = new GradientColorKey[]
            {
                new(gridColor1, 0.0f),
                new(gridColor2, 1.0f),
            }
        };
        lineRenderer.startWidth = 0.05f;
        lineRenderer.endWidth = 0.05f;
        lineRenderer.positionCount = 2*(width+height)+3;
        for (var x = 0; x <= width; ++x)
        {
            for (var y = 0; y < 2; ++y)
            {
                var i = x*2 + y;
                var posY = ((x&1) == 0 ? y : 1-y)*height - 0.5f;
                lineRenderer.SetPosition(i, startPosition + new Vector3(x-0.5f, posY, -0.5f));
            }
        }

        var offsetI = 2*width+1;
        for (var y = 0; y <= height; ++y)
        {
            for (var x = 0; x < 2; ++x)
            {
                var i = offsetI + y*2 + x;
                var posX = ((y&1) == 0 ? 1-x : x)*height - 0.5f;
                lineRenderer.SetPosition(i, startPosition + new Vector3(posX, y-0.5f, -0.5f));
            }
        }
        return;

        // ReSharper disable once PossibleLossOfFraction
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static float ShiftOf(int x) => x/2 - ((x&1)==0 ? 0.5f : 0f);
    }
}
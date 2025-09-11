using System;
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
    private FaceIndices[] _north;
    private FaceIndices[] _west;
    private FaceIndices[] _down;
    private FaceIndicesLabelService _labels;
    private int _localDepth = 0;
    private Vector3 _startPos;
    private float _defaultZ;

    private void Start()
    {
        if (!cellPrefab.GetComponent<Cell>()) Debug.LogError("The Prefab used not content a component Cell.");
        _defaultZ = transform.localPosition.z;
        _startPos = transform.position - new Vector3(ShiftOf(width), ShiftOf(height), ShiftOf(depth));
        var labelSize = 1.6f;
        var maxLen = Math.Max(Math.Max(width, height), depth) + labelSize;
        Player.P1.MoveForShowSouthFace(new Bounds(new Vector3(-labelSize/2, labelSize/2, 0), new Vector3(maxLen, maxLen, maxLen)));
        GenCells();
        InitializeFaceIndices();
        ComputeFaceIndices();
        DrawGrid();
        _labels = new FaceIndicesLabelService();
        _labels.CreateOrUpdate(width, height, _startPos);
        _labels.ShowFaceIndices(ref _north[0], 0);
        transform.position = new Vector3(transform.position.x, transform.position.y, _defaultZ-_localDepth);
        UpdateShowCells();
        var rotateComponent = GetComponent<RotateBySwipe>();
        if (rotateComponent) rotateComponent.OnRotateFinish += OnRotateFinish;
        return;

        // ReSharper disable once PossibleLossOfFraction
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static float ShiftOf(int x) => x/2 - ((x&1)==0 ? 0.5f : 0f);
    }

    private void GenCells()
    {
        _cells = new Cell[width, height, depth];
        var seed = UnityEngine.Random.Range(0, 100000); // 60198
        Debug.Log("Ore seed : "+seed);
        var seedOffset = new float3(seed*0.01f, seed*0.01f, seed*0.01f);
        for (var x = 0; x < width; x++)
        {
            for (var y = 0; y < height; y++)
            {
                for (var z = 0; z < depth; z++)
                {
                    var spawnPosition = _startPos + new Vector3(x, y, z);
                    _cells[x,y,z] = Instantiate(cellPrefab, spawnPosition, Quaternion.identity, transform).GetComponent<Cell>();
                    _cells[x,y,z].SetPure((noise.snoise(seedOffset + new float3(x, y, z))+1)*0.5f < density);
                }
            }
        }
    }

    private void InitializeFaceIndices()
    {
        _north = new FaceIndices[depth];
        for (var i = 0; i < depth; ++i) _north[i] = new FaceIndices(height, width);
        _west = new FaceIndices[width];
        for (var i = 0; i < width; ++i) _west[i] = new FaceIndices(height, depth);
        _down = new FaceIndices[height];
        for (var i = 0; i < height; ++i) _down[i] = new FaceIndices(depth, width);
    }

    private void ComputeFaceIndices()
    {
        for (var z = 0; z < depth; ++z)
        {
            var finalZ = z;
            _north[z].ComputeIndicesLineOrColumn(height, width, (minor, major) => _cells[minor, major, finalZ].IsPure(), false);
            _north[z].ComputeIndicesLineOrColumn(width, height, (minor, major) => _cells[major, minor, finalZ].IsPure(), true);
        }

        for (var x = 0; x < width; ++x)
        {
            var finalX = x;
            _west[x].ComputeIndicesLineOrColumn(height, depth, (minor, major) => _cells[finalX, minor, major].IsPure(), true, true);
            _west[x].ComputeIndicesLineOrColumn(depth, height, (minor, major) => _cells[finalX, major, minor ].IsPure(), false, false, true);
        }

        for (var y = 0; y < height; y++)
        {
            var finalY = y;
            _down[y].ComputeIndicesLineOrColumn(depth, width, (minor, major) => _cells[minor, finalY, major].IsPure(), false, true);
            _down[y].ComputeIndicesLineOrColumn( width, depth, (minor, major) => _cells[major, finalY, minor].IsPure(), true, false, true);
        }
    }

    private void DrawGrid()
    {
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
                lineRenderer.SetPosition(i, _startPos + new Vector3(x-0.5f, posY, -0.6f));
            }
        }

        var offsetI = 2*width+1;
        for (var y = 0; y <= height; ++y)
        {
            for (var x = 0; x < 2; ++x)
            {
                var i = offsetI + y*2 + x;
                var posX = ((y&1) == 0 ? 1-x : x)*height - 0.5f;
                lineRenderer.SetPosition(i, _startPos + new Vector3(posX, y-0.5f, -0.6f));
            }
        }
    }

    private void OnRotateFinish()
    {
        var cameraDir = (Player.P1.transform.position - transform.position).normalized;
        var localNormals = new[] {
            Vector3.back,
            Vector3.forward,
            Vector3.right,
            Vector3.left,
            Vector3.up,
            Vector3.down
        };
        var faceNames = new[] { "North", "Sud", "Est", "West", "Up", "Down" };
        Action<float>[] showFaces = {
            rotZ => _labels.ShowFaceIndices(ref _north[_localDepth], rotZ),
            rotZ => _labels.ShowFaceIndices(ref _north[depth - (1+_localDepth)], rotZ, true, false, false, true),
            rotZ => _labels.ShowFaceIndices(ref _west[width - (1+_localDepth)], rotZ, true, false, false, true),
            rotZ => _labels.ShowFaceIndices(ref _west[_localDepth], rotZ),
            rotZ => _labels.ShowFaceIndices(ref _down[height - (1+_localDepth)], rotZ, false, true, true),
            rotZ => _labels.ShowFaceIndices(ref _down[_localDepth], rotZ)
        };
        
        var maxDot = -Mathf.Infinity;
        var bestFace = -1;
        for (var i = 0; i < localNormals.Length; i++)
        {
            var worldNormal = transform.TransformDirection(localNormals[i]);
            var dot = Vector3.Dot(worldNormal, cameraDir);
            if (dot <= maxDot) continue;
            maxDot = dot;
            bestFace = i;
        }

        Vector3[] localRots =
        {
            new(0, 0, 0),
            new(0, 180, 0),
            new(0, 90, 0),
            new(0, 270, 0),
            new(270, 0, 0),
            new(90, 0, 0),
        };
        var rotBase = Quaternion.Euler(localRots[bestFace]);
        var rotSupplement = transform.rotation * Quaternion.Inverse(rotBase);
        var rotFace = rotSupplement.eulerAngles.z;

        int[] rotSign =
        {
            -1, 1, 1, -1, 0, -1
        };
        Debug.Log("Face : " + faceNames[bestFace] + ", rotFace : "+rotFace+", rotSign : "+rotSign[bestFace]+", rotSupp : "+rotSupplement.eulerAngles);
        showFaces[bestFace](rotFace*rotSign[bestFace]);
        UpdateShowCells();
    }

    private void UpdateShowCells()
    {
        for (var x = 0; x < width; ++x)
        {
            for (var y = 0; y < height; ++y)
            {
                for (var z = 0; z < depth; ++z)
                {
                    if (_cells[x, y, z] == null) continue;
                    var cellPos = _cells[x, y, z].transform.position;
                    _cells[x,y,z].gameObject.SetActive(cellPos.z > _startPos.z || Mathf.Approximately(_startPos.z, cellPos.z));
                }
            }
        }
    }

    public void DestroyAllCells()
    {
        for (var x = 0; x < width; ++x)
        {
            for (var y = 0; y < height; ++y)
            {
                for (var z = 0; z < depth; ++z)
                {
                    if (_cells[x, y, z] == null) continue;
                    _cells[x, y, z].DestroyCell(false);
                }
            }
        }
    }

    private void OnDestroy()
    {
        _labels?.Dispose();
    }
}
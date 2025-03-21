﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using TMPro;
using Unity.Mathematics;
using UnityEngine;

public class FaceIndices
{
    public FaceIndices(int nbLine, int nbColumn)
    {
        _lines = new List<int>[nbLine];
        for (var i = 0; i < nbLine; i++) _lines[i] = new List<int>();
        _columns = new List<int>[nbColumn];
        for (var i = 0; i < nbColumn; i++) _columns[i] = new List<int>();
    }
    private List<int>[] _lines;
    private List<int>[] _columns;

    public ref List<int> GetLine(int i)
    {
        return ref _lines[i];
    }
    
    public ref List<int> GetColumn(int i)
    {
        return ref _columns[i];
    }
}

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
    private TextMeshPro[] _lineTMP;
    private TextMeshPro[] _columnTMP;
    private int _localDepth = 0;
    private Vector3 _startPos;
    private float _defaultZ;

    private void Start()
    {
        if (!cellPrefab.GetComponent<Cell>()) Debug.LogError("The Prefab used not content a component Cell.");
        _defaultZ = transform.localPosition.z;
        _startPos = transform.position - new Vector3(ShiftOf(width), ShiftOf(height), ShiftOf(depth));
        GenCells();
        InitializeFaceIndices();
        InitializeTMP();
        ComputeFaceIndices();
        DrawGrid();
        ShowFaceIndices(ref _north[0], 0);
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

    private void InitializeTMP()
    {
        _lineTMP = new TextMeshPro[height];
        for (var i = 0; i < height; ++i)
        {
            var newGameObject = new GameObject();
            _lineTMP[i] = newGameObject.AddComponent<TextMeshPro>();
            _lineTMP[i].transform.position = _startPos + new Vector3(-1.5f, i, -0.5f);
            _lineTMP[i].text = "X X X";
            _lineTMP[i].alignment = TextAlignmentOptions.Center;
            _lineTMP[i].fontSize = 5;
            _lineTMP[i].color = Color.blue;
        }
        _columnTMP = new TextMeshPro[width];
        for (var i = 0; i < height; ++i)
        {
            var newGameObject = new GameObject();
            _columnTMP[i] = newGameObject.AddComponent<TextMeshPro>();
            _columnTMP[i].transform.position = _startPos + new Vector3(i, height+0.75f, -0.5f);
            _columnTMP[i].text = "X\nX\nX";
            _columnTMP[i].alignment = TextAlignmentOptions.Center;
            _columnTMP[i].fontSize = 5;
            _columnTMP[i].color = Color.blue;
        }
    }

    private void ComputeFaceIndices()
    {
        for (var z = 0; z < depth; ++z)
        {
            var finalZ = z;
            ComputeIndicesLineOrColumn(height, width, (minor, major) => _cells[minor, major, finalZ].IsPure(), ref _north[z], false);
            ComputeIndicesLineOrColumn(width, height, (minor, major) => _cells[major, minor, finalZ].IsPure(), ref _north[z], true);
        }

        for (var x = 0; x < width; ++x)
        {
            var finalX = x;
            ComputeIndicesLineOrColumn(height, depth, (minor, major) => _cells[finalX, minor, major].IsPure(), ref _west[x], true, true);
            ComputeIndicesLineOrColumn(depth, height, (minor, major) => _cells[finalX, major, minor ].IsPure(), ref _west[x], false, false, true);
        }

        for (var y = 0; y < height; y++)
        {
            var finalY = y;
            ComputeIndicesLineOrColumn(depth, width, (minor, major) => _cells[minor, finalY, major].IsPure(), ref _down[y], false, true);
            ComputeIndicesLineOrColumn( width, depth, (minor, major) => _cells[major, finalY, minor].IsPure(), ref _down[y], true, false, true);
        }
    }

    private void ComputeIndicesLineOrColumn(int majorSize, int minorSize, Func<int, int, bool> isPure, ref FaceIndices face, bool useColumn, bool inverseOrder = false, bool inverseMinor = false)
    {
        for (var major = 0; major < majorSize; ++major)
        {
            var index = inverseOrder ? majorSize - (1+major) : major;
            ref var line = ref useColumn ? ref face.GetColumn(index) : ref face.GetLine(index);
            var i = -1;
            var oldPure = false;
            for (var minor = 0; minor < minorSize; minor++)
            {
                var newPure = isPure(inverseMinor ? minorSize - (1+minor) : minor, major);
                if (newPure)
                {
                    if (!oldPure)
                    {
                        ++i;
                        line.Add(1);
                    } else line[i]++;
                }
                oldPure = newPure;
            }
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

    private void ShowFaceIndices(ref FaceIndices face, float rotZ, bool inverseColumnOrder = false, bool reverseColumn = false, bool inverseLineOrder = false, bool reverseLine = false)
    {
        if (rotZ < 0) rotZ += 360;
        bool shouldTranspose = false;
        if (Math.Abs(90 - rotZ) < 1)
        {
            shouldTranspose = true;
            inverseLineOrder = !inverseLineOrder;
            reverseColumn = !reverseColumn;
        }
        if (Math.Abs(180-rotZ) < 1)
        {
            inverseColumnOrder = !inverseColumnOrder;
            reverseColumn = !reverseColumn;
            inverseLineOrder = !inverseLineOrder;
            reverseLine = !reverseLine;
        }
        if (Math.Abs(270-rotZ) < 1)
        {
            shouldTranspose = true;
            inverseColumnOrder = !inverseColumnOrder;
            reverseLine = !reverseLine;
        }
        for (var x = 0; x < width; ++x)
        {
            var i = inverseColumnOrder ? width - (1 + x) : x;
            ref var column = ref shouldTranspose ? ref face.GetLine(i) : ref face.GetColumn(i);
            _columnTMP[x].text = string.Join('\n', reverseColumn ? column : column.AsEnumerable()!.Reverse());
        }

        for (var y = 0; y < height; ++y)
        {
            var i = inverseLineOrder ? height - (1 + y) : y;
            ref var line = ref shouldTranspose ? ref face.GetColumn(i) : ref face.GetLine(i);
            _lineTMP[y].text = string.Join(' ', reverseLine ? line.AsEnumerable()!.Reverse() : line);
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
            rotZ => ShowFaceIndices(ref _north[_localDepth], rotZ),
            rotZ => ShowFaceIndices(ref _north[depth - (1+_localDepth)], rotZ, true, false, false, true),
            rotZ => ShowFaceIndices(ref _west[width - (1+_localDepth)], rotZ, true, false, false, true),
            rotZ => ShowFaceIndices(ref _west[_localDepth], rotZ),
            rotZ => ShowFaceIndices(ref _down[height - (1+_localDepth)], rotZ, false, true, true),
            rotZ => ShowFaceIndices(ref _down[_localDepth], rotZ)
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
}
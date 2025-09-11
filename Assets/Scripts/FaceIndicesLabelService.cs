using System;
using System.Linq;
using TMPro;
using UnityEngine;

public class FaceIndicesLabelService : IDisposable
{
    private int _width = 5;
    private int _height = 5;
    private TextMeshPro[] _lineTMP;
    private TextMeshPro[] _columnTMP;

    public void CreateOrUpdate(int width, int height, Vector3 pos)
    {
        _width = width;
        _height = height;
        _lineTMP = new TextMeshPro[height];
        for (var i = 0; i < height; ++i)
        {
            var newGameObject = new GameObject();
            _lineTMP[i] = newGameObject.AddComponent<TextMeshPro>();
            _lineTMP[i].transform.position = pos + new Vector3(-1.5f, i, -0.5f);
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
            _columnTMP[i].transform.position = pos + new Vector3(i, height+0.75f, -0.5f);
            _columnTMP[i].text = "X\nX\nX";
            _columnTMP[i].alignment = TextAlignmentOptions.Center;
            _columnTMP[i].fontSize = 5;
            _columnTMP[i].color = Color.blue;
        }
    }
    
    public void ShowFaceIndices(ref FaceIndices face, float rotZ, bool inverseColumnOrder = false, bool reverseColumn = false, bool inverseLineOrder = false, bool reverseLine = false)
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
        for (var x = 0; x < _width; ++x)
        {
            var i = inverseColumnOrder ? _width - (1 + x) : x;
            ref var column = ref shouldTranspose ? ref face.GetLine(i) : ref face.GetColumn(i);
            _columnTMP[x].text = string.Join('\n', reverseColumn ? column : column.AsEnumerable()!.Reverse());
        }

        for (var y = 0; y < _height; ++y)
        {
            var i = inverseLineOrder ? _height - (1 + y) : y;
            ref var line = ref shouldTranspose ? ref face.GetColumn(i) : ref face.GetLine(i);
            _lineTMP[y].text = string.Join(' ', reverseLine ? line.AsEnumerable()!.Reverse() : line);
        }
    }
    
    public void Dispose()
    {
    }
}
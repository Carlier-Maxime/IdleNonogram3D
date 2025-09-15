using System;
using System.Linq;
using TMPro;
using UnityEngine;

public class FaceIndicesLabelService : IDisposable
{
    private int _width;
    private int _height;
    private TextMeshPro[] _lineTMP;
    private TextMeshPro[] _columnTMP;

    private TextMeshPro CreateTMP()
    {
        var tmp = new GameObject().AddComponent<TextMeshPro>();
        tmp.textWrappingMode = TextWrappingModes.NoWrap;
        tmp.enableAutoSizing = true;
        tmp.fontSizeMin = 2;
        tmp.fontSizeMax = 8;
        tmp.color = Color.blue;
        tmp.GetComponentInParent<RectTransform>().sizeDelta = new Vector2(1f, 1f);
        return tmp;
    }

    public void CreateOrUpdate(int width, int height, Vector3 pos)
    {
        _width = width;
        _height = height;
        _lineTMP = new TextMeshPro[height];
        for (var i = 0; i < height; ++i)
        {
            ref var tmp = ref _lineTMP[i];
            tmp = CreateTMP();
            tmp.transform.position = pos + new Vector3(-1.1f, i, -0.5f);
            tmp.text = "X X X";
            tmp.alignment = TextAlignmentOptions.Right;
        }
        _columnTMP = new TextMeshPro[width];
        for (var i = 0; i < height; ++i)
        {
            ref var tmp = ref _columnTMP[i];
            tmp = CreateTMP();
            tmp.transform.position = pos + new Vector3(i, height+0.02f, -0.5f);
            tmp.text = "X\nX\nX";
            tmp.alignment = TextAlignmentOptions.Bottom;
        }
    }
    
    public void ShowFaceIndices(ref FaceIndices face, float rotZ, bool inverseColumnOrder = false, bool reverseColumn = false, bool inverseLineOrder = false, bool reverseLine = false)
    {
        if (rotZ < 0) rotZ += 360;
        var shouldTranspose = false;
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
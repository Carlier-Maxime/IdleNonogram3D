using System;
using System.Collections.Generic;

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
    
    public void ComputeIndicesLineOrColumn(int majorSize, int minorSize, Func<int, int, bool> isPure, bool useColumn, bool inverseOrder = false, bool inverseMinor = false)
    {
        for (var major = 0; major < majorSize; ++major)
        {
            var index = inverseOrder ? majorSize - (1+major) : major;
            ref var line = ref useColumn ? ref GetColumn(index) : ref GetLine(index);
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
}
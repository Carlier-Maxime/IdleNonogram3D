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
}
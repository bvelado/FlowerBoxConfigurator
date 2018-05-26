using System;

[Serializable]
public struct Plank {

    private float _thickness;
    private float _height;
    private float _length;

    public float Thickness { get { return _thickness; } }
    public float Height { get { return _height; } }
    public float Length { get { return _length; } }

    public Plank(float thickness, float height, float length)
    {
        _thickness = thickness;
        _height = height;
        _length = length;
    }

    public Plank(RawPlank rawPlank, float length)
    {
        _thickness = rawPlank.Thickness;
        _height = rawPlank.Height;
        _length = length;
    }
}

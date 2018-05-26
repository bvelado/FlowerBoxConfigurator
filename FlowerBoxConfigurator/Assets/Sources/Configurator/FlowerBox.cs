using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowerBox : MonoBehaviour {

    [Header("Flower box dimensions")]
    [SerializeField] private float _width = 1200f;
    [SerializeField] private float _height = 400f;
    [SerializeField] private float _depth = 400f;
    [Space()]
    [SerializeField] private float _bottomThickness;
    [Header("Material")]
    [Space]
    [SerializeField] private RawPlank _plank;

    private float _interiorWidth;
    private float _exteriorDepth;
    private float _finalHeight;
    private float _finalDepth;
    private int _numberOfRows;

    private List<Plank> _result = new List<Plank>();
    private Plank _bigSidePlank;
    private Plank _smallSidePlank;
    private Plank _bottomPlank;

    private MeshFilter MeshFilter { get { return GetComponent<MeshFilter>(); } }

    public void Generate()
    {
        _result.Clear();

        UpdateSidesMeasures();

        RegisterBigSides();
        RegisterSmallSides();
        RegisterBottom();

        GenerateMesh();
    }

    private void UpdateSidesMeasures()
    {
        _numberOfRows = ClosestInteger(_height, _plank.Height);
        _finalHeight = _plank.Height * _numberOfRows;
        _finalDepth = _depth + 2 * _plank.Thickness;

        Debug.Log("Nombre de lignes (côtés) : " + _numberOfRows);
        Debug.Log("Hauteur : " + _finalHeight);
        Debug.Log("Profondeur : " + _finalDepth);
    }

    private void RegisterBigSides()
    {
        // Retirer l'épaisseur d'une planche car
        // 1 des angles de la jardiniere est 
        // rempli par la planche de l'autre coté
        var finalSize = _width - _plank.Thickness;
        _bigSidePlank = new Plank(_plank, finalSize);

        for (int i = 0; i < _numberOfRows * 2; i++)
        {
            _result.Add(_bigSidePlank);
        }
    }

    private void RegisterSmallSides()
    {
        // Retirer l'épaisseur d'une planche car
        // 1 des angles de la jardiniere est 
        // rempli par la planche de l'autre coté

        // Profondeur à l'intérieur + 2 fois l'épaisseur
        // d'une planche (2 cotés) - une fois l'épaisseur 
        // (voir au dessus)
        var finalSize = _finalDepth - _plank.Thickness;
        _smallSidePlank = new Plank(_plank, finalSize);

        for (int i = 0; i < _numberOfRows * 2; i++)
        {
            _result.Add(_smallSidePlank);
        }
    }

    private void RegisterBottom()
    {
        Debug.Log("Best size for bottom is " + _width + "x" + _finalDepth);

        _bottomPlank = new Plank(_finalDepth, _bottomThickness, _width);

        _result.Add(_bottomPlank);
    }

    private void GenerateMesh()
    {
        Mesh mesh = new Mesh();

        int vertexIndex = 0;
        int triangleIndex = 0;
        Vector3[] vertex = new Vector3[_result.Count * 8];
        int[] triangles = new int[_result.Count * 36];

        GenerateBigSidesMesh(ref vertexIndex, ref triangleIndex, ref vertex, ref triangles);
        GenerateSmallSidesMesh(ref vertexIndex, ref triangleIndex, ref vertex, ref triangles);

        mesh.vertices = vertex;
        mesh.triangles = triangles;

        //mesh.RecalculateNormals();

        MeshFilter.mesh = mesh;
    }

    private void GenerateBigSidesMesh(ref int vertexIndex, ref int triangleIndex, ref Vector3[] vertex, ref int[] triangles)
    {
        Vector3 size = new Vector3(_bigSidePlank.Length, _bigSidePlank.Height, _bigSidePlank.Thickness);
        Vector3 pos = Vector3.zero;

        for(int i = 0; i < _numberOfRows; i++)
        {
            pos.y = i * (_plank.Height + 0.05f);
            pos.x = (i % 2 > 0) ? _plank.Thickness : 0f;
            GenerateCubeMesh(pos, size, ref vertexIndex, ref triangleIndex, ref vertex, ref triangles);
        }

        pos.z = _finalDepth - _plank.Thickness;

        for (int i = 0; i < _numberOfRows; i++)
        {
            pos.y = i * (_plank.Height + 0.05f);
            pos.x = (i % 2 > 0) ? 0f : _plank.Thickness;
            GenerateCubeMesh(pos, size, ref vertexIndex, ref triangleIndex, ref vertex, ref triangles);
        }
    }

    private void GenerateSmallSidesMesh(ref int vertexIndex, ref int triangleIndex, ref Vector3[] vertex, ref int[] triangles)
    {
        Vector3 size = new Vector3(_smallSidePlank.Thickness, _smallSidePlank.Height, _smallSidePlank.Length);
        Vector3 pos = Vector3.zero;

        for (int i = 0; i < _numberOfRows; i++)
        {
            pos.y = i * (_plank.Height + 0.05f);
            pos.z = (i % 2 > 0) ? 0f : _plank.Thickness;
            GenerateCubeMesh(pos, size, ref vertexIndex, ref triangleIndex, ref vertex, ref triangles);
        }

        pos.x = _width - _plank.Thickness;

        for (int i = 0; i < _numberOfRows; i++)
        {
            pos.y = i * (_plank.Height + 0.05f);
            pos.z = (i % 2 > 0) ? _plank.Thickness : 0f;
            GenerateCubeMesh(pos, size, ref vertexIndex, ref triangleIndex, ref vertex, ref triangles);
        }
    }

    private int ClosestInteger(float desiredSize, float atomSize)
    {
        return Mathf.RoundToInt(desiredSize / atomSize);
    }

    private void GenerateCubeMesh(Vector3 pos, Vector3 size, ref int vertexIndex, ref int triangleIndex, ref Vector3[] vertex, ref int[] triangles)
    {
        vertex[vertexIndex + 0] = pos + new Vector3(0,0,0);
        vertex[vertexIndex + 1] = pos + new Vector3(size.x, 0, 0);
        vertex[vertexIndex + 2] = pos + new Vector3(size.x, 0, size.z);
        vertex[vertexIndex + 3] = pos + new Vector3(0, 0, size.z);
        vertex[vertexIndex + 4] = pos + new Vector3(0, size.y, 0);
        vertex[vertexIndex + 5] = pos + new Vector3(size.x, size.y, 0);
        vertex[vertexIndex + 6] = pos + new Vector3(size.x, size.y, size.z);
        vertex[vertexIndex + 7] = pos + new Vector3(0, size.y, size.z);

        // Bottom face
        triangles[triangleIndex + 0] = vertexIndex;
        triangles[triangleIndex + 1] = vertexIndex + 1;
        triangles[triangleIndex + 2] = vertexIndex + 2;
        triangles[triangleIndex + 3] = vertexIndex;
        triangles[triangleIndex + 4] = vertexIndex + 2;
        triangles[triangleIndex + 5] = vertexIndex + 3;

        // Top face
        triangles[triangleIndex + 6] = vertexIndex + 7;
        triangles[triangleIndex + 7] = vertexIndex + 6;
        triangles[triangleIndex + 8] = vertexIndex + 5;
        triangles[triangleIndex + 9] = vertexIndex + 7;
        triangles[triangleIndex + 10] = vertexIndex + 5;
        triangles[triangleIndex + 11] = vertexIndex + 4;

        // Left face
        triangles[triangleIndex + 12] = vertexIndex;
        triangles[triangleIndex + 13] = vertexIndex + 5;
        triangles[triangleIndex + 14] = vertexIndex + 1;
        triangles[triangleIndex + 15] = vertexIndex;
        triangles[triangleIndex + 16] = vertexIndex + 4;
        triangles[triangleIndex + 17] = vertexIndex + 5;

        // Right face
        triangles[triangleIndex + 18] = vertexIndex + 2;
        triangles[triangleIndex + 19] = vertexIndex + 7;
        triangles[triangleIndex + 20] = vertexIndex + 3;
        triangles[triangleIndex + 21] = vertexIndex + 2;
        triangles[triangleIndex + 22] = vertexIndex + 6;
        triangles[triangleIndex + 23] = vertexIndex + 7;

        // Front face
        triangles[triangleIndex + 24] = vertexIndex + 1;
        triangles[triangleIndex + 25] = vertexIndex + 6;
        triangles[triangleIndex + 26] = vertexIndex + 2;
        triangles[triangleIndex + 27] = vertexIndex + 1;
        triangles[triangleIndex + 28] = vertexIndex + 5;
        triangles[triangleIndex + 29] = vertexIndex + 6;

        // Rear face
        triangles[triangleIndex + 30] = vertexIndex + 3;
        triangles[triangleIndex + 31] = vertexIndex + 4;
        triangles[triangleIndex + 32] = vertexIndex;
        triangles[triangleIndex + 33] = vertexIndex + 3;
        triangles[triangleIndex + 34] = vertexIndex + 7;
        triangles[triangleIndex + 35] = vertexIndex + 4;

        vertexIndex += 8;
        triangleIndex += 36;
    }
}

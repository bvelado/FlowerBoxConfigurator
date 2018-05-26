using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class RawPlank : ScriptableObject {
    
    [Tooltip("Hauteur en mm")]
    [SerializeField] private float _height;
    public float Height { get { return _height; } }

    [Tooltip("Epaisseur en mm")]
    [SerializeField] private float _thickness;
    public float Thickness { get { return _thickness; } }
}

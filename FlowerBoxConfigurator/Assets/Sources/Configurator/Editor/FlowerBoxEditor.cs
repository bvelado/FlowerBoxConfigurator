using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(FlowerBox))]
public class FlowerBoxEditor : Editor {

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var flowerBox = target as FlowerBox;

        if (GUILayout.Button("Regenerate"))
        {
            flowerBox.Generate();
        }
    }

}

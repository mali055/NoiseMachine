using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(Perlin))]
public class PerlinNoiseInspector : Editor
{
    //private PerlinNoise myTarget;

    private void OnEnable()
    {
        //myTarget = target as PerlinNoise;
        Undo.undoRedoPerformed += recolor;
    }

    private void OnDisable()
    {
        Undo.undoRedoPerformed -= recolor;
    }

    private void recolor()
    {
        if (Application.isPlaying)
        {
            //myTarget.FillColor();
        }
    }

    public override void OnInspectorGUI()
    {
        EditorGUI.BeginChangeCheck();

        DrawDefaultInspector();

        if (EditorGUI.EndChangeCheck()) recolor();
    }
}
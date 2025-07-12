using UnityEditor;
using UnityEngine;
using DG.Tweening;
using Tweener = _Game.Scripts.Presentation.Scripts.Utility.Tweener;

[CustomEditor(typeof(Tweener))]
public class TweenerEditor : Editor
{
    private SerializedProperty useUnscaledTimeProp;
    private SerializedProperty startFromInitialActiveStateProp;
    private SerializedProperty simultaneousTweensProp;
    private SerializedProperty sequentialTweensProp;
    private bool[] foldoutsSimultaneous;
    private bool[] foldoutsSequential;

    private void OnEnable()
    {
        useUnscaledTimeProp = serializedObject.FindProperty("useUnscaledTime");
        startFromInitialActiveStateProp = serializedObject.FindProperty("startFromInitialActiveState");
        simultaneousTweensProp = serializedObject.FindProperty("simultaneousTweens");
        sequentialTweensProp = serializedObject.FindProperty("sequentialTweens");
        UpdateFoldoutsArray();
    }

    private void UpdateFoldoutsArray()
    {
        foldoutsSimultaneous = new bool[simultaneousTweensProp.arraySize];
        foldoutsSequential = new bool[sequentialTweensProp.arraySize];
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        DrawColoredBackground(Color.black, Color.white, () =>
        {
            EditorGUILayout.PropertyField(useUnscaledTimeProp, new GUIContent("Use Unscaled Time", "Use unscaled time for tweens"));
            EditorGUILayout.PropertyField(startFromInitialActiveStateProp, new GUIContent("Start From Initial Active State", "Start tween from the initial active state of the object"));
        });

        EditorGUILayout.Space();
        DrawColoredBackground(Color.black, Color.white, () => DrawTweensList("Simultaneous Tweens", simultaneousTweensProp, foldoutsSimultaneous));

        EditorGUILayout.Space();
        DrawColoredBackground(Color.black, Color.white, () => DrawTweensList("Sequential Tweens", sequentialTweensProp, foldoutsSequential));

        serializedObject.ApplyModifiedProperties();
    }

    private void DrawColoredBackground(Color backgroundColor, Color textColor, System.Action drawContent)
    {
        var rect = EditorGUILayout.BeginVertical();
        EditorGUI.DrawRect(rect, backgroundColor);
        GUI.color = textColor;
        drawContent();
        GUI.color = Color.white;
        EditorGUILayout.EndVertical();
    }

    private void DrawTweensList(string label, SerializedProperty tweensProp, bool[] foldouts)
    {
        EditorGUILayout.LabelField(label, EditorStyles.boldLabel);

        if (foldouts.Length != tweensProp.arraySize)
        {
            UpdateFoldoutsArray();
        }

        for (int i = 0; i < tweensProp.arraySize; i++)
        {
            SerializedProperty tween = tweensProp.GetArrayElementAtIndex(i);
            SerializedProperty tweenName = tween.FindPropertyRelative("name");
            SerializedProperty tweenType = tween.FindPropertyRelative("tweenType");

            foldouts[i] = EditorGUILayout.Foldout(foldouts[i], $"{label} {i + 1}: {tweenName.stringValue}", true);

            if (foldouts[i])
            {
                EditorGUILayout.PropertyField(tween.FindPropertyRelative("target"));
                EditorGUILayout.PropertyField(tweenName, new GUIContent("Name"));
                EditorGUILayout.PropertyField(tweenType);

                switch ((Tweener.TweenSettings.TweenType)tweenType.enumValueIndex)
                {
                    case Tweener.TweenSettings.TweenType.Move:
                    case Tweener.TweenSettings.TweenType.Scale:
                    case Tweener.TweenSettings.TweenType.Rotate:
                        EditorGUILayout.PropertyField(tween.FindPropertyRelative("targetValue"));
                        break;
                    case Tweener.TweenSettings.TweenType.Fade:
                        EditorGUILayout.PropertyField(tween.FindPropertyRelative("targetAlpha"));
                        break;
                    case Tweener.TweenSettings.TweenType.Color:
                        EditorGUILayout.PropertyField(tween.FindPropertyRelative("targetColor"));
                        break;
                }

                EditorGUILayout.PropertyField(tween.FindPropertyRelative("duration"));
                EditorGUILayout.PropertyField(tween.FindPropertyRelative("delay"));

                SerializedProperty useCustomCurve = tween.FindPropertyRelative("useCustomCurve");
                EditorGUILayout.PropertyField(useCustomCurve);

                if (useCustomCurve.boolValue)
                {
                    EditorGUILayout.PropertyField(tween.FindPropertyRelative("customCurve"));
                }
                else
                {
                    EditorGUILayout.PropertyField(tween.FindPropertyRelative("easeType"));
                }

                SerializedProperty loop = tween.FindPropertyRelative("loop");
                EditorGUILayout.PropertyField(loop);

                if (loop.boolValue)
                {
                    EditorGUILayout.PropertyField(tween.FindPropertyRelative("loopCount"));
                    EditorGUILayout.PropertyField(tween.FindPropertyRelative("pingpong"));
                }

                EditorGUILayout.PropertyField(tween.FindPropertyRelative("OnTweenComplete"));
            }

            EditorGUILayout.BeginHorizontal();
            GUI.backgroundColor = Color.cyan;
            if (GUILayout.Button("Move Up") && i > 0)
            {
                tweensProp.MoveArrayElement(i, i - 1);
            }
            GUI.backgroundColor = Color.magenta;
            if (GUILayout.Button("Move Down") && i < tweensProp.arraySize - 1)
            {
                tweensProp.MoveArrayElement(i, i + 1);
            }
            GUI.backgroundColor = Color.red;
            if (GUILayout.Button("Remove"))
            {
                tweensProp.DeleteArrayElementAtIndex(i);
                serializedObject.ApplyModifiedProperties();
                UpdateFoldoutsArray();
                return;
            }
            GUI.backgroundColor = Color.white;
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();
        }

        GUI.backgroundColor = Color.green;
        if (GUILayout.Button($"Add {label} Animation"))
        {
            tweensProp.InsertArrayElementAtIndex(tweensProp.arraySize);
            serializedObject.ApplyModifiedProperties();
            UpdateFoldoutsArray();
        }
        GUI.backgroundColor = Color.white;
    }
}
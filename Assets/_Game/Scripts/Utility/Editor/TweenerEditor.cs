using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Tweener))]
public class TweenerEditor : Editor
{
    private SerializedProperty tweenAnimations;
    private bool[] foldouts;

    private void OnEnable()
    {
        tweenAnimations = serializedObject.FindProperty("tweenAnimations");
        UpdateFoldoutsArray();
    }

    private void UpdateFoldoutsArray()
    {
        foldouts = new bool[tweenAnimations?.arraySize ?? 0];
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        Tweener tweener = (Tweener)target;

        EditorGUILayout.PropertyField(serializedObject.FindProperty("useUnscaledTime"));

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Tween Animations", EditorStyles.boldLabel);

        if (foldouts.Length != (tweenAnimations?.arraySize ?? 0))
        {
            UpdateFoldoutsArray();
        }

        if (tweenAnimations != null)
        {
            for (int i = 0; i < tweenAnimations.arraySize; i++)
            {
                SerializedProperty tween = tweenAnimations.GetArrayElementAtIndex(i);
                SerializedProperty tweenType = tween.FindPropertyRelative("tweenType");

                foldouts[i] = EditorGUILayout.Foldout(foldouts[i], $"Animation {i + 1}", true);
                if (foldouts[i])
                {
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

                    EditorGUILayout.PropertyField(tween.FindPropertyRelative("isUIElement"));
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

                    EditorGUILayout.PropertyField(tween.FindPropertyRelative("startOnActive"));
                    EditorGUILayout.PropertyField(tween.FindPropertyRelative("OnTweenComplete"));

                    if (GUILayout.Button("Remove Tween Animation"))
                    {
                        tweenAnimations.DeleteArrayElementAtIndex(i);
                        serializedObject.ApplyModifiedProperties();
                        UpdateFoldoutsArray();
                        return; // Exit the loop to avoid issues with the modified array
                    }

                    EditorGUILayout.Space();
                }
            }
        }

        if (GUILayout.Button("Add Tween Animation"))
        {
            tweener.tweenAnimations.Add(new Tweener.TweenSettings());
            serializedObject.ApplyModifiedProperties();
            UpdateFoldoutsArray();
        }

        serializedObject.ApplyModifiedProperties();
    }
}
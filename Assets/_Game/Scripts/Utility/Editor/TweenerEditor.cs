using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Tweener))]
public class TweenerEditor : Editor
{
    SerializedProperty tweenAnimations;
    private bool[] foldouts;

    private void OnEnable()
    {
        tweenAnimations = serializedObject.FindProperty("tweenAnimations");
        foldouts = new bool[tweenAnimations.arraySize];
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        Tweener tweener = (Tweener)target;

        EditorGUILayout.PropertyField(serializedObject.FindProperty("debugMode"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("runOnStart"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("useUnscaledTime"));

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Tween Animations", EditorStyles.boldLabel);

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
                        EditorGUILayout.PropertyField(tween.FindPropertyRelative("isUIElement"));
                        break;
                }

                EditorGUILayout.PropertyField(tween.FindPropertyRelative("targetObject"));
                EditorGUILayout.PropertyField(tween.FindPropertyRelative("duration"));
                EditorGUILayout.PropertyField(tween.FindPropertyRelative("delay"));
                EditorGUILayout.PropertyField(tween.FindPropertyRelative("easeType"));
                EditorGUILayout.PropertyField(tween.FindPropertyRelative("loop"));
                EditorGUILayout.PropertyField(tween.FindPropertyRelative("pingpong"));
                EditorGUILayout.PropertyField(tween.FindPropertyRelative("loopCount"));
                EditorGUILayout.PropertyField(tween.FindPropertyRelative("onComplete"));

                EditorGUILayout.Space();
            }
        }

        if (GUILayout.Button("Add Tween Animation"))
        {
            tweener.tweenAnimations.Add(new Tweener.TweenSettings());
            foldouts = new bool[tweenAnimations.arraySize];
        }

        serializedObject.ApplyModifiedProperties();
    }
}
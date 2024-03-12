using UnityEngine;
using UnityEditor;

using GogoGaga.TME;

[CanEditMultipleObjects]
[CustomEditor(typeof(LeantweenCustomAnimator))]
public class LeantweenCustomEditor : Editor
{
    Vector2 scroll;
    string text = "Notes";

    Texture2D logo;
    public override void OnInspectorGUI()
    {

        var component = (LeantweenCustomAnimator)target;
        if (component == null) return;

        serializedObject.Update();

        
        

        if (logo == null)
        {
            logo = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/GogoGaga/TweenMadeEasy/Sprites/logo.png", typeof(Texture2D));
        }
        else
        {
            //GUI.DrawTexture(new Rect(100, 0, 200,  100), logo,ScaleMode.ScaleToFit);
            GUIContent content = new GUIContent(logo,"Click here!");

            GUIStyle gUIStyle = new GUIStyle(GUI.skin.box);

            //gUIStyle.hover.background = logo;

            if(GUI.Button(new Rect(50, 0, 200, 100), content,gUIStyle))
            {
                Application.OpenURL("https://assetstore.unity.com/publishers/44583");
            }
        }


        EditorGUILayout.Space(100);


        scroll = EditorGUILayout.BeginScrollView(scroll);
        text = EditorGUILayout.TextArea(text, GUILayout.Height(50));
        EditorGUILayout.EndScrollView();

        EditorGUILayout.Space(20);


        EditorGUILayout.PropertyField(serializedObject.FindProperty("Name"));
        
        EditorGUILayout.Space(5);

        EditorGUILayout.PropertyField(serializedObject.FindProperty("AnimationType"));

        if (component.AnimationType == LeantweenCustomAnimator.MOTION_TYPE._fadeCanvasGroup)
            if (component.GetComponent<CanvasGroup>() == null)
            {
                EditorGUILayout.LabelField("There is no canvas group component attached to this object");
                serializedObject.ApplyModifiedProperties();
                return;
            }
        EditorGUILayout.Space(5);

        EditorGUILayout.PropertyField(serializedObject.FindProperty("PlayAutomatic"));




        EditorGUILayout.Space(20);

        EditorGUILayout.LabelField("Start Animation Properties :");


        EditorGUILayout.PropertyField(serializedObject.FindProperty("OverrideStartValues"));

        if (component.OverrideStartValues)
        {
            if (component.AnimationType == LeantweenCustomAnimator.MOTION_TYPE._scale
             || component.AnimationType == LeantweenCustomAnimator.MOTION_TYPE._move
             || component.AnimationType == LeantweenCustomAnimator.MOTION_TYPE._rotate
             || component.AnimationType == LeantweenCustomAnimator.MOTION_TYPE._spin)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("StartTargetType"));

                if (component.StartTargetType == LeantweenCustomAnimator.TARGET_TYPE.vector3)
                {
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("start_vector"));
                    component.StartTargetTranform = null;
                }
                else
                {
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("StartTargetTranform"));
                    component.start_vector = Vector3.zero;
                }
            }
            else 
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("startValue"));
            }
        }



        EditorGUILayout.Space(20);
        EditorGUILayout.LabelField("End Animation Properties :");



        if (component.AnimationType == LeantweenCustomAnimator.MOTION_TYPE._scale
            || component.AnimationType == LeantweenCustomAnimator.MOTION_TYPE._move
            || component.AnimationType == LeantweenCustomAnimator.MOTION_TYPE._rotate)
        {

            EditorGUILayout.PropertyField(serializedObject.FindProperty("DontOverrideEndProperty"));
            if (!component.DontOverrideEndProperty)
            {
                
                EditorGUILayout.PropertyField(serializedObject.FindProperty("EndTargetType"));

                if (component.EndTargetType == LeantweenCustomAnimator.TARGET_TYPE.vector3)
                {
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("end_vector"));
                    component.EndTargetTranform = null;
                }
                else
                {
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("EndTargetTranform"));
                    component.end_vector = Vector3.zero;
                }
            }
       
        }
        else if (component.AnimationType == LeantweenCustomAnimator.MOTION_TYPE._spin)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("RotationAxis"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("RotationPerSec"));
        }
        else if (component.AnimationType == LeantweenCustomAnimator.MOTION_TYPE._fade 
            || component.AnimationType == LeantweenCustomAnimator.MOTION_TYPE._fadeCanvasGroup)
        {

            if (component.AnimationType == LeantweenCustomAnimator.MOTION_TYPE._fadeCanvasGroup)
                EditorGUILayout.PropertyField(serializedObject.FindProperty("DontOverrideEndProperty"));

            if (!component.DontOverrideEndProperty)
                EditorGUILayout.PropertyField(serializedObject.FindProperty("endValue"));
        }



            EditorGUILayout.Space(20);
        EditorGUILayout.LabelField("Animation Time Properties :");


        EditorGUILayout.PropertyField(serializedObject.FindProperty("_delay"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("_time"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("_easeType"));




        EditorGUILayout.Space(20);
        EditorGUILayout.LabelField("Advance Properties :");

        EditorGUILayout.PropertyField(serializedObject.FindProperty("StopOtherTweensOnThis"));
        
        EditorGUILayout.PropertyField(serializedObject.FindProperty("PlayInDeltaTime"));
        
        EditorGUILayout.PropertyField(serializedObject.FindProperty("DestroyOnCompletion"));


        EditorGUILayout.Space(20);
        EditorGUILayout.LabelField("Events :");

        EditorGUILayout.PropertyField(serializedObject.FindProperty("OnStart"));

        if (component.AnimationType == LeantweenCustomAnimator.MOTION_TYPE._scale
                     || component.AnimationType == LeantweenCustomAnimator.MOTION_TYPE._move
                     || component.AnimationType == LeantweenCustomAnimator.MOTION_TYPE._rotate
                     || component.AnimationType == LeantweenCustomAnimator.MOTION_TYPE._spin)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("OnUpdate_Vector"));
            component.OnUpdate_float = new LeantweenCustomAnimator.UpdateEventfloat();
        }
        else
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("OnUpdate_float"));
            component.OnUpdate_Vector = new LeantweenCustomAnimator.UpdateEventVector();
        }
        EditorGUILayout.PropertyField(serializedObject.FindProperty("OnCompletion"));



        serializedObject.ApplyModifiedProperties();
    }
}

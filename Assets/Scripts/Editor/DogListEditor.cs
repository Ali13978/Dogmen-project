using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class DogStatsRandomizerWindow : EditorWindow
{
    private DogList dogList;

    public static void ShowWindow()
    {
        GetWindow<DogStatsRandomizerWindow>("Dogs Stats Randomizer");
    }

    private void OnGUI()
    {
        dogList = EditorGUILayout.ObjectField("Dog List", dogList, typeof(DogList), false) as DogList;

        if (GUILayout.Button("Randomize Stats") && dogList != null)
        {
            foreach (var dogSO in dogList.dogs)
            {
                dogSO.willPower = Random.Range(0.7f, 0.9f);
                dogSO.attackDamagePower = Random.Range(0.7f, 0.9f);
                dogSO.moveSpeedPower = Random.Range(0.7f, 0.9f);
                dogSO.stamina = Random.Range(0.7f, 0.9f);
            }

            EditorUtility.SetDirty(dogList);
        }
    }
}

[CustomEditor(typeof(DogList))]
public class DogListEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (GUILayout.Button("Dogs Stats Randomizer"))
        {
            DogStatsRandomizerWindow.ShowWindow();
        }
    }
}


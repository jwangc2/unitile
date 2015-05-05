using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class InputEditor : EditorWindow {

    private Dictionary<string, int> countMap;
    private List<string> keys;

    // Add menu named "My Window" to the Window menu
    [MenuItem ("Window/InputMod")]
    public static void ShowWindow()
    {
        //Show existing window instance. If one doesn't exist, make one.
        EditorWindow.GetWindow(typeof(InputEditor));
    }

    void OnEnable()
    {
        countMap = new Dictionary<string, int>();
        countMap.Add("Gamepad Count", 1);
        countMap.Add("Joystick Count", 1);
        countMap.Add("Button Count", 1);
        countMap.Add("Starting Index", 16);

        keys = new List<string>();
        foreach (KeyValuePair<string, int> kvp in countMap)
        {
            keys.Add(kvp.Key);
        }

        foreach (string thisKey in keys)
        {
            if (EditorPrefs.HasKey(thisKey))
            {
                countMap[thisKey] = EditorPrefs.GetInt(thisKey);
            }
        }
    }

    void OnGUI()
    {
        foreach (string thisKey in keys)
        {
            countMap[thisKey] =  EditorGUILayout.IntField(thisKey, countMap[thisKey]);
            countMap[thisKey] = Mathf.Max(1, countMap[thisKey]);
        }

        bool toUpdate = GUILayout.Button("Update");
        if (toUpdate)
        {
            UpdateValues();
        }
    }

    void UpdateValues()
    {
        // Rewrite the input manager
        int index = countMap["Starting Index"] - 1;
        for (int g = 1; g <= countMap["Gamepad Count"]; g ++)
        {
            for (int j = 1; j <= countMap["Joystick Count"]; j ++)
            {
                InputAxis a = InputDefina.DefaultJoystick(g, j);
                InputDefina.AddAxis(a, index);
                index ++;
            }
            for (int b = 0; b < countMap["Button Count"]; b ++)
            {
                InputAxis a = InputDefina.DefaultButton(g, b);
                InputDefina.AddAxis(a, index);
                index ++;
            }
        }

        // Save these values to the memory
        foreach (KeyValuePair<string, int> kvp in countMap)
        {
            EditorPrefs.SetInt(kvp.Key, kvp.Value);
        }
    }
}

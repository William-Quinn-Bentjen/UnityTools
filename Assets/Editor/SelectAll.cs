using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

class SelectAll : EditorWindow
{
    static GameObject gameObject;
    static bool addToSelection = true;
    //name
    static bool careAboutName;
    enum NameSetting
    {
        ExactMatch,
        Contains
    }
    static NameSetting nameSetting;
    static string nameContains;
    static bool careAboutTag;
    static bool careAboutLayer;
    enum Vector3Setting
    {
        Exatly,
        Within,
        Between
    }
    //exact match, within values, between values
    static bool careAboutPosition;
    static Vector3Setting positionSetting;
    static Vector3 positionWithin;
    static Vector3[] positionBetween = new Vector3[2] {Vector3.zero, Vector3.zero};
    static bool careAboutRotation;
    static bool careAboutScale;
    static bool careAboutActive;
    static bool careAboutComponents;
    static bool gameObjectIsNull = true;
    [MenuItem("Window/Tools/Select All")]
    
    public static void ShowWindow()
    {
        GetWindow(typeof(SelectAll), false, "Select all with...", true);
    }

    void OnGUI()
    {
        gameObject = (GameObject)EditorGUILayout.ObjectField(gameObject, typeof(GameObject), true);
        gameObjectIsNull = (gameObject == null);
        if (gameObjectIsNull)
        {
            GUILayout.Label("Requires a game object to search", EditorStyles.boldLabel);
        }
        addToSelection = EditorGUILayout.Toggle("Add To Current Selection", addToSelection);
        GUILayout.Label("Care about", EditorStyles.boldLabel);
        if (gameObjectIsNull)
        {
            //comment for debugging 
            //GUI.enabled = false;
        }
        ToggleLeft("Name", ref careAboutName);
        if (careAboutName)
        {
            nameSetting = (NameSetting)EditorGUILayout.EnumPopup(nameSetting);
            if (nameSetting == NameSetting.Contains)
            {
                nameContains = EditorGUILayout.TextField(nameContains);
            }
        }
        ToggleLeft("Tag", ref careAboutTag);
        ToggleLeft("Layer", ref careAboutLayer);
        ToggleLeft("Position", ref careAboutPosition);
        if (careAboutPosition)
        {
            positionSetting = (Vector3Setting)EditorGUILayout.EnumPopup(positionSetting);
            if (positionSetting == Vector3Setting.Within)
            {
                positionWithin = EditorGUILayout.Vector3Field("+ or - of GameObject on each axis", positionWithin);
            }
            else if (positionSetting == Vector3Setting.Between)
            {
                positionBetween[0] = EditorGUILayout.Vector3Field("Starting Value", positionBetween[0]);
                positionBetween[1] = EditorGUILayout.Vector3Field("Ending Value", positionBetween[1]);
            }
        }
        ToggleLeft("Rotation", ref careAboutRotation);
        ToggleLeft("Scale", ref careAboutScale);
        ToggleLeft("Active", ref careAboutActive);
        ToggleLeft("Components", ref careAboutComponents);
        if (GUILayout.Button("Search"))
        {
            
            foreach(GameObject obj in GetAllGameObjects())
            {
                if (obj.activeInHierarchy)
                {
                    Debug.Log(obj.name);
                }
                else
                {
                    Debug.Log("INACTIVE " + obj.name);
                }
            }
            //UnityEditor.Selection.gameObjects
        }
    }
    /// <summary>
    /// Get all the GameObjects in the active scene 
    /// NOTE: includes inactive objects
    /// </summary>
    /// <returns>All GameObjects in the active scene</returns>
    private List<GameObject> GetAllGameObjects()
    {
        List<GameObject> allGameObjects = new List<GameObject>();
        foreach (GameObject obj in GetRootObjects())
        {
            //add root object
            allGameObjects.Add(obj);
            //add all children of that object
            ExploreRootObject(obj.transform, ref allGameObjects);
        }
        return allGameObjects;
    }
    /// <summary>
    /// Adds all children of a root GameObject to the list of all children
    /// </summary>
    /// <param name="rootTransform">The Transform of the GameObject at the root of the scene (is a child of no one)</param>
    /// <param name="allChildren">A reference to the list of all children in the scene</param>
    private void ExploreRootObject(Transform rootTransform, ref List<GameObject> allChildren)
    {
        List<Transform> unexplored = new List<Transform>();
        unexplored.Add(rootTransform);
        while(unexplored.Count > 0)
        { 
            unexplored.AddRange(Explore(unexplored[0], ref allChildren));
            unexplored.RemoveAt(0);
        }
    }
    /// <summary>
    /// Adds children of parent object to the all children list and returns the children of each child
    /// </summary>
    /// <param name="parent">The parent that will have it's children explored</param>
    /// <param name="allChildren">A reference to the list of all children in the scene</param>
    /// <returns></returns>
    private List<Transform> Explore(Transform parent, ref List<GameObject> allChildren)
    {
        List<Transform> children = new List<Transform>();
        foreach(Transform child in parent)
        {
            children.Add(child);
            allChildren.Add(child.gameObject);
        }
        return children;
    }
    /// <summary>
    /// Gets the root objects in the active scene
    /// </summary>
    /// <returns>A list of all objects in the active scene that are a child of nothing</returns>
    private List<GameObject> GetRootObjects()
    {
        List <GameObject> rootObjects = new List<GameObject>();
        Scene scene = SceneManager.GetActiveScene();
        scene.GetRootGameObjects(rootObjects);
        return rootObjects;
    }
    /// <summary>
    /// used to make a simple toggle with the toggle on the right
    /// </summary>
    /// <param name="text">label</param>
    /// <param name="boolToChange">A reference to the state of the toggle</param>
    private void Toggle(string text, ref bool boolToChange)
    {
        boolToChange = EditorGUILayout.Toggle(text, boolToChange);
    }
    /// <summary>
    /// used to make a simple toggle with the toggle on the left
    /// </summary>
    /// <param name="text">label</param>
    /// <param name="boolToChange">A reference to the state of the toggle</param>
    private void ToggleLeft(string text, ref bool boolToChange)
    {
        boolToChange = EditorGUILayout.ToggleLeft(text, boolToChange);
    }
    
}
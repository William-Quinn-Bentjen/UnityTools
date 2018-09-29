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
    //exact match, within values, between values
    enum Vector3Setting
    {
        Exatly,
        Within,
        Between
    }
    //position
    static bool careAboutPosition;
    static Vector3Setting positionSetting;
    static Vector3 positionWithin;
    static Vector3[] positionBetween = new Vector3[2] {Vector3.zero, Vector3.zero};
    //rotation
    static bool careAboutRotation;
    static Vector3Setting rotationSetting;
    static Vector3 rotationWithin;
    static Vector3[] rotationBetween = new Vector3[2] { Vector3.zero, Vector3.zero };
    //scale
    static bool careAboutScale;
    static Vector3Setting scaleSetting;
    static Vector3 scaleWithin;
    static Vector3[] scaleBetween = new Vector3[2] { Vector3.zero, Vector3.zero };
    //active
    static bool careAboutActive;
    enum ActiveSetting
    {
        InHierarchy,
        Self
    }
    enum BoolSetting
    {
        Match,
        True,
        False
    }
    static ActiveSetting activeSetting;
    static BoolSetting activeBoolSetting;
    static bool careAboutComponents;
    enum ComponentSetting
    {
        Has,
        DoesNotHave,
    }
    static ComponentSetting componentSetting;
    bool careAboutComponentValues;
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
                Vector3Field("+ or - of GameObject on each axis", ref positionWithin);
            }
            else if (positionSetting == Vector3Setting.Between)
            {
                Vector3Field("Starting Value", ref positionBetween[0]);
                Vector3Field("Ending Value", ref positionBetween[1]);
            }
        }
        ToggleLeft("Rotation", ref careAboutRotation);
        if (careAboutRotation)
        {
            rotationSetting = (Vector3Setting)EditorGUILayout.EnumPopup(rotationSetting);
            if (rotationSetting == Vector3Setting.Within)
            {
                Vector3Field("+ or - of GameObject on each axis", ref rotationWithin);
            }
            else if (rotationSetting == Vector3Setting.Between)
            {
                Vector3Field("Starting Value", ref rotationBetween[0]);
                Vector3Field("Ending Value", ref rotationBetween[1]);
            }
        }
        ToggleLeft("Scale", ref careAboutScale);
        if (careAboutScale)
        {
            scaleSetting = (Vector3Setting)EditorGUILayout.EnumPopup(scaleSetting);
            if (scaleSetting == Vector3Setting.Within)
            {
                Vector3Field("+ or - of GameObject on each axis", ref scaleWithin);
            }
            else if (scaleSetting == Vector3Setting.Between)
            {
                Vector3Field("Starting Value", ref scaleBetween[0]);
                Vector3Field("Ending Value", ref scaleBetween[1]);
            }
        }
        ToggleLeft("Active", ref careAboutActive);
        if (careAboutActive)
        {
            activeSetting = (ActiveSetting)EditorGUILayout.EnumPopup(activeSetting);
            activeBoolSetting = (BoolSetting)EditorGUILayout.EnumPopup(activeBoolSetting);
        }
        ToggleLeft("Components", ref careAboutComponents);
        if (careAboutComponents)
        {
            componentSetting = (ComponentSetting)EditorGUILayout.EnumPopup(componentSetting);
            ToggleLeft("Care about values", ref careAboutComponentValues);
        }
        if (GUILayout.Button("Search"))
        {
            
            Select(Filter(GetAllGameObjects()));
            //UnityEditor.Selection.gameObjects
        }
    }
    /// <summary>
    /// deals with
    /// </summary>
    /// <param name="allFilteredGameObjects"></param>
    private void Select(List<GameObject> allFilteredGameObjects)
    {
        if (addToSelection)
        {
            foreach (GameObject obj in Selection.objects)
            {
                allFilteredGameObjects.Add(obj);
            }
        }
        Selection.objects = allFilteredGameObjects.ToArray();
    }
    /// <summary>
    /// Main filter 
    /// </summary>
    /// <param name="allGameObjects"></param>
    private List<GameObject> Filter(List<GameObject> allGameObjects)
    {
        if (careAboutName) { FilterName(allGameObjects); }
        if (careAboutTag) { FilterTag(allGameObjects); }
        if (careAboutLayer) { FilterLayer(allGameObjects); }
        if (careAboutPosition) { FilterPosition(allGameObjects); }
        if (careAboutRotation) { FilterRotation(allGameObjects); }
        if (careAboutScale) { FilterScale(allGameObjects); }
        if (careAboutActive) { FilterActive(allGameObjects); }
        if (careAboutComponents) { FilterComponents(allGameObjects); }
        return allGameObjects;
    }
    private void FilterName(List<GameObject> allGameObjects)
    {
        switch (nameSetting)
        {
            case NameSetting.ExactMatch:
                string gameObjectName = gameObject.name;
                for (int i = 0; i < allGameObjects.Count;)
                {
                    if (allGameObjects[i].name == gameObjectName)
                    {
                        i++;
                    }
                    else
                    {
                        allGameObjects.RemoveAt(i);
                    }
                }
                break;
            case NameSetting.Contains:
                for (int i = 0; i < allGameObjects.Count;)
                {
                    if (allGameObjects[i].name.Contains(nameContains))
                    {
                        i++;
                    }
                    else
                    {
                        allGameObjects.RemoveAt(i);
                    }
                }
                break;
        }
    }
    private void FilterTag(List<GameObject> allGameObjects)
    {
        string gameObjectTag = gameObject.tag;
        for (int i = 0; i < allGameObjects.Count;)
        {
            if (allGameObjects[i].tag == gameObjectTag)
            {
                i++;
            }
            else
            {
                allGameObjects.RemoveAt(i);
            }
        }
    }
    private void FilterLayer(List<GameObject> allGameObjects)
    {
        int gameObjectLayer = gameObject.layer;
        for (int i = 0; i < allGameObjects.Count;)
        {
            if (allGameObjects[i].layer == gameObjectLayer)
            {
                i++;
            }
            else
            {
                allGameObjects.RemoveAt(i);
            }
        }
    }
    private void FilterPosition(List<GameObject> allGameObjects)
    {
        Vector3 gameObjectPosition = gameObject.transform.position;
        switch (positionSetting)
        {
            case Vector3Setting.Exatly:
                for (int i = 0; i < allGameObjects.Count;)
                {
                    if (allGameObjects[i].transform.position == gameObjectPosition)
                    {
                        i++;
                    }
                    else
                    {
                        allGameObjects.RemoveAt(i);
                    }
                }
                break;
            case Vector3Setting.Between:
                //greater/less than? between values
                Vector3Int greatestInt = new Vector3Int();
                Vector3 greatest, least = new Vector3();
                greatestInt.x = positionBetween[0].x > positionBetween[1].x ? 0 : 1;
                greatestInt.y = positionBetween[0].y > positionBetween[1].y ? 0 : 1;
                greatestInt.z = positionBetween[0].z > positionBetween[1].z ? 0 : 1;
                //set least
                least.x = greatestInt.x == 1 ? positionBetween[0].x : positionBetween[1].x;
                least.y = greatestInt.x == 1 ? positionBetween[0].y : positionBetween[1].y;
                least.z = greatestInt.x == 1 ? positionBetween[0].z : positionBetween[1].z;
                //set greatest
                greatest.x = positionBetween[greatestInt.x].x;
                greatest.y = positionBetween[greatestInt.y].y;
                greatest.z = positionBetween[greatestInt.z].z;
                for (int i = 0; i < allGameObjects.Count;)
                {
                    Vector3 position = allGameObjects[i].transform.position;
                    if ((position.x <= greatest.x && position.x >= least.x) &&
                        (position.y <= greatest.y && position.y >= least.y) &&
                        (position.z <= greatest.z && position.z >= least.z))
                    {
                        i++;
                    }
                    else
                    {
                        allGameObjects.RemoveAt(i);
                    }
                }
                break;
            case Vector3Setting.Within:
                break;
        }
    }
    private void FilterRotation(List<GameObject> allGameObjects)
    {

    }
    private void FilterScale(List<GameObject> allGameObjects)
    {

    }
    private void FilterActive(List<GameObject> allGameObjects)
    {
        switch (activeSetting)
        {
            case ActiveSetting.Self:
                switch (activeBoolSetting)
                {
                        case BoolSetting.True:
                        for (int i = 0; i < allGameObjects.Count;)
                        {
                            if (allGameObjects[i].activeSelf)
                            {
                                i++;
                            }
                            else
                            {
                                allGameObjects.RemoveAt(i);
                            }
                        }
                        break;
                    case BoolSetting.False:
                        for (int i = 0; i < allGameObjects.Count;)
                        {
                            if (allGameObjects[i].activeSelf)
                            {

                                allGameObjects.RemoveAt(i);
                            }
                            else
                            {
                                i++;
                            }
                        }
                        break;
                    case BoolSetting.Match:
                        bool gameObjectActive = gameObject.activeSelf;
                        for (int i = 0; i < allGameObjects.Count;)
                        {
                            if (allGameObjects[i].activeSelf == gameObjectActive)
                            {
                                i++;
                            }
                            else
                            {
                                allGameObjects.RemoveAt(i);
                            }
                        }
                        break;
                }
                break;
            case ActiveSetting.InHierarchy:
                switch (activeBoolSetting)
                {
                    case BoolSetting.True:
                        for (int i = 0; i < allGameObjects.Count;)
                        {
                            if (allGameObjects[i].activeInHierarchy)
                            {
                                i++;
                            }
                            else
                            {
                                allGameObjects.RemoveAt(i);
                            }
                        }
                        break;
                    case BoolSetting.False:
                        for (int i = 0; i < allGameObjects.Count;)
                        {
                            if (allGameObjects[i].activeInHierarchy)
                            {

                                allGameObjects.RemoveAt(i);
                            }
                            else
                            {
                                i++;
                            }
                        }
                        break;
                    case BoolSetting.Match:
                        bool gameObjectActive = gameObject.activeInHierarchy;
                        for (int i = 0; i < allGameObjects.Count;)
                        {
                            if (allGameObjects[i].activeInHierarchy == gameObjectActive)
                            {
                                i++;
                            }
                            else
                            {
                                allGameObjects.RemoveAt(i);
                            }
                        }
                        break;
                }
                break;
        }
    }
    private void FilterComponents(List<GameObject> allGameObjects)
    {

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
    private void Vector3Field(string text, ref Vector3 vector)
    {
        vector = EditorGUILayout.Vector3Field(text, vector);
    }
}
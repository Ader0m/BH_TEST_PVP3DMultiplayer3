using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    #region Singltone
    public static Game Instance
    {
        get
        {
            return _instance;
        }
    }

    private static Game _instance;
    #endregion

    public Dictionary<int, Sprite> ResourceSpriteDict;

    void Start()
    {
        _instance = this;
        ResourceSpriteDict = new Dictionary<int, Sprite>();


        LoadResources();
        Cursor.lockState = CursorLockMode.Confined;
    }

    private void LoadResources()
    {
        Sprite[] mass;


        mass = Resources.LoadAll<Sprite>("Image");
        for (int i = 0; i < mass.Length; i++)
        {
            ResourceSpriteDict.Add(i, mass[i]);
        }
        Debug.Log("Load Resources finish");
    }
}

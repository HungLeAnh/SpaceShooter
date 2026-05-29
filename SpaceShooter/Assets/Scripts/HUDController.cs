using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum HUDStateType
{
    Stats,
    Menu,
    GameOver,
    Wave,
    Victory
}

[Serializable]
public class HUDElement
{
    public HUDStateType StateType;
    public GameObject Element;
}

public class HUDController : MonoBehaviour
{
    public static HUDController Instance;

    [SerializeField] private List<HUDElement> HUDElements;

    private Dictionary<HUDStateType, GameObject> HUDelementDictionary;
    private void Awake()
    {
        if(Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        HUDelementDictionary = new Dictionary<HUDStateType, GameObject>();
        foreach (var element in HUDElements)
        {
            HUDelementDictionary[element.StateType] = element.Element;
        }
    }
    private void Start()
    {
    }

    public void ShowElement(HUDStateType stateType)
    {
        if (HUDelementDictionary.TryGetValue(stateType, out GameObject element))
        {
            element.SetActive(true);
        }
    }
    public void HideElement(HUDStateType stateType)
    {
        if(HUDelementDictionary.TryGetValue(stateType, out GameObject element))
        {
            element.SetActive(false);
        }
    }

}

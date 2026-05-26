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
    GameOver
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

    private void Awake()
    {
        if(Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
    private void Start()
    {
    }

    public void ShowElement(HUDStateType stateType)
    {
        foreach (var element in HUDElements)
        {
            if (element.StateType == stateType)
            {
                element.Element.SetActive(true);
            }
        }
    }
    public void HideElement(HUDStateType stateType)
    {
        foreach (var element in HUDElements)
        {
            if (element.StateType == stateType)
            {
                element.Element.SetActive(false);
            }
        }
    }

}

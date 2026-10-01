using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    [SerializeField] List<GameObject> panels;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        foreach (GameObject panel in panels)
        {
            if (panel.name == "MainMenuPanel") { panel.SetActive(true); }
            else { panel.SetActive(false); }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void QuitGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        Debug.Log("Quit Game Pressed");
#endif
    }
}

using UnityEngine;
using TMPro;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using JetBrains.Annotations;

public class PuzzleOne : MonoBehaviour
{
    [SerializeField] private TextMeshPro ansText;
    [SerializeField] private TMP_Text ranShapetext;
    [SerializeField] private Canvas playerMKCanvas;
    [SerializeField] private Canvas playerCCanvas;
    [SerializeField] private CharacterController playerMK;
    [SerializeField] private CharacterController playerC;

    [SerializeField] private List<string> passShapeList = new List<string>() {"Cube", "Diamond", "Triangle", "Square"};
    void Start()
    {
        string shapeAns = GetRandomShape(passShapeList);
        ranShapetext.text = shapeAns;
    }

    void Update()
    {
        
    }

// The Function Gets a random index from the list given and returns that string to be used as our password shape
    public string GetRandomShape(List<string> passwordList)
    {
        int randomIndex = Random.Range(0, passwordList.Count);

        return passwordList[randomIndex];
    }
}

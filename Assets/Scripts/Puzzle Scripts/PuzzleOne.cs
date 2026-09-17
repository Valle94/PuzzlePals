using UnityEngine;
using TMPro;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using JetBrains.Annotations;
using System.Dynamic;

public class PuzzleOne : MonoBehaviour
{
    [Header("Text Fields")]
    [SerializeField] private TMP_Text ansText;
    [SerializeField] private TMP_Text ranShapetext;

    [Header("Player Canvas")]
    [SerializeField] private Canvas playerMKCanvas;
    [SerializeField] private Canvas playerCCanvas;

    [Header("Level Exit")]
    [SerializeField] private LevelExit levelExit;

    [SerializeField] private List<string> passShapeList = new List<string>() {"Cube", "Diamond", "Triangle", "Square"};
    private string password;
    private bool completePuzzle = false;
    void Start()
    {
        // Double checks the canvas for both players are off
        playerMKCanvas.enabled = false;
        playerCCanvas.enabled = false;

        // get the randome shape being used and assigns it to the text in the canvas too
        string shapeAns = GetRandomShape(passShapeList);
        ranShapetext.text = shapeAns;
        
        // Depending on the shape picked the password string is assigned to the correct passphrase
        switch (shapeAns)
        {
            case "Cube":
                password = "1234";
                break;

            case "Diamond":
                password = "2468";
                break;

            case "Triangle":
                password = "1357";
                break;
            
            case "Square":
                password = "9876";
                break;
            
            default:
                Debug.Log("Shape Not Found");
                break;
        }
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

    // When a player interacts with the puzzle their spefic UI will open
    public void Interact(GameObject player)
    {
        if (player.CompareTag("Player1"))
        {
            playerMKCanvas.enabled = true;
        }

        else if (player.CompareTag("Player2"))
        {
            playerCCanvas.enabled = true;
        }
    }

//When the player closes the puzzle the UI closes too
    public void OnClose(GameObject player)
    {
        if (player.CompareTag("Player1"))
        {
            playerMKCanvas.enabled = false;
        }

        else if (player.CompareTag("Player2"))
        {
            playerCCanvas.enabled = false;
        }
    }

// When they click the button it will check to see if the players answer is the same as the pass phrase
//If it is right and the puzzle is compleet the UI truns off for both players
    public void SubmitAnswer()
    {
        string playerAns = ansText.text;

        if (playerAns == password)
        {
            completePuzzle = true;
        }

        if (completePuzzle == true)
        {
            levelExit.isLocked = false;
            playerCCanvas.enabled = false;
            playerMKCanvas.enabled = false;
        }
    }
}

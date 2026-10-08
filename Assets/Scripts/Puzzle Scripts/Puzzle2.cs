using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.UI;

public class Puzzle2 : MonoBehaviour
{
    [Header("Player One")]
    [SerializeField] Canvas playerMKCanvas;
    [SerializeField] GameObject panelOne;
    [SerializeField] GameObject panelTwo;
    [SerializeField] TMP_Dropdown gridSizeDropdown;
    [SerializeField] TMP_Dropdown shadedAmountDropDown;
    [SerializeField] Transform playerOneGridParent;


    [Header("Player Two")]
    [SerializeField] Canvas playerCCanvas;
    [SerializeField] Transform playerTwoGridParent;

    [Header("Grid")]
    [SerializeField] GameObject gridButtonPrefab;

    [Header("Door")]
    [SerializeField] private LevelExit levelExit;

    [Header("Puzzle Data")]
    [SerializeField] bool completePuzzle = false;
    private int gridSize;
    private int shadedAmount;
    private List<int> correctShadedSquares = new List<int>();
    private List<int> playerShadedSquares = new List<int>();

    private bool playerOneGridCreated = false;

    private void GeneratePuzzle()
    {
        // Randomly chooses a grid size of a 2x2, 3x3, or 4x4
        gridSize = Random.Range(2, 5);

        // get total number of buttons needed
        int totalSquares = gridSize * gridSize;

        // Randomly choose how many squares are shaded
        shadedAmount = Random.Range(1, Mathf.Min(totalSquares, 6) + 1);

        correctShadedSquares.Clear();

        // Keep choosing random squares 
        while (correctShadedSquares.Count < shadedAmount)
        {
            int randomSquare = Random.Range(0, totalSquares);

            if (!correctShadedSquares.Contains(randomSquare))
            {
                correctShadedSquares.Add(randomSquare);
            }
        }

        Debug.Log("Grid Size: " + gridSize + "x" + gridSize);
        Debug.Log("Shaded Squares: " + shadedAmount);
    }

    private void CreatePlayerOneGrid()
    {
        playerShadedSquares.Clear();

        GridLayoutGroup gridLayout =
            playerOneGridParent.GetComponent<GridLayoutGroup>();

        gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        gridLayout.constraintCount = gridSize;

        int totalSquares = gridSize * gridSize;

        for (int i = 0; i < totalSquares; i++)
        {
            GameObject square =
                Instantiate(gridButtonPrefab, playerOneGridParent);

            Button button = square.GetComponent<Button>();

            button.interactable = true;
            button.image.color = Color.white;

            int squareIndex = i;

            button.onClick.AddListener(() =>
                ToggleSquare(squareIndex, button));
        }
    }


    private void CreatePlayerTwoGrid()
    {
        GridLayoutGroup gridLayout = playerTwoGridParent.GetComponent<GridLayoutGroup>();

        gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        gridLayout.constraintCount = gridSize;

        int totalSquares = gridSize * gridSize;

        for (int i = 0; i < totalSquares; i++)
        {
            GameObject square =
                Instantiate(gridButtonPrefab, playerTwoGridParent);

            Button button = square.GetComponent<Button>();

            // Player 2 cannot click these
            button.interactable = false;

            // Check whether this square should be shaded
            if (correctShadedSquares.Contains(i))
            {
                button.image.color = Color.red;
            }
            else
            {
                button.image.color = Color.white;
            }
        }
    }

    private void ToggleSquare(int index, Button button)
    {
        if (playerShadedSquares.Contains(index))
        {
            playerShadedSquares.Remove(index);
            button.image.color = Color.white;
        }
        else
        {
            playerShadedSquares.Add(index);
            button.image.color = Color.red;
        }
    }

    public void SubmitGrid()
    {

        if (completePuzzle)
        {
            return;
        }

        
        if (playerShadedSquares.Count != correctShadedSquares.Count)
        {
            Debug.Log("Incorrect pattern!");
            return;
        }

        foreach (int square in correctShadedSquares)
        {
            if (!playerShadedSquares.Contains(square))
            {
                Debug.Log("Incorrect pattern!");
                return;
            }
        }

        completePuzzle = true;

        if (completePuzzle == true)
        {
            levelExit.isLocked = false;
            playerCCanvas.GetComponent<Canvas>().enabled = (false);
            playerMKCanvas.GetComponent<Canvas>().enabled = (false);
            PlayerInteraction[] players = FindObjectsByType<PlayerInteraction>();
            foreach (var player in players)
            {
                player.CloseUI();
            }
        }

        Debug.Log("PUZZLE TWO COMPLETE!");
    }

    public void SubmitQuestions()
    {
        // Assumes dropdown options are 2x2, 3x3, 4x4
        int selectedGridSize = gridSizeDropdown.value + 2;

        // Assumes shaded amount options start at 1
        int selectedShadedAmount = shadedAmountDropDown.value + 1;

        if (selectedGridSize == gridSize &&
    selectedShadedAmount == shadedAmount)
        {
            Debug.Log("Correct answers!");

            panelOne.SetActive(false);
            panelTwo.SetActive(true);

            if (!playerOneGridCreated)
            {
                CreatePlayerOneGrid();
                playerOneGridCreated = true;
            }
        }
        else
        {
            Debug.Log("Incorrect answers! Try again.");
        }
    }

    public void Interact(GameObject player)
    {
        Debug.Log(player.tag);
        if (player.CompareTag("Player1"))
        {
            Debug.Log(player.CompareTag("Player1"));
            playerMKCanvas.GetComponent<Canvas>().enabled = (true);

        }

        else if (player.CompareTag("Player2"))
        {
            playerCCanvas.enabled = true;
        }
    }

    public void OnClose(GameObject player)
    {
        if (player.CompareTag("Player1"))
        {
            playerMKCanvas.GetComponent<Canvas>().enabled = (false);
        }

        else if (player.CompareTag("Player2"))
        {
            playerCCanvas.GetComponent<Canvas>().enabled = (false);
        }
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        // Double checks the canvas for both players are off
        playerMKCanvas.GetComponent<Canvas>().enabled = (false);
        playerCCanvas.GetComponent<Canvas>().enabled = (false);

        GeneratePuzzle();
        CreatePlayerTwoGrid();
    }

    // Update is called once per frame
    void Update()
    {

    }
}

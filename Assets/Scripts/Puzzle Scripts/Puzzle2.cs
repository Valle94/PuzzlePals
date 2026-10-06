using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class Puzzle2 : MonoBehaviour
{
    [Header("Player One")]
    [SerializeField] Canvas playerOneCanvas;
    [SerializeField] GameObject panelOne;
    [SerializeField] GameObject panelTwo;
    [SerializeField] TMP_Dropdown gridSizeDropdown;
    [SerializeField] TMP_Dropdown shadedAmountDropDown;
    [SerializeField] Transform playerOneGridParent;


    [Header("Player Two")]
    [SerializeField] Canvas playerTwoCanvas;
    [SerializeField] Transform playerTwoGridParent;

    [Header("Grid")]
    [SerializeField] GameObject gridButtonPrefab;

    [Header("Puzzle Data")]
    [SerializeField] bool completePuzzle = false;
    private int gridSize;
    private int shadedAmount;
    private List<int> correctShadedSquares = new List<int>();
    private List<int> playerShadedSquares = new List<int>();

    private void GeneratePuzzle()
    {
        // Randomly chooses a grid size of a 2x2, 3x3, or 4x4
        gridSize = Random.Range(2,5);

        // get total number of buttons needed
        int totalSquares = gridSize * gridSize;

        // Randomly choose how many squares are shaded
        shadedAmount = Random.Range(1,7);

        correctShadedSquares.Clear();

        // Keep choosing random squares 
    }
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

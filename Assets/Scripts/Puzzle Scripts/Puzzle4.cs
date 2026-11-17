using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Puzzle4 : MonoBehaviour
{
    [Header("Interface")]
    [SerializeField] Canvas playerMKCanvas;   // Player 1 (guesser) canvas
    [SerializeField] Canvas playerCCanvas;    // Player 2 (color observer) canvas

    [Header("Puzzle Params")]
    [SerializeField] private int maxGuesses = 6;
    [SerializeField] private int wordLength = 5;
    [SerializeField] private TextAsset wordListFile;
    [SerializeField] private TextAsset validGuessFile; // Optional; falls back to wordListFile

    [Header("Player 1 UI (Guesser)")]
    [SerializeField] private TMP_InputField guessInput;
    [SerializeField] private Button submitButton;
    [SerializeField] private TextMeshProUGUI[] remainingGuessesTexts;
    // Plain-text rows of previous guesses, row-major. e.g. 6 rows x 1 TMP_Text each,
    // or 6 rows x 5 letter TMP_Texts. Your call on visual style.
    [SerializeField] private TextMeshProUGUI[] player1LetterTiles;

    [Header("Player 2 UI (Colors Only)")]
    // Flat array: resultTiles[row * wordLength + col]
    [SerializeField] private Image[] player2ResultTiles;

    [Header("Tile Colors")]
    [SerializeField] private Color emptyColor = Color.gray;
    [SerializeField] private Color correctColor = new Color(0.42f, 0.65f, 0.31f); // Green
    [SerializeField] private Color presentColor = new Color(0.78f, 0.67f, 0.27f); // Yellow
    [SerializeField] private Color absentColor = new Color(0.31f, 0.31f, 0.31f); // Dark gray

    [Header("Audio")]
    [SerializeField] private AudioSource leftAudio;
    [SerializeField] private AudioSource rightAudio;
    [SerializeField] private AudioClip failSound;
    [SerializeField] private AudioClip successSound;
    [SerializeField] private AudioClip[] revealSounds; // [0]=absent, [1]=present, [2]=correct

    [Header("Door")]
    [SerializeField] private LevelExit levelExit;

    // --- Internal state ---
    private List<string> masterWordList = new List<string>();
    private HashSet<string> validGuesses = new HashSet<string>();

    private string masterWord;
    private int currentRow = 0;

    private enum State { Idle, PlayerGuessing, Won, Lost }
    private State currentState = State.Idle;

    // ============================================================
    // Lifecycle
    // ============================================================
    void Start()
    {
        playerMKCanvas.GetComponent<Canvas>().enabled = false;
        playerCCanvas.GetComponent<Canvas>().enabled = false;
        guessInput.characterLimit = wordLength;
        SetInputInteractable(false);
        LoadWordLists();
        ClearAllTiles();
        ClearAllGuessRows();
    }

    public void Interact(GameObject player)
    {
        if (player.CompareTag("Player1"))
        {
            playerMKCanvas.GetComponent<Canvas>().enabled = true;
            StartNewRound();   // Player 1 opening the panel begins the round
        }
        else if (player.CompareTag("Player2"))
        {
            playerCCanvas.GetComponent<Canvas>().enabled = true;
        }
    }

    public void OnClose(GameObject player)
    {
        ResetPuzzle();
        if (player.CompareTag("Player1"))
            playerMKCanvas.GetComponent<Canvas>().enabled = false;
        else if (player.CompareTag("Player2"))
            playerCCanvas.GetComponent<Canvas>().enabled = false;
    }

    // ============================================================
    // Round setup
    // ============================================================
    private void StartNewRound()
    {
        if (currentState == State.PlayerGuessing) return; // already running
        GenerateMasterWord();
        currentRow = 0;
        currentState = State.PlayerGuessing;
        SetInputInteractable(true);
        UpdateRemainingUI();          // NEW
    }

    private void GenerateMasterWord()
    {
        if (masterWordList.Count == 0)
        {
            Debug.LogError("Master word list is empty!");
            masterWord = "ERROR";
            return;
        }
        masterWord = masterWordList[Random.Range(0, masterWordList.Count)];
        Debug.Log($"Master Word: {masterWord}");
    }

    // ============================================================
    // Player 1 submits a guess
    // ============================================================
    public void OnGuessSubmitted()
    {
        if (currentState != State.PlayerGuessing) return;
        if (currentRow >= maxGuesses) return;

        string guess = guessInput.text.Trim().ToUpper();

        if (guess.Length != wordLength)
        {
            Debug.Log($"Word must be {wordLength} letters.");
            return;
        }
        if (!validGuesses.Contains(guess))
        {
            Debug.Log("Not a valid word.");
            leftAudio.PlayOneShot(failSound, 0.2f);
            return;
        }

        guessInput.text = string.Empty;
        StartCoroutine(SubmitGuessRoutine(guess));
    }

    IEnumerator SubmitGuessRoutine(string guess)
    {
        SetInputInteractable(false);

        // Player 1 sees their guess immediately, as plain text, no colors.
        WriteGuessToPlayer1Grid(currentRow, guess);

        // Player 2 sees the color pattern only — no letters on their tiles.
        int[] pattern = EvaluateGuess(masterWord, guess);
        yield return StartCoroutine(RevealRowRoutine(currentRow, pattern));

        // Win?
        if (guess == masterWord)
        {
            currentState = State.Won;
            UpdateRemainingUI();          // NEW
            rightAudio.PlayOneShot(successSound, 0.3f);
            levelExit.UnlockDoor();
            yield break;
        }

        currentRow++;

        // Loss?
        if (currentRow >= maxGuesses)
        {
            currentState = State.Lost;
            UpdateRemainingUI();          // NEW
            Debug.Log($"Out of guesses. The word was {masterWord}.");
            leftAudio.PlayOneShot(failSound, 0.2f);
            rightAudio.PlayOneShot(failSound, 0.2f);
            yield return new WaitForSeconds(1.0f);
            ResetPuzzle();
            StartNewRound();  // fresh word, still in the open panel
            yield break;
        }

        SetInputInteractable(true);
        UpdateRemainingUI();          // NEW
    }

    // ============================================================
    // Player 2 reveal — colors only, one tile at a time
    // ============================================================
    IEnumerator RevealRowRoutine(int row, int[] pattern)
    {
        for (int col = 0; col < wordLength; col++)
        {
            int idx = row * wordLength + col;
            if (idx >= player2ResultTiles.Length) yield break;

            Image tile = player2ResultTiles[idx];
            tile.color = PatternToColor(pattern[col]);

            int soundIdx = Mathf.Clamp(pattern[col], 0, 2);
            if (revealSounds != null && soundIdx < revealSounds.Length && revealSounds[soundIdx] != null)
                rightAudio.PlayOneShot(revealSounds[soundIdx], 0.4f);

            yield return new WaitForSeconds(0.25f);
        }
        yield return new WaitForSeconds(0.15f);
    }

    private Color PatternToColor(int value) => value switch
    {
        2 => correctColor,
        1 => presentColor,
        _ => absentColor
    };

    // ============================================================
    // Standard Wordle scoring — correct for duplicate letters
    // ============================================================
    private int[] EvaluateGuess(string target, string guess)
    {
        int len = wordLength;
        int[] result = new int[len];
        bool[] targetUsed = new bool[len];

        // Greens first
        for (int i = 0; i < len; i++)
            if (guess[i] == target[i]) { result[i] = 2; targetUsed[i] = true; }

        // Then yellows against remaining unused target letters
        for (int i = 0; i < len; i++)
        {
            if (result[i] == 2) continue;
            for (int j = 0; j < len; j++)
            {
                if (targetUsed[j]) continue;
                if (guess[i] == target[j])
                {
                    result[i] = 1;
                    targetUsed[j] = true;
                    break;
                }
            }
        }
        return result;
    }

    // ============================================================
    // Reset & clear
    // ============================================================
    public void ResetPuzzle()
    {
        StopAllCoroutines();
        currentState = State.Idle;
        currentRow = 0;
        masterWord = null;
        guessInput.text = string.Empty;
        SetInputInteractable(false);
        ClearAllTiles();
        ClearAllGuessRows();
        UpdateRemainingUI();          // NEW
    }

    private void ClearAllTiles()
    {
        if (player2ResultTiles == null) return;
        foreach (var tile in player2ResultTiles)
            if (tile != null) tile.color = emptyColor;
    }

    private void ClearAllGuessRows()
    {
        if (player1LetterTiles == null) return;
        foreach (var cell in player1LetterTiles)
            if (cell != null) cell.text = string.Empty;
    }

    // ============================================================
    // Helpers
    // ============================================================
    private void SetInputInteractable(bool state)
    {
        if (guessInput != null) guessInput.interactable = state;
        if (submitButton != null) submitButton.interactable = state;
        if (state) guessInput.ActivateInputField();
    }

    private void UpdateRemainingUI()
    {
        if (remainingGuessesTexts == null) return;

        if (currentState == State.Won)
            foreach (TextMeshProUGUI textfield in remainingGuessesTexts)
            textfield.text = "Solved!";
        else if (currentState == State.Lost)
            foreach (TextMeshProUGUI textfield in remainingGuessesTexts)
            textfield.text = "0 Guesses Remaining";
        else
            foreach (TextMeshProUGUI textfield in remainingGuessesTexts)
            textfield.text = $"{maxGuesses - currentRow} Guesses Remaining";
    }

    private void WriteGuessToPlayer1Grid(int row, string guess)
    {
        for (int col = 0; col < wordLength; col++)
        {
            int idx = row * wordLength + col;
            if (idx >= player1LetterTiles.Length) return;
            if (player1LetterTiles[idx] == null) continue;
            player1LetterTiles[idx].text = guess[col].ToString();
        }
    }

    // ============================================================
    // Word list loading
    // ============================================================
    private void LoadWordLists()
    {
        if (wordListFile != null)
        {
            foreach (var w in wordListFile.text.Split(
                new[] { '\r', '\n' }, System.StringSplitOptions.RemoveEmptyEntries))
            {
                string word = w.Trim().ToUpper();
                if (word.Length == wordLength) masterWordList.Add(word);
            }
        }
        else Debug.LogError("No master word list assigned!");

        TextAsset source = validGuessFile != null ? validGuessFile : wordListFile;
        if (source != null)
        {
            foreach (var w in source.text.Split(
                new[] { '\r', '\n' }, System.StringSplitOptions.RemoveEmptyEntries))
            {
                string word = w.Trim().ToUpper();
                if (word.Length == wordLength) validGuesses.Add(word);
            }
        }
    }
}
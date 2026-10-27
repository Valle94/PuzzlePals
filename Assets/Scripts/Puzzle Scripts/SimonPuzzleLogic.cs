using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SimonPuzzle : MonoBehaviour
{
    [Header("Puzzle Params")]
    [SerializeField] private int maxSequenceLength = 5;
    [SerializeField] private float flashDuration = 0.6f;
    [SerializeField] private float delayBetweenFlashes = 0.3f;

    [Header("Puzzle Objects")]
    [SerializeField] private Button[] buttons; // Assign your 4 color buttons here in inspector (0 to 3)

    private List<int> masterSequence = new List<int>();
    private List<int> userSequence = new List<int>();

    private enum State { WaitingToStart, PlayingSequence, WaitingForPlayer, Failed, Completed }
    private State currentState = State.WaitingToStart;

    void Start()
    {
        // Hook up button listeners automatically or do it in the Inspector
        for (int i = 0; i < buttons.Length; i++)
        {
            int index = i; // Local copy for closure
            buttons[i].onClick.AddListener(() => OnPlayerClickButton(index));
        }

        // Start the game loop automatically (or call this from a trigger)
        StartCoroutine(RunSimonGame());
    }

    /// <summary>
    /// Main game loop managing the progressive rounds.
    /// </summary>
    IEnumerator RunSimonGame()
    {
        GenerateMasterSequence();

        // Play progressively longer sequences (e.g., length 1, then 2, then 3...)
        for (int currentLength = 1; currentLength <= masterSequence.Count; currentLength++)
        {
            userSequence.Clear();

            // 1. Play the sequence for the current round length
            yield return StartCoroutine(PlaySequenceRoutine(currentLength));

            // 2. Wait for the player to match it step-by-step
            yield return StartCoroutine(GetPlayerResponseRoutine(currentLength));

            if (currentState == State.Failed)
            {
                Debug.Log("Game Over! You entered the wrong sequence.");
                SetButtonsInteractable(false);

                // Exit the loop entirely, stopping the puzzle sequence
                yield break;
            }

            // 3. Brief pause between rounds
            yield return new WaitForSeconds(1.0f);
        }

        currentState = State.Completed;
        Debug.Log("Puzzle Completed Successfully!");
    }

    void GenerateMasterSequence()
    {
        masterSequence.Clear();
        for (int i = 0; i < maxSequenceLength; i++)
        {
            // Assuming your buttons array has indices 0 through 3 (or 0 to buttons.Length - 1)
            masterSequence.Add(Random.Range(0, buttons.Length));
        }
        Debug.Log($"Master Sequence Generated: {string.Join(", ", masterSequence)}");
    }

    IEnumerator PlaySequenceRoutine(int length)
    {
        currentState = State.PlayingSequence;
        Debug.Log($"--- Round Length: {length} ---");

        // Disable buttons while computer is playing
        SetButtonsInteractable(false);

        yield return new WaitForSeconds(0.5f);

        for (int i = 0; i < length; i++)
        {
            int btnIndex = masterSequence[i];

            // Flash the button (Visual/Audio cue)
            yield return StartCoroutine(FlashButtonRoutine(btnIndex));

            yield return new WaitForSeconds(delayBetweenFlashes);
        }

        // Re-enable buttons for the player's turn
        SetButtonsInteractable(true);
    }

    IEnumerator FlashButtonRoutine(int index)
    {
        // Simple color tint flash or animation trigger placeholder
        Image img = buttons[index].GetComponent<Image>();
        Color originalColor = img.color;

        img.color = Color.white; // Flash color
        // TODO: If you have audio clips, play them here!

        yield return new WaitForSeconds(flashDuration);

        img.color = originalColor;
    }

    IEnumerator GetPlayerResponseRoutine(int length)
    {
        currentState = State.WaitingForPlayer;
        Debug.Log("Waiting for player input...");

        // We wait until the user has entered enough inputs for this round length
        // The actual validation happens live inside OnPlayerClickButton()
        yield return new WaitUntil(() => userSequence.Count == length || currentState == State.Failed);

        if (currentState == State.Failed)
        {
            Debug.Log("Wrong input! Resetting puzzle...");
            // Handle failure reset here if desired
            yield break;
        }
    }

    /// <label>Hook this up to your UI buttons' OnClick events (or assigned via code in Start)</label>
    public void OnPlayerClickButton(int index)
    {
        if (currentState != State.WaitingForPlayer) return;

        userSequence.Add(index);
        StartCoroutine(FlashButtonRoutine(index)); // Give immediate feedback on click

        int currentIndex = userSequence.Count - 1;

        // Check if the player clicked the correct button for this step
        if (userSequence[currentIndex] != masterSequence[currentIndex])
        {
            currentState = State.Failed;
            SetButtonsInteractable(false);
        }
        else
        {
            Debug.Log($"Correct step! Clicked {index}, expected {masterSequence[currentIndex]} ({userSequence.Count} so far)");
        }
    }

    private void SetButtonsInteractable(bool state)
    {
        foreach (var btn in buttons)
        {
            btn.interactable = state;
        }
    }

    public void ResetPuzzle()
    {
        // Stop any running coroutines just in case
        StopAllCoroutines();

        currentState = State.WaitingToStart;
        userSequence.Clear();

        // Restart the game loop
        StartCoroutine(RunSimonGame());
    }
}
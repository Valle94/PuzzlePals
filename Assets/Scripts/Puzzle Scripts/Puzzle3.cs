using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Puzzle3 : MonoBehaviour
{
    [Header("Interface")]
    [SerializeField] Canvas playerMKCanvas;
    [SerializeField] Canvas playerCCanvas;
    [SerializeField] Sprite[] darkImages;
    [SerializeField] Sprite[] litImages;

    [Header("Puzzle Params")]
    [SerializeField] private int maxSequenceLength = 5;
    [SerializeField] private float flashDuration = 0.6f;
    [SerializeField] private float delayBetweenFlashes = 0.3f;

    [Header("Puzzle Objects")]
    [SerializeField] private Button[] playerOneButtons; // Assign your 4 color buttons here in inspector (0 to 3)
    [SerializeField] private Button[] playerTwoButtons; // Assign your 4 color buttons here in inspector (0 to 3)
    [SerializeField] private Button readyButton;

    [Header("Audio")]
    [SerializeField] private AudioSource leftAudio;
    [SerializeField] private AudioSource rightAudio;
    [SerializeField] AudioClip failSound;
    [SerializeField] private AudioClip[] buttonSounds;

    [Header("Door")]
    [SerializeField] private LevelExit levelExit;

    private List<int> masterSequence = new List<int>();
    private List<int> userSequence = new List<int>();

    private enum State { WaitingToStart, PlayingSequence, WaitingForPlayer, Failed, Completed }
    private State currentState = State.WaitingToStart;

    void Start()
    {
        // Double checks the canvas for both players are off
        playerMKCanvas.GetComponent<Canvas>().enabled = (false);
        playerCCanvas.GetComponent<Canvas>().enabled = (false);
        SetButtonsInteractable(false);
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
        ResetPuzzle();
        if (player.CompareTag("Player1"))
        {
            playerMKCanvas.GetComponent<Canvas>().enabled = (false);
        }
        else if (player.CompareTag("Player2"))
        {
            playerCCanvas.GetComponent<Canvas>().enabled = (false);
        }
    }

    public void ReadyButtonClicked()
    {
        if (playerCCanvas.GetComponent<Canvas>().enabled == (false)) { return; }
        readyButton.enabled = false;
        readyButton.GetComponentInChildren<TextMeshProUGUI>().text = "Begin!";
        StartCoroutine(RunSimonGame());
    }

    IEnumerator RunSimonGame()
    {
        GenerateMasterSequence();

        // Play progressively longer sequences
        for (int currentLength = 1; currentLength <= masterSequence.Count; currentLength++)
        {
            userSequence.Clear();

            // Play the sequence for the current round length
            yield return StartCoroutine(PlaySequenceRoutine(currentLength));

            // Wait for the player to match it step-by-step
            yield return StartCoroutine(GetPlayerResponseRoutine(currentLength));

            if (currentState == State.Failed)
            {
                Debug.Log("Game Over! You entered the wrong sequence.");
                SetButtonsInteractable(false);
                leftAudio.PlayOneShot(failSound, 0.1f);
                rightAudio.PlayOneShot(failSound, 0.1f);

                ResetPuzzle();
            }

            // Brief pause between rounds
            yield return new WaitForSeconds(1.0f);
        }

        currentState = State.Completed;
        CompletePuzzle();
    }

    private void CompletePuzzle()
    {
        Debug.Log("Puzzle Completed Successfully!");
        readyButton.enabled = false;
        readyButton.GetComponentInChildren<TextMeshProUGUI>().text = "Well Done!";
        levelExit.isLocked = false;
    }

    IEnumerator PlaySequenceRoutine(int length)
    {
        currentState = State.PlayingSequence;
        Debug.Log($"--- Round Length: {length} ---");

        // Disable buttons while computer is playing
        SetButtonsInteractable(false);

        yield return new WaitForSeconds(2.0f);

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
        // If the computer is playing the sequence, adjust Player2 buttons
        if (currentState == State.PlayingSequence)
        {
            Image image = playerTwoButtons[index].GetComponent<Image>();
            image.sprite = litImages[index];

            rightAudio.PlayOneShot(buttonSounds[index]);

            yield return new WaitForSeconds(flashDuration);

            image.sprite = darkImages[index];
        }
        // If the Player1 is clicking the buttons, adjust Player1 buttons
        else if (currentState == State.WaitingForPlayer)
        {
            Image image = playerOneButtons[index].GetComponent<Image>();
            image.sprite = litImages[index];

            leftAudio.PlayOneShot(buttonSounds[index]);

            yield return new WaitForSeconds(flashDuration);

            image.sprite = darkImages[index];
        }
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
            yield break;
        }
    }

    // This is hooked up to each button in the editor
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
        foreach (Button button in playerOneButtons)
        {
            button.interactable = state;
        }
    }

    public void ResetPuzzle()
    {
        // Stop any running coroutines just in case
        StopAllCoroutines();

        currentState = State.WaitingToStart;
        userSequence.Clear();

        readyButton.enabled = true;
        readyButton.GetComponentInChildren<TextMeshProUGUI>().text = "Ready?";
    }

    void GenerateMasterSequence()
    {
        masterSequence.Clear();
        for (int i = 0; i < maxSequenceLength; i++)
        {
            masterSequence.Add(Random.Range(0, playerOneButtons.Length));
        }
        Debug.Log($"Master Sequence Generated: {string.Join(", ", masterSequence)}");
    }

}

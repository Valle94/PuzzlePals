using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.Events;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] Camera camera;
    [SerializeField] PlayerInput playerInput;
    [SerializeField] Image crosshair;

    [Header("Variables")]
    [Range(1, 1000)]
    [SerializeField] float raycastLength = 10;

    public UnityEvent onEscapePressed;

    private bool uiOpen = false;    // TEMP: Bool used for debugging purposes

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (camera == null) { camera = GetComponentInChildren<Camera>(); }
        if (playerInput == null) { playerInput = GetComponentInChildren<PlayerInput>(); }
    }

    // Update is called once per frame
    void Update()
    {
        ChangeCrosshair();
    }

    // This method shoots a raycast that interacts with world objects
    private void Interact()
    {
        RaycastHit hit;     // Instantiate raycast hit output

        if (Physics.Raycast(camera.transform.position, camera.transform.forward, out hit, raycastLength))
        {
            if (hit.collider.tag == "Puzzle" && uiOpen == false)
            {
                // TEMP: Simulate opening a UI Puzzle
                uiOpen = true;
                Debug.Log($"You clicked a puzzle! UI Open: {uiOpen}");
                playerInput.SwitchCurrentActionMap("UI");
                CursorManager cursorManager = FindAnyObjectByType<CursorManager>();
                cursorManager.ShowCursor();
                PuzzleOne puzzle = hit.collider.GetComponent<PuzzleOne>();
                puzzle.Interact(this.gameObject);
                //puzzleOne.Interact(gameObject);
            }
            else
            {
                Debug.Log(hit.collider.name);
            }
        }
    }

    // This method changes the color of the crosshair when hovering over a puzzle
    private void ChangeCrosshair()
    {
        RaycastHit puzzleHover;

        if (Physics.Raycast(camera.transform.position, camera.transform.forward, out puzzleHover, raycastLength)
            && puzzleHover.collider.tag == "Puzzle")
        {
            crosshair.color = Color.green;
        }
        else
        {
            crosshair.color = Color.white;
        }
    }

    // This method closes the UI window 
    public void CloseUI()
    {
        if (uiOpen == true)
        {
            // TEMP: Simulate closing a UI puzzle
            uiOpen = false;
            Debug.Log($"Puzzle Closed. UI Open: {uiOpen}");
            playerInput.SwitchCurrentActionMap("Player");
            CursorManager cursorManager = FindAnyObjectByType<CursorManager>();
            cursorManager.HideCursor();

            PuzzleOne puzzle = FindAnyObjectByType<PuzzleOne>();
            puzzle.OnClose(this.gameObject);
        }
    }

    // Public player input methods
    public void OnInteract(InputValue value)
    {
        Interact();
    }

    public void OnCancel(InputValue value)
    {
        CloseUI();
    }


    public void OnEscape()
    {
        MenuManager menuManager = FindAnyObjectByType<MenuManager>();
        menuManager.OpenInGameMenu();
        playerInput.SwitchCurrentActionMap("UI");
        onEscapePressed?.Invoke();
    }
}

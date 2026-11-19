using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

// Attach to an object with a Collider. When a player clicks it (via PlayerInteraction),
// the current scene is reloaded.
public class RestartGame : MonoBehaviour
{
    [Header("End Room Text")]
    [SerializeField] TextMeshPro endRoomText;
    [TextArea]
    [SerializeField] string endRoomMessage = "Congratulations! You finished the game!\nClick here to restart.";

    private bool isRestarting = false;

    void Start()
    {
        
    }

    // Called by PlayerInteraction when the player clicks this object
    public void Interact(GameObject player)
    {
        Restart();
    }

    // Reloads the currently active scene
    public void Restart()
    {
        if (isRestarting) { return; }   // Prevent double-clicks from loading twice
        isRestarting = true;

        Time.timeScale = 1f;            // In case the game was paused

        Scene currentScene = SceneManager.GetActiveScene();
        Debug.Log($"Restarting scene: {currentScene.name}");
        SceneManager.LoadScene(currentScene.buildIndex);
    }

    public void SetEndText()
    {
        endRoomText.text = endRoomMessage;
    }
}

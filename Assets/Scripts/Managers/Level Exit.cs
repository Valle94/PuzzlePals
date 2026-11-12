
using UnityEngine;

public class LevelExit : MonoBehaviour
{
    [SerializeField] public bool isLocked = true;
    [SerializeField] private GameObject door;
    [SerializeField] private GameObject puzzle;
    [SerializeField] private Material unlockedMaterial;
    [SerializeField] private AudioManager audioManager;

    private Renderer doorRenderer;
    private Collider doorCollider;

    void Start()
    {
        door.SetActive(true);

        doorRenderer = door.GetComponent<Renderer>();
        doorCollider = door.GetComponent<Collider>();
    }

    void Update()
    {

    }

    public void UnlockDoor()
    {
        isLocked = false;
        // Play Correct Audio Clip
        audioManager.PlayDoorUnlock();
        // Change the door to green
        doorRenderer.material = unlockedMaterial;

        // Turn off the collider
        doorCollider.enabled = false;
    }
}



using UnityEngine;

public class LevelExit : MonoBehaviour
{
    [SerializeField] public bool isLocked = true;
    [SerializeField] private GameObject door;
    [SerializeField] private GameObject puzzle;
    [SerializeField] private Material unlockedMaterial;

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
        if (!isLocked)
        {
            // Change the door to green
            doorRenderer.material = unlockedMaterial;

            // Turn off the collider
            doorCollider.enabled = false;
        }
    }
}


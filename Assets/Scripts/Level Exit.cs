using Unity.VisualScripting;
using UnityEngine;

public class LevelExit : MonoBehaviour
{
    [SerializeField] public bool isLocked = true;
    [SerializeField] private GameObject door;
    [SerializeField] private GameObject puzzle;
    void Start()
    {
        door.SetActive(true);
    }

    
    void Update()
    {
        if (!isLocked)
        {
            door.SetActive(false);
        }

    }


}

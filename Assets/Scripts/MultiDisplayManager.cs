using UnityEngine;

public class MultiDisplayManager : MonoBehaviour
{
    void Start()
    {
        Debug.Log("Displays connected: " + Display.displays.Length);

        // Display.displays[0] is primary and active by default.
        // Check for additional displays and activate them.
        if (Display.displays.Length > 1)
        {
            Display.displays[1].Activate();
        }
    }
}

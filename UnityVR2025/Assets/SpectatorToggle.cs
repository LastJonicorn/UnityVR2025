using UnityEngine;
using UnityEngine.UI;

public class SpectatorToggle : MonoBehaviour
{
    public Camera spectatorCamera;
    void Awake()
    {
        // Activate all connected displays (except HMD/main display)
        for (int i = 1; i < Display.displays.Length; i++)
            Display.displays[i].Activate();
    }

    void Start()
    {
        // Activate second display (monitor)
        if (Display.displays.Length > 1)
            Display.displays[1].Activate();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            if (spectatorCamera != null)
                spectatorCamera.enabled = !spectatorCamera.enabled;
        }
    }
}

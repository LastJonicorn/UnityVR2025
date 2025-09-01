using UnityEngine;

public class HandSettingsPanel : MonoBehaviour
{
    [Tooltip("Hand transform to follow")]
    public Transform handTransform;

    [Tooltip("Panel prefab to show")]
    public GameObject panelPrefab;

    private GameObject panelInstance;
    private bool isOpen = false;

    [Tooltip("Offset from the wrist")]
    public Vector3 localOffset = new Vector3(0, 0.1f, 0.2f);

    void Update()
    {
        if (isOpen && panelInstance != null && handTransform != null)
        {
            // Follow the hand with offset
            panelInstance.transform.position = handTransform.position + handTransform.rotation * localOffset;
            panelInstance.transform.rotation = Quaternion.LookRotation(Camera.main.transform.forward);
        }
    }

    public void TogglePanel()
    {
        Debug.Log("ToggleSettings");

        if (isOpen)
        {
            ClosePanel();
        }
        else
        {
            OpenPanel();
        }
    }

    private void OpenPanel()
    {
        if (panelPrefab == null || handTransform == null) return;

        panelInstance = Instantiate(panelPrefab);
        panelInstance.transform.position = handTransform.position + handTransform.rotation * localOffset;
        panelInstance.transform.rotation = Quaternion.LookRotation(Camera.main.transform.forward);
        isOpen = true;
    }

    private void ClosePanel()
    {
        if (panelInstance != null)
        {
            Destroy(panelInstance);
            panelInstance = null;
        }
        isOpen = false;
    }
}

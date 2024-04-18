using UnityEngine;

public class OptionsMenu : MonoBehaviour
{
    public KeyCode optionsMenuKey = KeyCode.Escape;
    public GameObject optionsMenuCanvas;

    private bool isOptionsMenuOpen = false;

    private void Start()
    {
        if (optionsMenuCanvas != null)
        {
            optionsMenuCanvas.SetActive(false); // Ensure options menu is initially closed
        }
    }

    private void Update()
    {
        // Check for input to toggle options menu
        if (Input.GetKeyDown(optionsMenuKey))
        {
            if (!isOptionsMenuOpen)
            {
                OpenOptions();
            }
            else
            {
                CloseOptions();
            }
        }
    }

    public void OpenOptions()
    {
        isOptionsMenuOpen = true;

        // Open options menu
        Time.timeScale = 0f; // Pause time
        optionsMenuCanvas.SetActive(true); // Show options menu
    }

    public void CloseOptions()
    {
        isOptionsMenuOpen = false;

        // Close options menu
        Time.timeScale = 1f; // Restore previous time scale
        optionsMenuCanvas.SetActive(false); // Hide options menu
    }
}

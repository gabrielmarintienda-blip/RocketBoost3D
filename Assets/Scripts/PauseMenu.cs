using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    [SerializeField] private GameObject hud;
    [SerializeField] private string menuSceneName = "MainMenu";

    [Header("Buttons")]
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button menuButton;

    private bool isPaused;
    private CursorLockMode previousLockState;
    private bool previousCursorVisible;

    void Awake()
    {
        resumeButton.onClick.AddListener(Resume);
        menuButton.onClick.AddListener(GoToMenu);
        panel.SetActive(false);
    }

    void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (isPaused) Resume();
            else Pause();
        }
    }

    private void Pause()
    {
        isPaused = true;
        Time.timeScale = 0f;

        // Remember the cursor so we can give it back on resume
        previousLockState = Cursor.lockState;
        previousCursorVisible = Cursor.visible;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (hud != null) hud.SetActive(false);
        panel.SetActive(true);
    }

    private void Resume()
    {
        isPaused = false;
        Time.timeScale = 1f;

        Cursor.lockState = previousLockState;
        Cursor.visible = previousCursorVisible;

        panel.SetActive(false);
        if (hud != null) hud.SetActive(true);
    }

    private void GoToMenu()
    {
        Time.timeScale = 1f;
        CheckpointData.Clear();
        SceneManager.LoadScene(menuSceneName);
    }
}
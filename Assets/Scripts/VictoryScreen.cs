using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class VictoryScreen : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    [SerializeField] private float showDelay = 1.5f;     // Time to enjoy the landing first
    [SerializeField] private string menuSceneName = "MainMenu";

    [Header("Hide on victory")]
    [SerializeField] private GameObject hud;
    [SerializeField] private float audioFadeTime = 1.5f;

    [Header("Buttons")]
    [SerializeField] private Button nextLevelButton;
    [SerializeField] private Button menuButton;

    void Awake()
    {
        nextLevelButton.onClick.AddListener(NextLevel);
        menuButton.onClick.AddListener(GoToMenu);
        panel.SetActive(false);
    }

    public void Show()
    {
        StartCoroutine(FadeOutAudio());
        StartCoroutine(ShowRoutine());
    }

    private IEnumerator ShowRoutine()
    {
        yield return new WaitForSeconds(showDelay);

        if (hud != null) hud.SetActive(false);

        // Hide the "next level" button if this is the last scene in the build
        bool hasNextLevel = SceneManager.GetActiveScene().buildIndex + 1 < SceneManager.sceneCountInBuildSettings;
        nextLevelButton.gameObject.SetActive(hasNextLevel);

        panel.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // Fades out ALL the sounds in the scene (music, effects, etc.)
    private IEnumerator FadeOutAudio()
    {
        float startVolume = AudioListener.volume;
        float t = 0f;

        while (t < audioFadeTime)
        {
            t += Time.deltaTime;
            AudioListener.volume = Mathf.Lerp(startVolume, 0f, t / audioFadeTime);
            yield return null;
        }

        AudioListener.volume = 0f;
    }

    private void NextLevel()
    {
        Time.timeScale = 1f;
        AudioListener.volume = 1f; // Bring the sound back
        CheckpointData.Clear(); // Don't spawn at an old level's checkpoint
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    private void GoToMenu()
    {
        Time.timeScale = 1f;
        AudioListener.volume = 1f; // Bring the sound back
        CheckpointData.Clear();
        SceneManager.LoadScene(menuSceneName);
    }
}
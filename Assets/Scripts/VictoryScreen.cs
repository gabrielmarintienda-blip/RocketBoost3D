using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class VictoryScreen : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    [SerializeField] private float showDelay = 1.5f;     // Time to enjoy the landing first
    [SerializeField] private string menuSceneName = "MainMenu";

    [Header("Hide on victory")]
    [SerializeField] private GameObject hud;
    [SerializeField] private AudioSource music;
    [SerializeField] private float musicFadeTime = 1.5f;

    [Header("Buttons")]
    [SerializeField] private GameObject nextLevelButton;

    void Awake()
    {
        panel.SetActive(false);
    }

    public void Show()
    {
        StartCoroutine(FadeOutMusic());
        StartCoroutine(ShowRoutine());
    }

    private IEnumerator ShowRoutine()
    {
        yield return new WaitForSeconds(showDelay);

        if (hud != null) hud.SetActive(false);

        // Hide the "next level" button if this is the last scene in the build
        bool hasNextLevel = SceneManager.GetActiveScene().buildIndex + 1 < SceneManager.sceneCountInBuildSettings;
        if (nextLevelButton != null) nextLevelButton.SetActive(hasNextLevel);

        panel.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private IEnumerator FadeOutMusic()
    {
        if (music == null) yield break;

        float startVolume = music.volume;
        float t = 0f;

        while (t < musicFadeTime)
        {
            t += Time.deltaTime;
            music.volume = Mathf.Lerp(startVolume, 0f, t / musicFadeTime);
            yield return null;
        }

        music.Stop();
    }

    // Hook this to the "Next level" button
    public void NextLevel()
    {
        Time.timeScale = 1f;
        CheckpointData.Clear(); // Don't spawn at an old level's checkpoint
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    // Hook this to the "Menu" button
    public void GoToMenu()
    {
        Time.timeScale = 1f;
        CheckpointData.Clear();
        SceneManager.LoadScene(menuSceneName);
    }
}
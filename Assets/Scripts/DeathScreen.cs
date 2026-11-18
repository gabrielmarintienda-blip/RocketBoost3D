using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DeathScreen : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    [SerializeField] private float showDelay = 2f;      // Time to let the ship fall first
    [SerializeField] private string menuSceneName = "MainMenu";

    [Header("Hide on death")]
    [SerializeField] private GameObject hud;
    [SerializeField] private AudioSource music;
    [SerializeField] private float musicFadeTime = 1.5f;

    [Header("Buttons")]
    [SerializeField] private Button retryButton;
    [SerializeField] private Button menuButton;

    void Awake()
    {
        retryButton.onClick.AddListener(Retry);
        menuButton.onClick.AddListener(GoToMenu);
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

    // Reloads the current level (keeps the checkpoint, if any)
    private void Retry()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void GoToMenu()
    {
        Time.timeScale = 1f;
        CheckpointData.Clear();
        SceneManager.LoadScene(menuSceneName);
    }
}
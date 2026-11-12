using System.Collections;
using UnityEngine;
using DG.Tweening;

public class TimeStopManager : MonoBehaviour
{
    public static TimeStopManager Instance { get; private set; }
    public static bool IsStopped { get; private set; }

    [Header("Time stop")]
    [SerializeField] private float duration = 5f;
    [SerializeField] private AudioClip timeStopSfx;
    [SerializeField] private AudioClip timeResumeSfx;   // Optional
    [SerializeField] private AudioSource music;         // Optional: paused while time is stopped

    [Header("Visual effect")]
    [SerializeField] private Material effectMaterial;
    [SerializeField] private float waveDuration = 0.6f;
    [SerializeField] private float maxRadius = 2.5f;

    private Transform player;
    private Coroutine routine;
    private float stopEndTime;

    void Awake()
    {
        Instance = this;
        IsStopped = false;
        ResetEffect();
    }

    void Start()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null) player = playerObject.transform;
    }

    void OnDestroy()
    {
        IsStopped = false;
        ResetEffect();
    }

    // Called by the power-up
    public void Activate()
    {
        // Picking up another one while time is stopped just refreshes the timer
        stopEndTime = Time.time + duration;

        if (routine == null)
            routine = StartCoroutine(TimeStopRoutine());
    }

    private IEnumerator TimeStopRoutine()
    {
        IsStopped = true;
        DOTween.PauseAll();
        PlaySfx(timeStopSfx);
        if (music != null) music.Pause();

        yield return PlayWave(false);

        // The effect stays on screen while time is stopped
        stopEndTime = Mathf.Max(stopEndTime, Time.time);
        while (Time.time < stopEndTime)
            yield return null;

        // Time resumes together with the reverse wave
        IsStopped = false;
        DOTween.PlayAll();
        PlaySfx(timeResumeSfx);
        if (music != null) music.UnPause();

        yield return PlayWave(true);

        ResetEffect();
        routine = null;
    }

    private IEnumerator PlayWave(bool reverse)
    {
        if (effectMaterial == null) yield break;

        float edge = effectMaterial.GetFloat("_EdgeWidth");
        effectMaterial.SetFloat("_Reverse", reverse ? 1f : 0f);
        effectMaterial.SetVector("_Center", GetPlayerViewportPosition());

        float t = 0f;
        while (t < waveDuration)
        {
            t += Time.deltaTime;

            // Ease-out: the wave starts fast and slows down
            float k = Mathf.Clamp01(t / waveDuration);
            k = 1f - (1f - k) * (1f - k);

            effectMaterial.SetFloat("_Radius", Mathf.Lerp(-edge, maxRadius, k));
            yield return null;
        }

        effectMaterial.SetFloat("_Radius", maxRadius);
    }

    private Vector4 GetPlayerViewportPosition()
    {
        Camera cam = Camera.main;
        if (cam == null || player == null) return new Vector4(0.5f, 0.5f, 0f, 0f);

        Vector3 viewport = cam.WorldToViewportPoint(player.position);
        return new Vector4(viewport.x, viewport.y, 0f, 0f);
    }

    // Radius -1 = effect off. Needed because the material is an asset and keeps its values
    private void ResetEffect()
    {
        if (effectMaterial == null) return;

        effectMaterial.SetFloat("_Radius", -1f);
        effectMaterial.SetFloat("_Reverse", 0f);
    }

    private void PlaySfx(AudioClip clip)
    {
        if (clip != null && Camera.main != null)
            AudioSource.PlayClipAtPoint(clip, Camera.main.transform.position);
    }
}
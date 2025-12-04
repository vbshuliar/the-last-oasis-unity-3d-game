using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneTransitionManager : MonoBehaviour
{
    public static SceneTransitionManager Instance { get; private set; }

    [Header("Transition Settings")]
    [SerializeField] private float fadeDuration = 1f;
    [SerializeField] private Image fadeImage;

    private bool isTransitioning = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            
            // create fade image if not assigned
            if (fadeImage == null)
            {
                CreateFadeImage();
            }
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void CreateFadeImage()
    {
        GameObject canvasObj = new GameObject("TransitionCanvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 9999; // always on top
        canvasObj.AddComponent<CanvasScaler>();
        canvasObj.AddComponent<GraphicRaycaster>();

        GameObject imageObj = new GameObject("FadeImage");
        imageObj.transform.SetParent(canvasObj.transform, false);
        fadeImage = imageObj.AddComponent<Image>();
        fadeImage.color = new Color(0, 0, 0, 0);
        
        RectTransform rect = fadeImage.GetComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.sizeDelta = Vector2.zero;
    }

    public void LoadScene(string sceneName)
    {
        if (!isTransitioning)
        {
            StartCoroutine(TransitionCoroutine(sceneName));
        }
    }

    public void LoadScene(int sceneIndex)
    {
        if (!isTransitioning)
        {
            StartCoroutine(TransitionCoroutine(sceneIndex));
        }
    }

    IEnumerator TransitionCoroutine(string sceneName)
    {
        isTransitioning = true;

        // fade out
        yield return StartCoroutine(FadeOut());

        // load scene
        SceneManager.LoadScene(sceneName);

        // fade in
        yield return StartCoroutine(FadeIn());

        isTransitioning = false;
    }

    IEnumerator TransitionCoroutine(int sceneIndex)
    {
        isTransitioning = true;

        // fade out
        yield return StartCoroutine(FadeOut());

        // load scene
        SceneManager.LoadScene(sceneIndex);

        // fade in
        yield return StartCoroutine(FadeIn());

        isTransitioning = false;
    }

    IEnumerator FadeOut()
    {
        if (fadeImage == null) yield break; // yield break exits coroutine early

        float elapsed = 0f;
        Color color = fadeImage.color;

        // gradually increase alpha transparency from 0 to 1 over fadeDuration
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            color.a = Mathf.Clamp01(elapsed / fadeDuration); // alpha goes from 0 to 1
            fadeImage.color = color;
            yield return null; // wait one frame before continuing
        }

        color.a = 1f;
        fadeImage.color = color;
    }

    IEnumerator FadeIn()
    {
        if (fadeImage == null) yield break;

        float elapsed = 0f;
        Color color = fadeImage.color;
        color.a = 1f;

        // gradually decrease alpha from 1 to 0 over fadeDuration
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            color.a = 1f - Mathf.Clamp01(elapsed / fadeDuration); // alpha goes from 1 to 0
            fadeImage.color = color;
            yield return null;
        }

        color.a = 0f;
        fadeImage.color = color;
    }

    public void FadeOutOnly(float duration = -1f)
    {
        if (duration < 0) duration = fadeDuration;
        StartCoroutine(FadeOutCoroutine(duration));
    }

    public void FadeInOnly(float duration = -1f)
    {
        if (duration < 0) duration = fadeDuration;
        StartCoroutine(FadeInCoroutine(duration));
    }

    IEnumerator FadeOutCoroutine(float duration)
    {
        if (fadeImage == null) yield break;

        float elapsed = 0f;
        Color color = fadeImage.color;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            color.a = Mathf.Clamp01(elapsed / duration);
            fadeImage.color = color;
            yield return null;
        }

        color.a = 1f;
        fadeImage.color = color;
    }

    IEnumerator FadeInCoroutine(float duration)
    {
        if (fadeImage == null) yield break;

        float elapsed = 0f;
        Color color = fadeImage.color;
        color.a = 1f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            color.a = 1f - Mathf.Clamp01(elapsed / duration);
            fadeImage.color = color;
            yield return null;
        }

        color.a = 0f;
        fadeImage.color = color;
    }
}


using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// provides fade in and fade out transitions when loading scenes
public class SceneTransitionManager : MonoBehaviour
{
    public static SceneTransitionManager Instance { get; private set; }

    [Header("Transition Settings")]
    [SerializeField] private float fadeDuration = 1f;
    [SerializeField] private Color fadeColor = Color.black;
    [SerializeField] private Image fadeImage;

    private bool isTransitioning;
    private Coroutine transitionRoutine;

    // builds the singleton instance and ensures a fade image exists
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (fadeImage == null)
        {
            CreateFadeOverlay();
        }
        else
        {
            PrepareFadeImage(fadeImage);
        }
    }

    // begins with a fade in so the screen starts visible
    void Start()
    {
        if (fadeImage == null)
        {
            return;
        }

        Color color = fadeImage.color;
        color.a = 1f;
        fadeImage.color = color;
        StartCoroutine(FadeIn());
    }

    // creates a fullscreen ui image to drive the fade effect
    void CreateFadeOverlay()
    {
        GameObject canvasObj = new GameObject("TransitionCanvas");
        canvasObj.transform.SetParent(transform, false);

        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = short.MaxValue;

        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);

        canvasObj.AddComponent<GraphicRaycaster>();

        GameObject imageObj = new GameObject("FadeImage");
        imageObj.transform.SetParent(canvasObj.transform, false);
        fadeImage = imageObj.AddComponent<Image>();

        RectTransform rect = fadeImage.rectTransform;
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        PrepareFadeImage(fadeImage);
    }

    // configures the fade image color and persistence
    void PrepareFadeImage(Image targetImage)
    {
        if (targetImage == null)
        {
            return;
        }

        targetImage.color = new Color(fadeColor.r, fadeColor.g, fadeColor.b, 0f);
        targetImage.raycastTarget = false; // enable only while fading

        Transform root = targetImage.transform.root;
        if (root != null)
        {
            DontDestroyOnLoad(root.gameObject);
        }
    }

    // fades out, loads by name, then fades in again
    public void LoadScene(string sceneName)
    {
        if (isTransitioning)
        {
            return;
        }

        transitionRoutine = StartCoroutine(TransitionRoutine(() => SceneManager.LoadSceneAsync(sceneName)));
    }

    // fades out, loads by build index, then fades in again
    public void LoadScene(int sceneIndex)
    {
        if (isTransitioning)
        {
            return;
        }

        transitionRoutine = StartCoroutine(TransitionRoutine(() => SceneManager.LoadSceneAsync(sceneIndex)));
    }

    // runs the fade out, load, and fade in sequence
    IEnumerator TransitionRoutine(System.Func<AsyncOperation> operationFactory)
    {
        isTransitioning = true;

        yield return FadeOut();
        yield return LoadSceneAsync(operationFactory);
        yield return FadeIn();

        isTransitioning = false;
        transitionRoutine = null;
    }

    // loads scenes asynchronously and waits for completion
    IEnumerator LoadSceneAsync(System.Func<AsyncOperation> operationFactory)
    {
        AsyncOperation asyncOperation = operationFactory?.Invoke();

        if (asyncOperation == null)
        {
            yield break;
        }

        asyncOperation.allowSceneActivation = false;

        while (asyncOperation.progress < 0.9f)
        {
            yield return null;
        }

        asyncOperation.allowSceneActivation = true;

        while (!asyncOperation.isDone)
        {
            yield return null;
        }
    }

    // helper that fades alpha from transparent to opaque
    IEnumerator FadeOut()
    {
        yield return Fade(0f, 1f, fadeDuration);
    }

    // helper that fades alpha from opaque to transparent
    IEnumerator FadeIn()
    {
        yield return Fade(1f, 0f, fadeDuration);
    }

    // interpolates the fade image alpha over time
    IEnumerator Fade(float from, float to, float duration)
    {
        if (fadeImage == null)
        {
            yield break;
        }

        float elapsed = 0f;
        Color color = fadeImage.color;
        fadeImage.raycastTarget = true;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = duration > 0f ? Mathf.Clamp01(elapsed / duration) : 1f;
            color.a = Mathf.Lerp(from, to, t);
            fadeImage.color = color;
            yield return null;
        }

        color.a = to;
        fadeImage.color = color;
        fadeImage.raycastTarget = to > 0f;
    }

    // triggers only a fade out without loading a scene
    public void FadeOutOnly(float duration = -1f)
    {
        if (fadeImage == null)
        {
            return;
        }

        StartCoroutine(Fade(0f, 1f, duration < 0f ? fadeDuration : duration));
    }

    // triggers only a fade in without loading a scene
    public void FadeInOnly(float duration = -1f)
    {
        if (fadeImage == null)
        {
            return;
        }

        StartCoroutine(Fade(1f, 0f, duration < 0f ? fadeDuration : duration));
    }
}


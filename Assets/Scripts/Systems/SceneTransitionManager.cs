using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneTransitionManager : MonoBehaviour
{
    public static SceneTransitionManager Instance { get; private set; }

    [Header("Transition Settings")]
    [SerializeField] private float fadeDuration = 1f;
    [SerializeField] private Color fadeColor = Color.black;
    [SerializeField] private Image fadeImage;

    private bool isTransitioning;
    private Coroutine transitionRoutine;

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

    public void LoadScene(string sceneName)
    {
        if (isTransitioning)
        {
            return;
        }

        transitionRoutine = StartCoroutine(TransitionRoutine(() => SceneManager.LoadSceneAsync(sceneName)));
    }

    public void LoadScene(int sceneIndex)
    {
        if (isTransitioning)
        {
            return;
        }

        transitionRoutine = StartCoroutine(TransitionRoutine(() => SceneManager.LoadSceneAsync(sceneIndex)));
    }

    IEnumerator TransitionRoutine(System.Func<AsyncOperation> operationFactory)
    {
        isTransitioning = true;

        yield return FadeOut();
        yield return LoadSceneAsync(operationFactory);
        yield return FadeIn();

        isTransitioning = false;
        transitionRoutine = null;
    }

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

    IEnumerator FadeOut()
    {
        yield return Fade(0f, 1f, fadeDuration);
    }

    IEnumerator FadeIn()
    {
        yield return Fade(1f, 0f, fadeDuration);
    }

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

    public void FadeOutOnly(float duration = -1f)
    {
        if (fadeImage == null)
        {
            return;
        }

        StartCoroutine(Fade(0f, 1f, duration < 0f ? fadeDuration : duration));
    }

    public void FadeInOnly(float duration = -1f)
    {
        if (fadeImage == null)
        {
            return;
        }

        StartCoroutine(Fade(1f, 0f, duration < 0f ? fadeDuration : duration));
    }
}


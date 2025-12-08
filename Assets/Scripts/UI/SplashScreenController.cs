using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

// shows a splash screen for a set duration before loading the next scene
public class SplashScreenController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float splashDuration = 3f;
    [SerializeField] private string nextSceneName = "MainMenu";

    // kicks off the coroutine that advances to the next scene
    void Start()
    {
        StartCoroutine(LoadNextScene());
    }

    // waits for the configured delay and then loads the next scene
    IEnumerator LoadNextScene()
    {
        yield return new WaitForSeconds(splashDuration);

        if (SceneTransitionManager.Instance != null)
        {
            SceneTransitionManager.Instance.LoadScene(nextSceneName);
        }
        else
        {
            SceneManager.LoadScene(nextSceneName);
        }
    }
}


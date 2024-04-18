using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class UISceneTransition : MonoBehaviour
{
    public float transitionDuration = 1.0f; // Duration of the fade transition
    public Image transitionImage; // Reference to the UI Image used for transition

    private CanvasGroup canvasGroup;

    private void Start()
    {
        // Get the CanvasGroup component attached to the transitionImage
        canvasGroup = transitionImage.GetComponent<CanvasGroup>();

        // Ensure the transition image is fully transparent at the start
        canvasGroup.alpha = 0f;
    }

    public void StartTransition(string sceneName)
    {
        // Start the transition coroutine
        StartCoroutine(Transition(sceneName));
        DontDestroyOnLoad(gameObject);
    }

    private IEnumerator Transition(string sceneName)
    {
        // Fade to black
        while (canvasGroup.alpha < 1)
        {
            Time.timeScale = 1;
            canvasGroup.alpha += Time.deltaTime / transitionDuration;
            yield return null;
        }

        // Ensure the transition image is fully opaque
        canvasGroup.alpha = 1f;

        // Load the target scene asynchronously
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        // Fade back out
        while (canvasGroup.alpha > 0)
        {
            canvasGroup.alpha -= Time.deltaTime / transitionDuration;
            yield return null;
        }

        // Ensure the transition image is fully transparent
        canvasGroup.alpha = 0f;
        StartCoroutine(DestroyObject());
    }

    private IEnumerator DestroyObject()
    {
        // Wait for a short delay before destroying the object
        yield return new WaitForSeconds(0.1f); // Adjust the delay time as needed

        // Destroy this object
        Destroy(gameObject);
    }
}

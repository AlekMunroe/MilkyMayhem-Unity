using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class RestartTrigger : MonoBehaviour
{
    [SerializeField] private TMP_Text countdownText;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(RestartCountdown());
        }
    }

    private IEnumerator RestartCountdown()
    {
        countdownText.gameObject.SetActive(true);

        float timeRemaining = 5;

        while(timeRemaining > 0f)
        {
            countdownText.text = "Out of bounds: Restarting in " + timeRemaining.ToString("0");

            yield return null;

            timeRemaining -= Time.unscaledDeltaTime;
        }

        countdownText.text = "Restarting...";

        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.buildIndex);
    }
}

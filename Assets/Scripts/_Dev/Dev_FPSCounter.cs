using UnityEngine;
using TMPro;

public class Dev_FPSCounter : MonoBehaviour
{
    [SerializeField] private TMP_Text fpsText;
    [SerializeField] private float updateInterval = 0.5f;

    private float elapsedTime;
    private int frameCount;

    private void Update()
    {
        elapsedTime += Time.unscaledDeltaTime;
        frameCount++;

        if(elapsedTime < updateInterval)
        {
            return;
        }

        float currentFPS = frameCount / elapsedTime;

        fpsText.text = "FPS: " + Mathf.RoundToInt(currentFPS);

        elapsedTime = 0f;
        frameCount = 0;
    }
}

using UnityEngine;
using UnityEngine.UI;

public class PlayerUIController : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;

    [Header("UI Elements")] 
    [SerializeField] private Slider sprintSlider;

    void Awake()
    {
        if (sprintSlider == null)
        {
            Debug.LogError("PlayerUIController: Sprint Slider is not assigned");

            enabled = false;
            return;
        }

        if (playerController == null)
        {
            Debug.LogError("PlayerUIController: Player Controller is not assigned");

            enabled = false;
            return;
        }

        sprintSlider.interactable = false;
    }
    void Update()
    {
        UpdateSprintSlider();
    }

    private void UpdateSprintSlider()
    {
        float sprintSliderValue = playerController.SprintStaminaPercent;
        
        sprintSlider.value = sprintSliderValue;
    }
}

using UnityEngine;
using UnityEngine.UI;

public class PlayerUIController : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;

    [Header("UI Elements")] 
    [SerializeField] private Slider sprintSlider;

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

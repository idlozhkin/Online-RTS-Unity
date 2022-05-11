using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Slider slider;

    public void SetSliderValue(float hp)
    {
        slider.value = hp;
    }

    public void SetSliderMaxValue(float hp)
    {
        slider.maxValue = hp;
        slider.value = hp;
    }
}

using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Controll : MonoBehaviour
{
    public Slider healthSlider; // Ссылка на UI Slider
    public TMP_Text healthText;

    void Start()
    {
        // Проверка на null перед использованием
        if (PlayerController.Instance == null)
        {
            Debug.LogError("PlayerController.Instance не найден!");
            return;
        }

        // Инициализация UI
        healthSlider.maxValue = PlayerController.Instance.maxHealth;
        healthSlider.value = PlayerController.Instance.currentHealth;
        healthText.text = $"HP: {PlayerController.Instance.currentHealth}";

        // Подписка на событие изменения здоровья
        PlayerController.OnHealthChanged += UpdateHealth;
    }

    void UpdateHealth(int health)
    {
        healthSlider.value = health;
        healthText.text = $"HP: {health}";
    }

    void OnDestroy()
    {
        PlayerController.OnHealthChanged -= UpdateHealth;
    }
}

using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Controll : MonoBehaviour
{
    public Slider healthSlider; // ������ �� UI Slider
    public TMP_Text healthText;

    void Start()
    {
        // �������� �� null ����� ��������������
        if (PlayerController.Instance == null)
        {
            Debug.LogError("PlayerController.Instance �� ������!");
            return;
        }

        // ������������� UI
        healthSlider.maxValue = PlayerController.Instance.maxHealth;
        healthSlider.value = PlayerController.Instance.currentHealth;
        healthText.text = $"HP: {PlayerController.Instance.currentHealth}";

        // �������� �� ������� ��������� ��������
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

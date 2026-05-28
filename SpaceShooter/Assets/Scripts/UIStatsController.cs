using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIStatsController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private Slider healthBar;
    [SerializeField] private Image powerUpImage;
    void Start()
    {
        GameManager.Instance.SpaceShip.OnHealthChanged += OnHealthChanged;
        GameManager.Instance.SpaceShip.OnPowerUpChanged += OnPowerUpChanged;
        GameManager.Instance.SpaceShip.OnScoreChanged += OnScoreChanged;

        if (healthBar != null) 
        {
            healthBar.value = 1;
        }
        if (powerUpImage != null) 
        {
            powerUpImage.sprite = null;
            powerUpImage.gameObject.SetActive(false);
        }
        if (scoreText != null)
        {
            scoreText.text = $"Score: {0}";
        }
    }

    private void OnPowerUpChanged()
    {
        if(PowerUpManager.Instance.PowerUpConfigDictionary.TryGetValue(GameManager.Instance.SpaceShip.PowerUp,out var powerUpConfig))
        {
            powerUpImage.sprite = powerUpConfig.Icon;
        }
    }

    private void OnHealthChanged()
    {
        if (healthBar != null)
        {
            healthBar.value = GameManager.Instance.SpaceShip.Health / GameManager.Instance.SpaceShip.Maxhealth;
        }
    }
    private void OnScoreChanged()
    {
        if (scoreText != null)
        {
            scoreText.text = $"Score: {GameManager.Instance.SpaceShip.Score}";
        }
    }

    void Update()
    {
        
    }
}

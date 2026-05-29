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
    private void Start()
    {
        GameManager.Instance.OnPlayerSpawned += OnPlayerSpawned;
        Debug.Log("sub to player spawned");
    }

    public void OnPlayerSpawned()
    {
        Debug.Log("player spawned");
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
        if(GameManager.Instance.SpaceShip.PowerUp == null)
        {
            powerUpImage.sprite = null;
            powerUpImage.gameObject.SetActive(false);
            return;
        }

        if (PowerUpManager.Instance.PowerUpConfigDictionary.TryGetValue(GameManager.Instance.SpaceShip.PowerUp.Value, out var powerUpConfig))
        {
            powerUpImage.sprite = powerUpConfig.Icon;
            powerUpImage.gameObject.SetActive(true);
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

}

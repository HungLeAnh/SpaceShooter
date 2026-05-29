using System;

public class SpaceShip
{
    public Action OnHealthChanged;
    public Action OnPowerUpChanged;
    public Action OnScoreChanged;

    private float maxhealth;
    private float health;
    private float speed;
    private float fireRate;
    private PowerUpType? powerUp;
    private int score;
    public float Health => health;
    public PowerUpType? PowerUp => powerUp;
    public int Score => score;
    public float Speed { get => speed; set => speed = value; }
    public float FireRate { get => fireRate; set => fireRate = value; }
    public float Maxhealth { get => maxhealth; set => maxhealth = value; }

    public SpaceShip(int health, float speed)
    {
        this.maxhealth = health;
        this.health = health;
        this.Speed = speed;
        this.score = 0;

    }

    public void SetHealth(float newhealth)
    {
        health = newhealth;
        OnHealthChanged?.Invoke();
    }

    public void SetPowerUp(PowerUpType? powerUpType)
    {
        powerUp = powerUpType;
        OnPowerUpChanged?.Invoke();
    }

    public void UpdateScore(int scoreReward)
    {
        score += scoreReward;
        OnScoreChanged?.Invoke();
    }
}

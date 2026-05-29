using System;
using UnityEngine;
public enum PowerUpType
{
    LaserWeapon,
    MissileWeapon,
    PlasmaWeapon,
}

public class PowerUp : MonoBehaviour
{
    [SerializeField] private PowerUpType powerUpType;
    [SerializeField] private float powerUpTime = 10;
    [SerializeField] private float powerUpSpeed = 0.1f;

    private PowerUpManager powerUpManager;

    public PowerUpType PowerUpType { get => powerUpType; set => powerUpType = value; }

    private void OnTriggerEnter2D(Collider2D other)
    {
        //Debug.Log("powerup trigger");
        if (other.gameObject.TryGetComponent<IPowerUpable>(out var powerUpable))
        {
            //Debug.Log("is powerupable");
            powerUpable.PowerUp(powerUpType,powerUpTime);
            powerUpManager.ReleasePowerUp(this);
        }
        else
        {
            //Debug.Log("not powerupable");
        }
    }

    public void Initialize(PowerUpManager powerUpManager)
    {
        this.powerUpManager = powerUpManager;
    }
    public void SetSpawnPos(Vector3 pos)
    {
        gameObject.transform.position = pos;
    }
    private void Update()
    {
        if (GameManager.Instance.CurrentStateType != GameStateType.Playing)
            return;

        gameObject.transform.position += Vector3.down * powerUpSpeed * Time.deltaTime;

        var screenBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height,0));

        if (gameObject.transform.position.y < -screenBounds.y)
        {
            ReturnToPool();
        }
    }
    public void ReturnToPool()
    {
        if (powerUpManager != null && gameObject.activeSelf)
        {
            powerUpManager.ReleasePowerUp(this);
        }
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Properties;
using UnityEngine;
using UnityEngine.InputSystem;

public enum WeaponType
{
    StandardWeapon,
    LaserWeapon,
    PlasmaWeapon,
    MissileWeapon,
}

[Serializable]
public struct WeaponConfig
{
    public WeaponType WeaponType;
    public GameObject WeaponPrefab;
}
public interface IDamageable
{
    void Damage(float damage);
}
public interface IPowerUpable
{
    void PowerUp(PowerUpType powerUpType,float powerUpTime);
}
public class SpaceshipController : MonoBehaviour,IDamageable,IPowerUpable
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private SpriteRenderer spaceshipRenderer;
    [SerializeField] private SpriteRenderer engineRenderer;
    [SerializeField] private SpriteRenderer engineEffect;
    [SerializeField] private Transform spaceshipWeaponParent;
    [SerializeField] private List<WeaponConfig> weaponList;
    [SerializeField] private SpaceshipWeapon defaultWeapon;

    private Dictionary<WeaponType, WeaponConfig> weaponsDictionary;

    private PlayerInputAction actions;
    private InputAction moveAction;

    private Vector2 screenBounds;
    private float objectWidth;
    private float objectHeight;
    private bool isMoving = false;
    private float currentHealth;
    private float shieldTime;
    private float weaponTime;

    private SpaceshipWeapon currentShield;
    private SpaceshipWeapon currentWeapon;

    private void Awake()
    {
        actions = new PlayerInputAction();
        actions.Player.Enable();
    }
    private void Start()
    {
        currentHealth = GameManager.Instance.SpaceShip.Maxhealth;
        weaponsDictionary = new Dictionary<WeaponType, WeaponConfig>();
        foreach (var weapon in weaponList)
        {
            weaponsDictionary[weapon.WeaponType] = weapon;
        }

        screenBounds = mainCamera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, mainCamera.transform.position.z));
        objectWidth = spaceshipRenderer.bounds.size.x / 2;
        objectHeight = spaceshipRenderer.bounds.size.y / 2;

        moveAction = actions.Player.Move;
        actions.Player.Move.performed += OnPlayerMovePerformed;
        actions.Player.Move.canceled += OnPlayerMoveCanceled;
    }

    private void OnPlayerMoveCanceled(InputAction.CallbackContext context)
    {
        isMoving = false;
    }

    private void OnPlayerMovePerformed(InputAction.CallbackContext context)
    {
        isMoving = true;
    }

    private void Update()
    {
        if (GameManager.Instance.CurrentStateType == GameStateType.Playing)
        {
            if (isMoving)
            {
                Vector2 input = GetInputVectorNormalized();
                transform.position += new Vector3(input.x, input.y, 0) * Time.deltaTime * GameManager.Instance.SpaceShip.Speed;

                float clampedX = Mathf.Clamp(transform.position.x, -screenBounds.x + objectWidth, screenBounds.x - objectWidth);
                float clampedY = Mathf.Clamp(transform.position.y, -screenBounds.y + objectHeight, screenBounds.y - objectHeight);
                transform.position = new Vector3(clampedX, clampedY, transform.position.z);

            }
            if (currentWeapon != defaultWeapon) 
            {   
                weaponTime -= Time.deltaTime;
                if (weaponTime < 0)
                {
                    if(currentWeapon != null)
                        Destroy(currentWeapon.gameObject);

                    currentWeapon = defaultWeapon;
                    currentWeapon.gameObject.SetActive(true);
                }
            }
            currentWeapon.FireActiveWeapon();
        }
    }
    private void LateUpdate()
    {

    }
    public Vector2 GetInputVectorNormalized()
    {
        if (moveAction != null)
        {
            Vector2 input = moveAction.ReadValue<Vector2>();
            return input.normalized;
        }
        return Vector2.zero;
    }

    public void PowerUp(PowerUpType powerUpType,float powerUpTime)
    {
        GameManager.Instance.SpaceShip.SetPowerUp(powerUpType);

        switch (powerUpType)
        {
            case PowerUpType.LaserWeapon:
                SetWeapon(WeaponType.LaserWeapon,powerUpTime);
                break;
            case PowerUpType.MissileWeapon:
                SetWeapon(WeaponType.MissileWeapon,powerUpTime);
                break;
            case PowerUpType.PlasmaWeapon:
                SetWeapon(WeaponType.PlasmaWeapon, powerUpTime);
                break;
        }
    }
    private void SetWeapon(WeaponType type,float powerUpTime)
    {
        weaponTime = powerUpTime;
        var newWeapon = Instantiate(weaponsDictionary[type].WeaponPrefab, spaceshipWeaponParent);
        if (currentWeapon != defaultWeapon)
            Destroy(currentWeapon.gameObject);
        else
            currentWeapon.gameObject.SetActive(false);
        currentWeapon = newWeapon.GetComponent<SpaceshipWeapon>();        
        //Debug.Log("powerUp");
    }

    public void Damage(float damage)
    {
        currentHealth -= damage;
        GameManager.Instance.SpaceShip.SetHealth(currentHealth);
    }
}

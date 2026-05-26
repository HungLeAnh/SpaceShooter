using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Properties;
using UnityEngine;
using UnityEngine.InputSystem;

public class SpaceshipController : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private SpriteRenderer spaceshipRenderer;
    [SerializeField] private SpriteRenderer engineRenderer;
    [SerializeField] private SpriteRenderer engineEffect;
    [SerializeField] private Transform spaceshipWeaponParent;
    [SerializeField] private List<GameObject> weaponList;
    

    private PlayerInputAction actions;
    private InputAction moveAction;

    private Vector2 screenBounds;
    private float objectWidth;
    private float objectHeight;
    private bool isMoving = false;
    private SpaceshipWeapon currentWeapon;

    private void Awake()
    {
        actions = new PlayerInputAction();
        actions.Player.Enable();
    }
    private void Start()
    {
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

}

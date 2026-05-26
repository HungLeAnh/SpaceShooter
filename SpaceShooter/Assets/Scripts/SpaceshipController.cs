using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class  SpaceShip
{
    private int health;
    private float speed;
    public int Health { get => health; set => health = value; }
    public float Speed { get => speed; set => speed = value; }
    public SpaceShip(int health, float speed)
    {
        this.Health = health;
        this.Speed = speed;
    }


}

public class SpaceshipController : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private int health = 3;

    [SerializeField] private Camera mainCamera;
    [SerializeField] private SpriteRenderer spaceshipRenderer;
    [SerializeField] private SpriteRenderer engineRenderer;
    [SerializeField] private SpriteRenderer engineEffect;

    private PlayerInputAction actions;
    private InputAction moveAction;

    private Vector2 screenBounds;
    private float objectWidth;
    private float objectHeight;
    private bool isMoving = false;

    private SpaceShip spaceShip;
    private void Awake()
    {
        actions = new PlayerInputAction();
        actions.Player.Enable();
        spaceShip = new SpaceShip(health, speed);
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
        if (isMoving) 
        {
            Vector2 input = GetInputVectorNormalized();
            transform.position += new Vector3(input.x, input.y, 0) * Time.deltaTime * spaceShip.Speed;

            float clampedX = Mathf.Clamp(transform.position.x, -screenBounds.x + objectWidth, screenBounds.x - objectWidth);
            float clampedY = Mathf.Clamp(transform.position.y, -screenBounds.y + objectHeight, screenBounds.y - objectHeight);
            transform.position = new Vector3(clampedX, clampedY, transform.position.z);
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

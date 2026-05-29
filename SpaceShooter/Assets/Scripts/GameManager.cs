using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameStateType
{
    MainMenu,
    Playing,
    GameOver,
    VicTory,
}
[DefaultExecutionOrder(-1)]
public class GameManager : MonoBehaviour
{
    public Action OnPlayerSpawned;
    public Action OnStateChanged;
    
    public static GameManager Instance;

    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private float speed = 5f;
    [SerializeField] private int health = 100;
    [SerializeField] private Vector3 spawnPos = Vector3.zero;

    private GameStateManager gameStateManager;
    private GameStateType currentStateType;
    private SpaceShip spaceShip;
    private GameObject playerGameObject;


    public GameStateType CurrentStateType { get => currentStateType; set => currentStateType = value; }
    public SpaceShip SpaceShip { get => spaceShip; set => spaceShip = value; }

    private void Awake()
    {
        if(Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void OnHealthChanged()
    {
        if (spaceShip.Health < 0)
        {
            ChangeState(GameStateType.GameOver);
        }
    }

    private void Start()
    {
        gameStateManager = new GameStateManager();
        ChangeState(GameStateType.MainMenu);
    }
    private void Update()
    {
        gameStateManager.Update();
    }
    public void ChangeState(GameStateType stateType)
    {
        switch(stateType)
        {
            case GameStateType.MainMenu:
                gameStateManager.ChangeState(new MainMenuState());
                currentStateType = GameStateType.MainMenu;
                break;
            case GameStateType.Playing:
                gameStateManager.ChangeState(new PlayingState());
                currentStateType = GameStateType.Playing;
                break;
            case GameStateType.GameOver:
                gameStateManager.ChangeState(new GameOverState());
                currentStateType = GameStateType.GameOver;
                break;
            case GameStateType.VicTory:
                gameStateManager.ChangeState(new GameVictoryState());
                currentStateType = GameStateType.VicTory;
                break;
        }
        OnStateChanged?.Invoke();
    }
    public void OnPlayTap()
    {
        ChangeState(GameStateType.Playing);
    }
    public void InstantiatePlayer()
    {
        spaceShip = new SpaceShip(health, speed);
        spaceShip.OnHealthChanged += OnHealthChanged;
        playerGameObject = Instantiate(playerPrefab);
        playerGameObject.transform.position = spawnPos;
        OnPlayerSpawned?.Invoke();
        Debug.Log($"Invoke player spawn event {OnPlayerSpawned.GetInvocationList().Length}");
    }

    public void DestroyPlayer()
    {
        Destroy(playerGameObject);
    }
}
public class GameStateManager
{
    private GameState currentState;
    public GameState CurrentState { get => currentState; set => currentState = value; }
    public GameStateManager()
    {
        currentState = null;
    }
    public void ChangeState(GameState newState)
    {
        if (currentState != null)
            currentState.Exit();

        currentState = newState;
        currentState.Enter();
    }
    public void Update()
    {
        currentState.Update();
    }
}
public abstract class GameState
{
    public abstract void Enter();
    public abstract void Exit();
    public abstract void Update();
}
public class MainMenuState : GameState
{
    public override void Enter()
    {
        HUDController.Instance.ShowElement(HUDStateType.Menu);
    }

    public override void Exit()
    {
        HUDController.Instance.HideElement(HUDStateType.Menu);
    }
    public override void Update()
    {

    }
}

public class PlayingState : GameState
{
    public override void Enter()
    {
        PowerUpManager.Instance.ReturnAllToPool();
        GameManager.Instance.InstantiatePlayer();
        EnemyManager.Instance.StartSpawningSequence();
        HUDController.Instance.ShowElement(HUDStateType.Stats);
    }
    public override void Exit()
    {
    }
    public override void Update()
    {
        
    }
}
public class GameOverState : GameState
{
    public override void Enter()
    {
        HUDController.Instance.ShowElement(HUDStateType.GameOver);
        GameManager.Instance.DestroyPlayer();
    }
    public override void Exit()
    {
        HUDController.Instance.HideElement(HUDStateType.GameOver);
        EnemyManager.Instance.DestroyAllEnenmy();
        MultiBulletPoolManager.Instance.ReturnAllBulletToPool();
    }
    public override void Update()
    {
        
    }
}
public class GameVictoryState : GameState
{
    public override void Enter()
    {
        HUDController.Instance.ShowElement(HUDStateType.Victory);
        EnemyManager.Instance.DestroyAllEnenmy();
    }
    public override void Exit()
    {
        HUDController.Instance.HideElement(HUDStateType.Victory);
        GameManager.Instance.DestroyPlayer();

    }
    public override void Update()
    {
        
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameStateType
{
    MainMenu,
    Playing,
    GameOver
}

public class GameManager : MonoBehaviour
{
    public Action OnStateChanged;
    public static GameManager Instance;

    [SerializeField] private float speed = 5f;
    [SerializeField] private int health = 3;

    private GameStateManager gameStateManager;
    private GameStateType currentStateType;
    private SpaceShip spaceShip;


    public GameStateType CurrentStateType { get => currentStateType; set => currentStateType = value; }
    public SpaceShip SpaceShip { get => spaceShip; set => spaceShip = value; }

    private void Awake()
    {
        if(Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
        spaceShip = new SpaceShip(health, speed);

    }

    private void Start()
    {
        gameStateManager = new GameStateManager();
        ChangeState(GameStateType.MainMenu);
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
        }
        OnStateChanged?.Invoke();
    }
    public void OnPlayTap()
    {
        ChangeState(GameStateType.Playing);
    }
}
public class GameStateManager
{
    private GameState currentState;
    public GameState CurrentState { get => currentState; set => currentState = value; }
    public GameStateManager()
    {
        currentState = new GameState();
    }
    public void ChangeState(GameState newState)
    {
        if (currentState != null)
            currentState.Exit();

        currentState = newState;
        currentState.Enter();
    }
}
public class GameState
{
    public virtual void Enter()
    {
        // Code to execute when entering the state
    }
    public virtual void Exit()
    {
        // Code to execute when exiting the state
    }

}
public class MainMenuState : GameState
{
    public override void Enter()
    {
        base.Enter();
        HUDController.Instance.ShowElement(HUDStateType.Menu);
    }

    public override void Exit()
    {
        base.Exit();
        HUDController.Instance.HideElement(HUDStateType.Menu);
    }
}

public class PlayingState : GameState
{
    public override void Enter()
    {
        base.Enter();
        HUDController.Instance.ShowElement(HUDStateType.Stats);
    }
    public override void Exit()
    {
        base.Exit();
        HUDController.Instance.HideElement(HUDStateType.Stats);
    }
}
public class GameOverState : GameState
{
    public override void Enter()
    {
        base.Enter();
        HUDController.Instance.ShowElement(HUDStateType.GameOver);
    }
    public override void Exit()
    {
        base.Exit();
        HUDController.Instance.HideElement(HUDStateType.GameOver);
    }

}
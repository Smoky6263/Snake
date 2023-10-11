using Colyseus;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MultiplayerManager : ColyseusManager<MultiplayerManager>
{
    #region Server
    private const string GameRoomName = "state_handler";

    private ColyseusRoom<State> _room;
    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
        InitializeClient();
        Connection();
    }

    private async void Connection()
    {
        _room = await client.JoinOrCreate<State>(GameRoomName);
        _room.OnStateChange += Onchange;
    }

    private void Onchange(State state, bool isFirstState)
    {
        if (isFirstState == false) return;
        _room.OnStateChange -= Onchange;

        state.players.ForEach((key, player) =>
        {
            if (key == _room.SessionId) CreatePlayer(player);
            else CreateEnemy(key, player);
        });

        _room.State.players.OnAdd += CreateEnemy;
        _room.State.players.OnRemove += RemoveEnemy;

        _room.State.apples.ForEach(CreateApple);

        _room.State.apples.OnAdd += (key, value) => CreateApple(value);
        _room.State.apples.OnRemove += RemoveApple;
    }


    protected override void OnApplicationQuit()
    {
        base.OnApplicationQuit();
        LeaveRoom();
    }

    public void LeaveRoom()
    {
        _room?.Leave();
    }

    public void SendMessage(string key, Dictionary<string, object> data)
    {
        _room.Send(key, data);
    }
    #endregion

    #region Player
    [SerializeField] private PlayerAim _playerAim;
    [SerializeField] private Controller _controllerPrefab;
    [SerializeField] private Snake _snakePrefab;

    private void CreatePlayer(Player player)
    {
        Vector3 position = new Vector3(player.x, 0, player.z);
        Quaternion rotation = Quaternion.identity;

        Snake snake = Instantiate(_snakePrefab, position, rotation);
        snake.Init(player.d);

        PlayerAim aim = Instantiate(_playerAim, position, rotation);
        aim.Init(snake._head, snake.Speed);

        Controller controller = Instantiate(_controllerPrefab);
        controller.Init(aim, player, snake);
    }
    #endregion

    #region Enemy
    private Dictionary<string, EnemyController> _enemies = new Dictionary<string, EnemyController>();
    private void CreateEnemy(string key, Player player)
    {
        Vector3 position = new Vector3(player.x, 0, player.z);

        Snake snake = Instantiate(_snakePrefab, position, Quaternion.identity);
        snake.Init(player.d);

        EnemyController enemy = snake.AddComponent<EnemyController>();
        enemy.Init(player, snake);

        _enemies.Add(key, enemy);
    }
    private void RemoveEnemy(string key, Player value)
    {
        if(_enemies.ContainsKey(key) == false)
        {
            Debug.Log("ѕопытка уничтожени€ enemy, которого нет в словаре");
            return;
        }
        EnemyController enemy = _enemies[key];
        _enemies.Remove(key);
        enemy.Destroy();
    }
    #endregion

    #region Apple
    [SerializeField] private Apple _applePrefab;
    private Dictionary<Vector2Float, Apple> _apples = new Dictionary<Vector2Float, Apple>();
    private void CreateApple(Vector2Float vector2Float)
    {
        Vector3 position = new Vector3(vector2Float.x, 0, vector2Float.z);

        Apple apple = Instantiate(_applePrefab, position, Quaternion.identity);
        apple.Init(vector2Float);
        _apples.Add(vector2Float, apple);
    }

    private void RemoveApple(int key, Vector2Float vector2Float)
    {
        if (_apples.ContainsKey(vector2Float) == false) return;

        Apple apple = _apples[vector2Float];
        _apples.Remove(vector2Float);
        apple.Destroy();
    }
    #endregion
}

using Colyseus.Schema;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Controller : MonoBehaviour
{
    [SerializeField] private Transform _cursor;
    [SerializeField] private float _cameraOffsetY = 15f;
    private MultiplayerManager _multiplayerManager;
    private Player _player;
    private PlayerAim _playerAim;
    private Snake _snake;
    private Camera _camera;
    private string _clientID;

    private Plane _plane;
    public void Init(string clientID, PlayerAim aim, Player player, Snake snake)
    {
        _multiplayerManager = MultiplayerManager.Instance;

        _clientID = clientID;
        _playerAim = aim;
        _player = player;
        _snake = snake;
        _camera = Camera.main;
        _plane = new Plane(Vector3.up, Vector3.zero);

        _camera.transform.parent = _snake.transform;
        _camera.transform.localPosition = Vector3.up * _cameraOffsetY;

        _player.OnChange += OnChange;
    }

    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            MoveCursos();
            _playerAim.SetTargetDirection(_cursor.position);
        }

        SendMove();
    }

    private void SendMove()
    {
        _playerAim.GetMoveInfo(out Vector3 position);

        Dictionary<string, object> data = new Dictionary<string, object>() 
        {
            { "x", position.x },
            { "z", position.z },
        };

        _multiplayerManager.SendMessage("move", data);
    }
    private void OnChange(List<DataChange> changes)
    {
        Vector3 position = _snake.transform.position;

        for (int i = 0; i < changes.Count; i++)
        {
            switch (changes[i].Field)
            {
                case "x":
                    position.x = (float)changes[i].Value;
                    break;

                case "z":
                    position.z = (float)changes[i].Value;
                    break;

                case "d":
                    _snake.SetDetailCount((byte)changes[i].Value);
                    break;

                case "score":
                    _multiplayerManager.UpdateScore(_clientID, (ushort)changes[i].Value);
                    break;

                default:
                    Debug.Log("Не обрабатывается изменение поля:" + changes[i].Field);
                    break;
            }
        }

        _snake.SetRotation(position);
    }

    public void Destroy()
    {
        CameraScript cameraScript = _camera.GetComponent<CameraScript>();
        cameraScript.ChangeCamera();
        _camera.transform.parent = null;
        _player.OnChange -= OnChange;

        _snake.Destroy(_clientID);

        Destroy(gameObject);
    }
    private void MoveCursos()
    {
        Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
        _plane.Raycast(ray, out float distance);
        Vector3 point = ray.GetPoint(distance);

        _cursor.position = point;
    }
}

using Colyseus.Schema;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    private Player _player;
    private Snake _snake;
    private string _clientID;

    public void Init(string clientID, Player player, Snake snake)
    {
        _clientID = clientID;
        _player = player;
        _snake = snake;
        player.OnChange += OnChange;
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
                    MultiplayerManager.Instance.UpdateScore(_clientID, (ushort)changes[i].Value);
                    break;

                default: Debug.Log("Не обрабатывается изменение поля:" + changes[i].Field);
                    break;
            }
        }

        _snake.SetRotation(position);
    }

    public void Destroy()
    {
        _player.OnChange -= OnChange;
        _snake?.Destroy(_clientID);
    }
}

using Colyseus.Schema;
using System.Collections.Generic;
using UnityEngine;

public class Apple : MonoBehaviour
{

    private Vector2Float _apple;
    public void Init(Vector2Float apple)
    {
        _apple = apple;

        _apple.OnChange += OnChange;
    }

    private void OnChange(List<DataChange> changes)
    {
        Vector3 position = transform.position;

        foreach (var change in changes)
        {
            switch (change.Field)
            {
                case "x":
                    position.x = (float)change.Value;
                    break;
                case "z":
                    position.z = (float)change.Value;
                    break;
                default:
                    Debug.Log("яблоко не реагирует на изменение пол€" + change.Field);
                    break;
            }
        }
        transform.position = position;
        gameObject.SetActive(true);        
    }

    public void Destroy()
    {
        if(_apple != null) _apple.OnChange -= OnChange;
        Destroy(gameObject);
    }

    public void Collect()
    {
        gameObject.SetActive(false);

        Dictionary<string, object> data = new Dictionary<string, object> 
        {
            { "id", _apple.id }
        };

        MultiplayerManager.Instance.SendMessage("collect", data);
    }
}

using UnityEngine;

public class Snake : MonoBehaviour
{
    public float Speed { get { return _speed; } }

    [SerializeField] private Tail _tailPrefab;
    [field: SerializeField] public Transform _head { get; private set; }
    [SerializeField] private float _speed = 2f;

    private Tail _tail;

    public void Init(int detailCount)
    {
        _tail = Instantiate(_tailPrefab, transform.position, Quaternion.identity);
        _tail.Init(_head, _speed, detailCount);
    }

    public void SetDetailCount(int detailCount) 
    {
        _tail.SetDetailCount(detailCount);
    }

    private void Update()
    {
        Move();
    }

    private void Move()
    {
        transform.position += _head.forward * Time.deltaTime * _speed;
    }

    public void SetRotation(Vector3 pointToLook) 
    {
        _head.LookAt(pointToLook);
    }

    public void Destroy()
    {
        _tail.Destroy();
        Destroy(gameObject);
    }
}

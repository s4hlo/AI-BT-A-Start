using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    private int _healthPoints;
    [SerializeField]
    private float _speed = 30f;

    private void Awake()
    {
        _healthPoints = 30;
    }

    public bool TakeHit()
    {
        _healthPoints -= 10;
        bool isDead = _healthPoints <= 0;
        if (isDead) _Die();
        return isDead;
    }

    private void _Die()
    {
        Destroy(gameObject);
    }

    private void RestoreLife()
    {
        _healthPoints = 30;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            RestoreLife();
        }

        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        transform.Translate(new Vector3(-verticalInput, 0f, horizontalInput) * _speed * Time.deltaTime, Space.Self);


    }
}

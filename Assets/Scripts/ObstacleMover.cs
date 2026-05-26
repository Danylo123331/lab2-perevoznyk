using UnityEngine;

public class ObstacleMover : MonoBehaviour
{
    public float speed = 5f;
    public bool isMovingUpDown = false;
    public float verticalSpeed = 2f;
    public float verticalRange = 1.5f;

    private float startY;

    void Start()
    {
        startY = transform.position.y;
    }

    void Update()
    {
        transform.Translate(Vector3.left * speed * Time.deltaTime);

        if (isMovingUpDown)
        {
            float newY = startY + Mathf.Sin(Time.time * verticalSpeed) * verticalRange;
            transform.position = new Vector3(transform.position.x, newY, transform.position.z);
        }

        if (transform.position.x < -12f)
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.AddScore();
            }
            Destroy(gameObject);
        }
    }
}
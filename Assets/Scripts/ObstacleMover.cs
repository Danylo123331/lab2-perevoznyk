using UnityEngine;
using System.Reflection;

public class ObstacleMover : MonoBehaviour
{
    public float speed = 5f;
    public bool isMovingUpDown = false;
    public float verticalSpeed = 2f;
    public float verticalRange = 1.5f;
    public AudioClip customHitSound;

    private float startY;
    private bool scoreAdded = false;
    private Transform playerTransform;

    void Start()
    {
        startY = transform.position.y;
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
    }

    void Update()
    {
        float currentSpeed = speed;
        if (GameManager.instance != null)
        {
            currentSpeed = speed * GameManager.instance.gameSpeedMultiplier;
        }

        transform.Translate(Vector3.left * currentSpeed * Time.deltaTime, Space.World);

        if (isMovingUpDown)
        {
            float currentVerticalSpeed = verticalSpeed;
            if (GameManager.instance != null)
            {
                currentVerticalSpeed = verticalSpeed * GameManager.instance.gameSpeedMultiplier;
            }
            float newY = startY + Mathf.PingPong(Time.time * currentVerticalSpeed, verticalRange * 2) - verticalRange;
            transform.position = new Vector3(transform.position.x, newY, transform.position.z);
        }

        float playerX = playerTransform != null ? playerTransform.position.x : -5f;
        if (!scoreAdded && transform.position.x < playerX)
        {
            scoreAdded = true;
            TriggerScoreAddition();
        }

        if (transform.position.x < -15f)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && customHitSound != null)
        {
            AudioSource.PlayClipAtPoint(customHitSound, Camera.main.transform.position);
        }
    }

    private void TriggerScoreAddition()
    {
        GameObject gm = GameObject.Find("GameManager");
        if (gm == null) return;

        MonoBehaviour[] scripts = gm.GetComponents<MonoBehaviour>();
        foreach (var script in scripts)
        {
            var method = script.GetType().GetMethod("AddScore", BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);
            if (method != null)
            {
                method.Invoke(script, null);
                return;
            }

            var field = script.GetType().GetField("score", BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);
            if (field != null)
            {
                int currentScore = (int)field.GetValue(script);
                field.SetValue(script, currentScore + 1);
                return;
            }
        }
    }
}
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float flyForce = 5f;
    public float maxEnergy = 100f;
    public float heatingSpeed = 40f;
    public float coolingSpeed = 20f;

    public AudioClip jetpackSound;
    public AudioClip overheatAlarmSound;

    private Rigidbody2D rb;
    private AudioSource jetpackAudio;
    private AudioSource alarmAudio;

    private float currentEnergy;
    private bool isOverheated = false;
    private bool isThrusting = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentEnergy = maxEnergy;

        jetpackAudio = gameObject.AddComponent<AudioSource>();
        jetpackAudio.clip = jetpackSound;
        jetpackAudio.loop = true;
        jetpackAudio.playOnAwake = false;

        alarmAudio = gameObject.AddComponent<AudioSource>();
        alarmAudio.clip = overheatAlarmSound;
        alarmAudio.loop = true;
        alarmAudio.playOnAwake = false;
    }

    void Update()
    {
        bool isFlyingInput = Input.GetKey(KeyCode.Space) || Input.GetMouseButton(0);

        if (isOverheated)
        {
            currentEnergy += coolingSpeed * Time.deltaTime;
            if (currentEnergy >= maxEnergy)
            {
                currentEnergy = maxEnergy;
                isOverheated = false;
            }
        }
        else
        {
            if (isFlyingInput)
            {
                currentEnergy -= heatingSpeed * Time.deltaTime;
                if (currentEnergy <= 0)
                {
                    currentEnergy = 0;
                    isOverheated = true;
                }
            }
            else
            {
                currentEnergy += coolingSpeed * Time.deltaTime;
                if (currentEnergy > maxEnergy) currentEnergy = maxEnergy;
            }
        }

        isThrusting = isFlyingInput && !isOverheated;

        if (isThrusting)
        {
            if (!jetpackAudio.isPlaying) jetpackAudio.Play();
        }
        else
        {
            if (jetpackAudio.isPlaying) jetpackAudio.Stop();
        }

        if (isOverheated)
        {
            if (!alarmAudio.isPlaying) alarmAudio.Play();
        }
        else
        {
            if (alarmAudio.isPlaying) alarmAudio.Stop();
        }

        if (GameManager.instance != null)
        {
            GameManager.instance.UpdateEnergyUI(currentEnergy, maxEnergy, isOverheated);

            if (GameManager.instance.energySlider != null)
            {
                Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position + Vector3.down * 0.8f);
                GameManager.instance.energySlider.transform.position = screenPos;
            }
        }
    }

    void FixedUpdate()
    {
        if (isThrusting)
        {
            rb.AddForce(Vector2.up * flyForce, ForceMode2D.Force);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Obstacle"))
        {
            if (GameManager.instance != null) GameManager.instance.GameOver();
        }
        else if (collision.CompareTag("Coin"))
        {
            if (GameManager.instance != null) GameManager.instance.AddCoin();
            Destroy(collision.gameObject);
        }
    }
}
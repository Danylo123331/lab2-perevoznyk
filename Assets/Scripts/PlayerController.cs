using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Fly Settings")]
    public float flyForce = 35f;
    private Rigidbody2D rb;

    [Header("Energy & Overheat")]
    public float maxEnergy = 100f;
    public float energyDrain = 60f;
    public float energyRecovery = 40f;

    private float currentEnergy;
    private bool isOverheated = false;
    private bool isThrusting = false;

    [Header("UI Follow Settings")]
    public Vector3 uiOffset = new Vector3(0, -1.2f, 0);

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentEnergy = maxEnergy;
    }

    void Update()
    {
        if ((Input.GetKey(KeyCode.Space) || Input.GetMouseButton(0)) && !isOverheated)
        {
            isThrusting = true;
        }
        else
        {
            isThrusting = false;
        }

        if (isThrusting)
        {
            currentEnergy -= energyDrain * Time.deltaTime;
            if (currentEnergy <= 0)
            {
                currentEnergy = 0;
                isOverheated = true;
            }
        }
        else
        {
            currentEnergy += energyRecovery * Time.deltaTime;

            if (currentEnergy >= maxEnergy)
            {
                currentEnergy = maxEnergy;
                isOverheated = false;
            }
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.UpdateEnergyUI(currentEnergy, maxEnergy, isOverheated);

            if (GameManager.Instance.energySlider != null)
            {
                Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position + uiOffset);
                GameManager.Instance.energySlider.transform.position = screenPos;
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
            if (GameManager.Instance != null) GameManager.Instance.GameOver();
        }
    }
}
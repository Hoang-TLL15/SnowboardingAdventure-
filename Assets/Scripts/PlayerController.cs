using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float torqueAmount = 1f;
    [SerializeField] SurfaceEffector2D surfaceEffector;
    [SerializeField] float maxSpeed = 15f;
    [SerializeField] float minSpeed = -5f;
    [SerializeField] float speedStep = 1f;
    [SerializeField] float downhillSpeed = 5f;
    [SerializeField] float uphillSpeed = -3f;
    [SerializeField] float flatSpeed = 0f;
    [SerializeField] float slopeThreshold = 10f;
    [SerializeField] float jumpForce = 8f;
    [SerializeField] float boostSpeed = 30f;
    [SerializeField] float boostDuration = 0.5f;
    [SerializeField] float boostCooldown = 4f;

    Rigidbody2D rb2d;
    Collider2D col2d;

    bool isBoosting = false;
    float boostTimer = 0f;
    float cooldownTimer = 0f;
    bool isPlayerControlling = false;
    bool isOnDownwardSlope = false;
    bool isGrounded = false;

    float accumulatedRotation = 0f;
    float lastZRotation = 0f;

    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        col2d = GetComponent<Collider2D>();
        if (surfaceEffector != null)
        {
            surfaceEffector.speed = 0f;
        }
        lastZRotation = transform.eulerAngles.z;
    }

    void Update()
    {
        HandleInput();
        HandleBoost();
        TrackRotation();
    }

    void HandleInput()
    {
        var keyboard = Keyboard.current;
        if (keyboard == null) return;

        isPlayerControlling = false;

        if (keyboard.leftArrowKey.isPressed)
        {
            rb2d.AddTorque(torqueAmount);
        }
        if (keyboard.rightArrowKey.isPressed)
        {
            rb2d.AddTorque(-torqueAmount);
        }

        if (keyboard.spaceKey.wasPressedThisFrame && isGrounded)
        {
            rb2d.linearVelocity = new Vector2(rb2d.linearVelocity.x, 0f);
            rb2d.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }

        // Boost trigger has priority
        if (keyboard.zKey.wasPressedThisFrame && cooldownTimer <= 0f)
        {
            isBoosting = true;
            boostTimer = boostDuration;
            cooldownTimer = boostCooldown;
            surfaceEffector.speed = boostSpeed;
            return; // Ignore other speed changes this frame
        }

        // Ignore speed changes if boosting
        if (isBoosting) return;

        if (keyboard.upArrowKey.isPressed)
        {
            isPlayerControlling = true;
            surfaceEffector.speed = Mathf.Min(surfaceEffector.speed + speedStep, maxSpeed);
        }
        else if (keyboard.downArrowKey.isPressed)
        {
            if (!isOnDownwardSlope)
            {
                isPlayerControlling = true;
                surfaceEffector.speed = Mathf.Max(surfaceEffector.speed - speedStep, minSpeed);
            }
        }

        if (cooldownTimer > 0f)
        {
            cooldownTimer -= Time.deltaTime;
        }
    }

    void HandleBoost()
    {
        if (isBoosting)
        {
            boostTimer -= Time.deltaTime;
            surfaceEffector.speed = boostSpeed; // Enforce boost speed
            if (boostTimer <= 0f)
            {
                isBoosting = false;
            }
        }

        if (cooldownTimer > 0f && !isBoosting)
        {
            cooldownTimer -= Time.deltaTime;
        }
    }

    void TrackRotation()
    {
        float currentZ = transform.eulerAngles.z;
        float delta = Mathf.DeltaAngle(lastZRotation, currentZ);
        accumulatedRotation += Mathf.Abs(delta);
        lastZRotation = currentZ;

        if (accumulatedRotation >= 360f)
        {
            accumulatedRotation -= 360f;
            if (GameManager.Instance != null)
            {
                GameManager.Instance.AddLapPoints();
            }
        }
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Ground"))
        {
            isGrounded = true;
        }

        // Ignore slope speed changes if boosting
        if (surfaceEffector == null || isPlayerControlling || isBoosting) return;

        ContactPoint2D closestContact = GetClosestContactPoint(collision.contacts);
        Vector2 surfaceNormal = closestContact.normal;

        float slopeAngle = Vector2.Angle(surfaceNormal, Vector2.up);

        isOnDownwardSlope = false;

        if (slopeAngle > slopeThreshold)
        {
            float slopeDirection = Vector2.Dot(surfaceNormal, Vector2.right);

            if (slopeDirection > 0.1f)
            {
                isOnDownwardSlope = true;
                surfaceEffector.speed = Mathf.MoveTowards(surfaceEffector.speed, downhillSpeed, speedStep * Time.deltaTime * 60f);
            }
            else if (slopeDirection < -0.1f)
            {
                surfaceEffector.speed = Mathf.MoveTowards(surfaceEffector.speed, uphillSpeed, speedStep * Time.deltaTime * 60f);
            }
        }
        else
        {
            surfaceEffector.speed = Mathf.MoveTowards(surfaceEffector.speed, flatSpeed, speedStep * Time.deltaTime * 60f);
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Ground"))
        {
            isGrounded = false;
        }

        isOnDownwardSlope = false;
        // Ignore speed reset if boosting
        if (!isPlayerControlling && !isBoosting)
        {
            surfaceEffector.speed = Mathf.MoveTowards(surfaceEffector.speed, 0f, speedStep * Time.deltaTime * 60f);
        }
    }

    ContactPoint2D GetClosestContactPoint(ContactPoint2D[] contacts)
    {
        ContactPoint2D closest = contacts[0];
        float closestDistance = Vector2.Distance(transform.position, contacts[0].point);

        for (int i = 1; i < contacts.Length; i++)
        {
            float distance = Vector2.Distance(transform.position, contacts[i].point);
            if (distance < closestDistance)
            {
                closest = contacts[i];
                closestDistance = distance;
            }
        }

        return closest;
    }

    public float GetBoostCooldown()
    {
        return Mathf.Max(0f, cooldownTimer);
    }

    public float GetBoostCooldownMax()
    {
        return boostCooldown;
    }
}
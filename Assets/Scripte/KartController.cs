using UnityEngine;
using TMPro;
using System.Collections;

public class KartController : MonoBehaviour
{
    [Header("Player Settings")]
    [SerializeField] private int playerNumber = 1; // 1 ou 2


    [Header("Checkpoint Settings")]
    [SerializeField] private TextMeshProUGUI positionText;
    [SerializeField] private TextMeshProUGUI lapText;
    [SerializeField] private int currentCheckpoint = 0;
    [SerializeField] private int currentLap = 1;
    [SerializeField] private int totalLaps = 3;
    [Header("Race End Settings")]
    [SerializeField] private GameObject raceEndCanvas;
    [SerializeField] private TextMeshProUGUI winnerText;
    [SerializeField] private AudioClip victorySound;
    [Header("Respawn Settings")]
    [SerializeField] private float invincibilityDuration = 3f;
    [SerializeField] private float blinkInterval = 0.2f;
    [SerializeField] private Renderer[] kartRenderers; // Assignez tous les renderers du kart
    private bool isInvincible = false;
    [SerializeField] private float fallHeight = -10f; // Si le joueur tombe sous cette hauteur
    [SerializeField] private float respawnHeight = 2f; // Hauteur de réapparition
    [SerializeField] private float respawnDelay = 1f; // Délai avant réapparition
    [SerializeField] private ParticleSystem respawnParticles; // Effet visuel (optionnel)
    [SerializeField] private AudioClip deathSound; // Son de mort (optionnel)

    private Vector3 lastCheckpointPosition;
    private Quaternion lastCheckpointRotation;
    [Header("Movement Settings")]
    [SerializeField] private Rigidbody rb;
    [SerializeField] private float speedMax = 3f;
    [SerializeField] private float accelerationFactor = 0.1f;
    [SerializeField] private float rotationSpeed = 0.5f;
    [SerializeField] private AnimationCurve accelerationCurve;
    [SerializeField] private bool canMove = true;
    [SerializeField] private KeyCode resetSpeedKey = KeyCode.P;

    [Header("Boost Settings")]
    [SerializeField] private float normalBoostAmount = 10f;
    [SerializeField] private float superBoostAmount = 15f;
    [SerializeField] private float boostDuration = 2f;
    [SerializeField] private GameObject boostParticles;
    private bool isBoosting;

    [Header("Ground Detection Settings")]
    [SerializeField] private float raycastDistance = 1f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform raycastOrigin;
    [SerializeField] private float asphaltSpeedMultiplier = 1f;
    [SerializeField] private float grassSpeedMultiplier = 0.7f;
    [SerializeField] private float dirtSpeedMultiplier = 0.8f;
    [SerializeField] private float iceSpeedMultiplier = 1.2f;

    [Header("Countdown Settings")]
    [SerializeField] private GameObject countdownCanvas;
    [SerializeField] private TextMeshProUGUI countdownText;
    [SerializeField] private TextMeshProUGUI holdProgressText;
    [SerializeField] private float countdownDuration = 3f;
    [SerializeField] private float minHoldDuration = 1.0f;
    [SerializeField] private float perfectHoldMin = 1.5f;
    [SerializeField] private float perfectHoldMax = 2.5f;
    [SerializeField] private float penaltyDuration = 2f;
    [SerializeField] private float startBoostDuration = 2f;

    private float speed;
    private float accelerationLerpInterpolator;
    private bool isAccelerating;
    private string currentGroundType;
    private float currentSpeedMultiplier = 1f;
    private bool isSpinning;
    private float holdTimer = 0f;
    private bool isHoldingStart = false;

    // Contrôles par joueur
    private string horizontalInput;
    private string verticalInput;
    private KeyCode accelerateKey;
    private KeyCode boostKey;
    private KeyCode startKey;

    private void Awake()
    {
        ConfigureControls();
    }

    private void ConfigureControls()
    {
        if (playerNumber == 1)
        {
            horizontalInput = "Horizontal_P1";
            verticalInput = "Vertical_P1";
            accelerateKey = KeyCode.UpArrow;
            startKey = KeyCode.UpArrow;
        }
        else // Joueur 2
        {
            horizontalInput = "Horizontal_P2";
            verticalInput = "Vertical_P2";
            accelerateKey = KeyCode.W;
            startKey = KeyCode.W;
        }
    }
    public void OnCheckpointReached(int checkpointNumber, Vector3 position, Quaternion rotation)
    {
        if (checkpointNumber == currentCheckpoint + 1 ||
            (currentCheckpoint >= RaceManager.Instance.TotalCheckpoints && checkpointNumber == 1))
        {
            currentCheckpoint = checkpointNumber;
            lastCheckpointPosition = position;
            lastCheckpointRotation = rotation;

            RaceManager.Instance.UpdatePlayerProgress(playerNumber, currentCheckpoint, currentLap);
            UpdatePositionDisplay();

            if (currentCheckpoint >= RaceManager.Instance.TotalCheckpoints)
            {
                CompleteLap();
            }
        }
    }
    public void ActivateInvincibility()
    {
        if (!isInvincible)
        {
            StartCoroutine(InvincibilityCoroutine());
        }
    }
    private void CompleteLap()
    {
        currentLap++;
        currentCheckpoint = 0;
        RaceManager.Instance.UpdatePlayerProgress(playerNumber, currentCheckpoint, currentLap);
        UpdateLapDisplay();
        UpdatePositionDisplay();

        if (currentLap > totalLaps)
        {
            FinishRace();
        }
    }
    private void UpdateLapDisplay()
    {
        if (lapText != null)
        {
            lapText.text = $"Tour: {currentLap}/{totalLaps+1}";
        }
    }
    private void UpdatePositionDisplay()
    {
        if (positionText != null)
        {
            int position = RaceManager.Instance.GetPlayerPosition(playerNumber);
            positionText.text = GetPositionString(position);
        }
    }

    private string GetPositionString(int position)
    {
        switch (position)
        {
            case 1: return "1er";
            case 2: return "2ème";
            case 3: return "3ème";
            case 4: return "4ème";
            default: return position.ToString();
        }
    }
    private void FinishRace()
    {
        canMove = false;

        // Active l'écran de fin
        if (raceEndCanvas != null)
        {
            raceEndCanvas.SetActive(true);
            winnerText.text = $"JOUEUR {playerNumber} A GAGNÉ !";

            // Joue le son de victoire
            if (victorySound != null)
                AudioSource.PlayClipAtPoint(victorySound, Camera.main.transform.position);
        }

        // Optionnel : Arrête le temps du jeu
        Time.timeScale = 0f;
    }

    private void Start()
    {
        if (countdownCanvas != null)
        {
            countdownCanvas.SetActive(false);
            holdProgressText.gameObject.SetActive(false);
        }
        
        StartCoroutine(StartCountdown());
        UpdateLapDisplay();
    }

    private IEnumerator InvincibilityCoroutine()
    {
        isInvincible = true;
        

        float timer = 0f;
        bool visibleState = true;

        while (timer < invincibilityDuration)
        {
            // Alterne la visibilité des renderers
            foreach (Renderer renderer in kartRenderers)
            {
                renderer.enabled = visibleState;
            }

            visibleState = !visibleState;
            timer += blinkInterval;
            yield return new WaitForSeconds(blinkInterval);
        }

        // S'assurer que les renderers sont réactivés à la fin
        foreach (Renderer renderer in kartRenderers)
        {
            renderer.enabled = true;
        }

        isInvincible = false;

    }

    private IEnumerator StartCountdown()
    {
        canMove = false;
        countdownCanvas.SetActive(true);
        holdProgressText.gameObject.SetActive(true);
        holdProgressText.text = "0%";

        // Phase 1: Prêt?
        countdownText.text = "PRÊT?";
        yield return new WaitForSeconds(1f);

        // Réinitialisation du timer
        holdTimer = 0f;
        isHoldingStart = false;

        // Phase 2: 3...2...1
        for (int i = 3; i > 0; i--)
        {
            countdownText.text = i.ToString();
            
            // Vérification d'un départ anticipé
            if (!isHoldingStart && Input.GetKeyDown(startKey))
            {
                ApplyDamageEffect(DamageObject.DamageType.Banana, penaltyDuration);
                countdownText.text = "TROP TOT!";
                yield return new WaitForSeconds(penaltyDuration);
                countdownCanvas.SetActive(false);
                holdProgressText.gameObject.SetActive(false);
                canMove = true;
                yield break;
            }
            
            yield return new WaitForSeconds(1f);
        }

        // Phase 3: Partez!
        countdownText.text = "PARTEZ!";
        EvaluateHoldDuration();
        
        yield return new WaitForSeconds(0.5f);
        countdownCanvas.SetActive(false);
        holdProgressText.gameObject.SetActive(false);
        canMove = true;
    }

    private void Update()
    {
        if (Input.GetKeyDown(resetSpeedKey))
        {
            ResetSpeed();
        }
        if (transform.position.y < fallHeight)
        {
            Respawn();
        }
        if (!canMove)
        {
            HandleStartInput();
        }
        else if (!isSpinning)
        {
            HandleRotation();
            HandleAcceleration();
            DetectGround();
        }
    }

    private void HandleStartInput()
    {
        if (Input.GetKeyDown(startKey))
        {
            isHoldingStart = true;
            holdTimer = 0f;
        }

        if (Input.GetKeyUp(startKey))
        {
            isHoldingStart = false;
            holdTimer = 0f;
            holdProgressText.text = "0%";
        }

        if (isHoldingStart)
        {
            holdTimer += Time.deltaTime;
            UpdateHoldProgressUI();
        }
    }

    private void UpdateHoldProgressUI()
    {
        float progress = Mathf.Clamp01(holdTimer / perfectHoldMax);
        holdProgressText.text = $"{(progress * 100):F0}%";
        
        if (holdTimer >= perfectHoldMin && holdTimer <= perfectHoldMax)
        {
            holdProgressText.color = Color.green;
        }
        else
        {
            holdProgressText.color = Color.white;
        }
    }
    private void Respawn()
    {
        if (isSpinning) return;
        StartCoroutine(RespawnCoroutine());
    }
    private void ResetSpeed()
    {
        speed = 0f;
        accelerationLerpInterpolator = 0f;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        
    }
    private IEnumerator RespawnCoroutine()
    {
        // Désactive les contrôles
        canMove = false;

        // Joue un son (optionnel)
        if (deathSound != null)
            AudioSource.PlayClipAtPoint(deathSound, transform.position);

        // Effet visuel avant disparition (optionnel)
        if (respawnParticles != null)
            respawnParticles.Play();

        yield return new WaitForSeconds(respawnDelay / 2);

        // Réinitialise la physique
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        // Téléportation au checkpoint
        transform.position = lastCheckpointPosition + Vector3.up * respawnHeight;
        transform.rotation = lastCheckpointRotation;

        // Effet visuel après réapparition (optionnel)
        if (respawnParticles != null)
            respawnParticles.Play();

        yield return new WaitForSeconds(respawnDelay / 2);

        // Réactive les contrôles
        ResetSpeed();
        canMove = true;
    }

    private void EvaluateHoldDuration()
    {
        if (!isHoldingStart || holdTimer < minHoldDuration)
        {
            countdownText.text = "DÉPART!";
            return;
        }

        if (holdTimer >= perfectHoldMin && holdTimer <= perfectHoldMax)
        {
            ApplyBoostEffect(BoostObject.BoostType.Normal, startBoostDuration);
            countdownText.text = "PARFAIT!";
        }
        else if (holdTimer > perfectHoldMax)
        {
            ApplyDamageEffect(DamageObject.DamageType.Oil, penaltyDuration);
            countdownText.text = "TROP LONG!";
        }
        else
        {
            countdownText.text = "DÉPART!";
        }
    }
    private void HandleRotation()
    {
        float horizontal = Input.GetAxis(horizontalInput);
        transform.Rotate(Vector3.up, horizontal * rotationSpeed * Time.deltaTime);
    }

    private void HandleAcceleration()
    {
        if (Input.GetKeyDown(accelerateKey))
        {
            isAccelerating = true;
        }
        if (Input.GetKeyUp(accelerateKey))
        {
            isAccelerating = false;
        }
    }
    private void FixedUpdate()
    {
        UpdateAcceleration();
        MoveCar();
    }

    private void UpdateAcceleration()
    {
        accelerationLerpInterpolator += isAccelerating ? accelerationFactor : -accelerationFactor * 2;
        accelerationLerpInterpolator = Mathf.Clamp01(accelerationLerpInterpolator);
        speed = accelerationCurve.Evaluate(accelerationLerpInterpolator) * speedMax * currentSpeedMultiplier;
    }

    private void MoveCar()
    {
        rb.MovePosition(transform.position + transform.forward * speed * Time.fixedDeltaTime);
    }

    private void DetectGround()
    {
        RaycastHit hit;
        if (Physics.Raycast(raycastOrigin.position, Vector3.down, out hit, raycastDistance, groundLayer))
        {
            switch (hit.collider.tag)
            {
                case "Asphalt":
                    currentGroundType = "Asphalt";
                    currentSpeedMultiplier = asphaltSpeedMultiplier;
                    break;
                case "Grass":
                    currentGroundType = "Grass";
                    currentSpeedMultiplier = grassSpeedMultiplier;
                    break;
                case "Dirt":
                    currentGroundType = "Dirt";
                    currentSpeedMultiplier = dirtSpeedMultiplier;
                    break;
                case "Ice":
                    currentGroundType = "Ice";
                    currentSpeedMultiplier = iceSpeedMultiplier;
                    break;
                default:
                    currentGroundType = "Default";
                    currentSpeedMultiplier = 1f;
                    break;
            }
        }
        else
        {
            currentGroundType = "Default";
            currentSpeedMultiplier = 1f;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Wall"))
        {
            ResetSpeed();

            // Calcul de la direction (centre du mur vers le joueur)
            Vector3 pushBackDirection = transform.position - other.transform.position;
            pushBackDirection.y = 0; // On ignore la composante Y si besoin
            pushBackDirection.Normalize(); // Normalise pour avoir une direction pure

            // Force de recul
            float pushBackForce = 10f;
            rb.AddForce(pushBackDirection * pushBackForce, ForceMode.Impulse);

        }
        if (other.CompareTag("Death"))
        {
            Respawn();
            return;
        }

        // Checkpoint
        if (other.CompareTag("Checkpoint"))
        {
            Checkpoint checkpoint = other.GetComponent<Checkpoint>();
            if (checkpoint != null && checkpoint.respawnPoint != null)
            {
                OnCheckpointReached(
                    checkpoint.checkpointNumber,
                    checkpoint.respawnPoint.position,
                    checkpoint.respawnPoint.rotation
                );
            }
            return;
        }
        if (other.CompareTag("Boost"))
        {
            BoostObject boost = other.GetComponent<BoostObject>();
            if (boost != null)
            {
                ApplyBoostEffect(boost.boostType, boost.boostDuration);
                Destroy(other.gameObject);
            }
        }
        else if (other.CompareTag("BoostTerain"))
        {
            BoostObject boost = other.GetComponent<BoostObject>();
            if (boost != null)
            {
                ApplyBoostEffect(boost.boostType, boost.boostDuration);
            }
        }
        else
        {
            DamageObject damage = other.GetComponent<DamageObject>();
            if (damage != null && !isInvincible)
            {
                ApplyDamageEffect(damage.damageType, damage.slowdownDuration);
                if (!other.CompareTag("DamageTerain"))
                {
                    Destroy(other.gameObject);
                }
            }
        }
    }

    private void ApplyBoostEffect(BoostObject.BoostType boostType, float duration)
    {
        switch (boostType)
        {
            case BoostObject.BoostType.Normal:
                StartCoroutine(Boost(normalBoostAmount, duration));
                break;
            case BoostObject.BoostType.Super:
                StartCoroutine(Boost(superBoostAmount, duration));
                break;
            case BoostObject.BoostType.Infinite:
                StartCoroutine(Boost(normalBoostAmount, Mathf.Infinity));
                break;
        }
    }

    private void ApplyDamageEffect(DamageObject.DamageType damageType, float duration)
    {
        switch (damageType)
        {
            case DamageObject.DamageType.Banana:
                StartCoroutine(SpinEffect(0.1f, duration, 360f));
                break;
            case DamageObject.DamageType.Oil:
                StartCoroutine(SpinEffect(0.05f, duration, 720f));
                break;
            case DamageObject.DamageType.Spike:
                StartCoroutine(SpinEffect(0f, duration, 1080f));
                break;
        }
    }
    public void SetLastCheckpoint(Vector3 position, Quaternion rotation)
    {
        lastCheckpointPosition = position;
        lastCheckpointRotation = rotation;
        Debug.Log($"Player {playerNumber} saved checkpoint at {position}");
    }
    public IEnumerator Boost(float boostAmount, float duration)
    {
        if (isBoosting) yield break;
    
        isBoosting = true;
        float originalSpeed = speedMax;
        speedMax += boostAmount;
    
        bool wasAccelerating = isAccelerating;
        isAccelerating = true;
        accelerationLerpInterpolator = 1f;

        if (boostParticles != null)
            boostParticles.SetActive(true);

        if (!float.IsInfinity(duration))
        {
            yield return new WaitForSeconds(duration);
        
            speedMax = originalSpeed;
            isAccelerating = Input.GetKey(accelerateKey);
        
            if (boostParticles != null)
                boostParticles.SetActive(false);

            isBoosting = false;
        }
    }

    private IEnumerator SpinEffect(float speedMultiplier, float duration, float rotationDegrees)
    {
        StartCoroutine(InvincibilityCoroutine());
        isSpinning = true;
        float originalSpeed = speedMax;
        speedMax *= speedMultiplier;

        float rotationSpeed = rotationDegrees / duration;
        float timer = 0f;

        while (timer < duration)
        {
            transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);
            timer += Time.deltaTime;
            yield return null;
        }

        speedMax = originalSpeed;
        isSpinning = false;
        ResetSpeed();
    }
}
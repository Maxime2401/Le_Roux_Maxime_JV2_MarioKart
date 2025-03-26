using UnityEngine;
using TMPro;
using System.Collections;

public class KartController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private Rigidbody _rb;
    [SerializeField] private float _speedMax = 3f;
    [SerializeField] private float _accelerationFactor = 0.1f;
    [SerializeField] private float _rotationSpeed = 0.5f;
    [SerializeField] private AnimationCurve _accelerationCurve;
    [SerializeField] private bool _canMove = true;

    [Header("Boost Settings")]
    [SerializeField] private float _normalBoostAmount = 10f;
    [SerializeField] private float _superBoostAmount = 15f;
    [SerializeField] private float _boostDuration = 2f;
    [SerializeField] private ParticleSystem _boostParticles;
    private bool _isBoosting;

    [Header("Ground Detection Settings")]
    [SerializeField] private float _raycastDistance = 1f;
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private Transform _raycastOrigin;
    [SerializeField] private float _asphaltSpeedMultiplier = 1f;
    [SerializeField] private float _grassSpeedMultiplier = 0.7f;
    [SerializeField] private float _dirtSpeedMultiplier = 0.8f;
    [SerializeField] private float _iceSpeedMultiplier = 1.2f;

    [Header("Countdown Settings")]
    [SerializeField] private GameObject _countdownCanvas;
    [SerializeField] private TextMeshProUGUI _countdownText;
    [SerializeField] private TextMeshProUGUI _holdProgressText;
    [SerializeField] private float _countdownDuration = 3f;
    [SerializeField] private float _minHoldDuration = 1.0f;
    [SerializeField] private float _perfectHoldMin = 1.5f;
    [SerializeField] private float _perfectHoldMax = 2.5f;
    [SerializeField] private float _penaltyDuration = 2f;
    [SerializeField] private float _startBoostDuration = 2f;

    private float _speed;
    private float _accelerationLerpInterpolator;
    private bool _isAccelerating;
    private string _currentGroundType;
    private float _currentSpeedMultiplier = 1f;
    private bool _isSpinning;
    private float _holdTimer = 0f;
    private bool _isHoldingSpace = false;

    private void Start()
    {
        if (_countdownCanvas != null)
        {
            _countdownCanvas.SetActive(false);
            _holdProgressText.gameObject.SetActive(false);
        }
        
        StartCoroutine(StartCountdown());
    }

    private IEnumerator StartCountdown()
    {
        _canMove = false;
        _countdownCanvas.SetActive(true);
        _holdProgressText.gameObject.SetActive(true);
        _holdProgressText.text = "0%";

        // Phase 1: Prêt?
        _countdownText.text = "PRÊT?";
        yield return new WaitForSeconds(1f);

        // Réinitialisation du timer
        _holdTimer = 0f;
        _isHoldingSpace = false;

        // Phase 2: 3...2...1
        for (int i = 3; i > 0; i--)
        {
            _countdownText.text = i.ToString();
            
            // Vérification d'un départ anticipé (touche relâchée pendant le compte à rebours)
            if (!_isHoldingSpace && Input.GetKeyDown(KeyCode.Space))
            {
                ApplyDamageEffect(DamageObject.DamageType.Banana, _penaltyDuration);
                _countdownText.text = "TROP TOT!";
                yield return new WaitForSeconds(_penaltyDuration);
                _countdownCanvas.SetActive(false);
                _holdProgressText.gameObject.SetActive(false);
                _canMove = true;
                yield break;
            }
            
            yield return new WaitForSeconds(1f);
        }

        // Phase 3: Partez!
        _countdownText.text = "PARTEZ!";
        
        // Évaluation finale de la durée d'appui
        EvaluateHoldDuration();
        
        yield return new WaitForSeconds(0.5f);
        _countdownCanvas.SetActive(false);
        _holdProgressText.gameObject.SetActive(false);
        _canMove = true;
    }

    private void Update()
    {
        if (!_canMove)
        {
            HandleStartInput();
        }
        else if (!_isSpinning)
        {
            HandleRotation();
            HandleAcceleration();
            HandleBoost();
            DetectGround();
        }
    }

    private void HandleStartInput()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _isHoldingSpace = true;
            _holdTimer = 0f;
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            _isHoldingSpace = false;
            _holdTimer = 0f;
            _holdProgressText.text = "0%";
        }

        if (_isHoldingSpace)
        {
            _holdTimer += Time.deltaTime;
            UpdateHoldProgressUI();
        }
    }

    private void UpdateHoldProgressUI()
    {
        float progress = Mathf.Clamp01(_holdTimer / _perfectHoldMax);
        _holdProgressText.text = $"{(progress * 100):F0}%";
        
        if (_holdTimer >= _perfectHoldMin && _holdTimer <= _perfectHoldMax)
        {
            _holdProgressText.color = Color.green;
        }
        else
        {
            _holdProgressText.color = Color.white;
        }
    }

    private void EvaluateHoldDuration()
    {
        if (!_isHoldingSpace || _holdTimer < _minHoldDuration)
        {
            // Départ normal si pas d'appui ou temps trop court
            _countdownText.text = "DÉPART!";
            return;
        }

        if (_holdTimer >= _perfectHoldMin && _holdTimer <= _perfectHoldMax)
        {
            // Zone parfaite - boost
            ApplyBoostEffect(BoostObject.BoostType.Normal, _startBoostDuration);
            _countdownText.text = "PARFAIT!";
        }
        else if (_holdTimer > _perfectHoldMax)
        {
            // Trop long - pénalité
            ApplyDamageEffect(DamageObject.DamageType.Oil, _penaltyDuration);
            _countdownText.text = "TROP LONG!";
        }
        else
        {
            // Temps entre minHoldDuration et perfectHoldMin - départ normal
            _countdownText.text = "DÉPART!";
        }
    }

    // Toutes les autres méthodes existantes restent inchangées
    private void HandleRotation()
    {
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            transform.Rotate(Vector3.down, _rotationSpeed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            transform.Rotate(Vector3.up, _rotationSpeed * Time.deltaTime);
        }
    }

    private void HandleAcceleration()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _isAccelerating = true;
        }
        if (Input.GetKeyUp(KeyCode.Space))
        {
            _isAccelerating = false;
        }
    }

    private void HandleBoost()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            StartCoroutine(Boost(_normalBoostAmount, _boostDuration));
        }
    }

    private void FixedUpdate()
    {
        UpdateAcceleration();
        MoveCar();
    }

    private void UpdateAcceleration()
    {
        _accelerationLerpInterpolator += _isAccelerating ? _accelerationFactor : -_accelerationFactor * 2;
        _accelerationLerpInterpolator = Mathf.Clamp01(_accelerationLerpInterpolator);
        _speed = _accelerationCurve.Evaluate(_accelerationLerpInterpolator) * _speedMax * _currentSpeedMultiplier;
    }

    private void MoveCar()
    {
        _rb.MovePosition(transform.position + transform.forward * _speed * Time.fixedDeltaTime);
    }

    private void DetectGround()
    {
        RaycastHit hit;
        Debug.DrawRay(_raycastOrigin.position, Vector3.down * _raycastDistance, Color.red);

        if (Physics.Raycast(_raycastOrigin.position, Vector3.down, out hit, _raycastDistance, _groundLayer))
        {
            string groundTag = hit.collider.tag;
            Debug.Log($"[SOL] Le kart roule sur: {groundTag} (Objet: {hit.collider.name})");

            if (groundTag == "Asphalt")
            {
                _currentGroundType = "Asphalt";
                _currentSpeedMultiplier = _asphaltSpeedMultiplier;
            }
            else if (groundTag == "Grass")
            {
                _currentGroundType = "Grass";
                _currentSpeedMultiplier = _grassSpeedMultiplier;
            }
            else if (groundTag == "Dirt")
            {
                _currentGroundType = "Dirt";
                _currentSpeedMultiplier = _dirtSpeedMultiplier;
            }
            else if (groundTag == "Ice")
            {
                _currentGroundType = "Ice";
                _currentSpeedMultiplier = _iceSpeedMultiplier;
            }
            else
            {
                _currentGroundType = "Default";
                _currentSpeedMultiplier = 1f;
                Debug.LogWarning($"[SOL] Tag non reconnu: {groundTag} sur l'objet {hit.collider.name}");
            }
        }
        else
        {
            _currentGroundType = "Default";
            _currentSpeedMultiplier = 1f;
            Debug.Log("[SOL] Aucun sol détecté sous le kart");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Boost"))
        {
            BoostObject boost = other.GetComponent<BoostObject>();
            if (boost != null)
            {
                ApplyBoostEffect(boost.boostType, boost.boostDuration);
                Destroy(other.gameObject);
                return;
            }
        }
        if (other.CompareTag("BoostTerain"))
        {
            BoostObject boost = other.GetComponent<BoostObject>();
            if (boost != null)
            {
                ApplyBoostEffect(boost.boostType, boost.boostDuration);

                return;
            }
        }

        DamageObject damage = other.GetComponent<DamageObject>();
        if (damage != null)
        {
            ApplyDamageEffect(damage.damageType, damage.slowdownDuration);
            Destroy(other.gameObject);
        }
        if (other.CompareTag("DamageTerain"))
        {
            ApplyDamageEffect(damage.damageType, damage.slowdownDuration);
        }
    }

    private void ApplyBoostEffect(BoostObject.BoostType boostType, float duration)
    {
        switch (boostType)
        {
            case BoostObject.BoostType.Normal:
                StartCoroutine(Boost(_normalBoostAmount, duration));
                break;
            case BoostObject.BoostType.Super:
                StartCoroutine(Boost(_superBoostAmount, duration));
                break;
            case BoostObject.BoostType.Infinite:
                StartCoroutine(Boost(_normalBoostAmount, Mathf.Infinity));
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

    public IEnumerator Boost(float boostAmount, float duration)
    {
        if (_isBoosting) yield break;
    
        _isBoosting = true;
        float originalSpeed = _speedMax;
        _speedMax += boostAmount;
    
        // Sauvegarde l'état original et force l'accélération
        bool wasAccelerating = _isAccelerating;
        _isAccelerating = true;
        _accelerationLerpInterpolator = 1f; // Accélération immédiate

        if (_boostParticles != null)
            _boostParticles.Play();

        if (!float.IsInfinity(duration))
        {
            yield return new WaitForSeconds(duration);
        
            // Restauration
            _speedMax = originalSpeed;
        
            // Vérifie l'input actuel
            bool isSpacePressed = Input.GetKey(KeyCode.Space);
        
            // Si le joueur n'appuie pas, désactive l'accélération
            if (!isSpacePressed)
            {
                _isAccelerating = false;
                _accelerationLerpInterpolator = 0f;
            }
            // Si le joueur appuie, garde l'accélération activée
            else
            {
                _isAccelerating = true;
            }
        
            if (_boostParticles != null)
                _boostParticles.Stop();
            
            _isBoosting = false;
        }
    }

    private IEnumerator SpinEffect(float speedMultiplier, float duration, float rotationDegrees)
    {
        _isSpinning = true;
        float originalSpeed = _speedMax;
        _speedMax *= speedMultiplier;

        float rotationSpeed = rotationDegrees / duration;
        float timer = 0f;

        while (timer < duration)
        {
            transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);
            timer += Time.deltaTime;
            yield return null;
        }

        _speedMax = originalSpeed;
        _isSpinning = false;
    }
}
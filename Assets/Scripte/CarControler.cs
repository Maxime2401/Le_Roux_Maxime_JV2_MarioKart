using UnityEngine;
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

    private float _speed;
    private float _accelerationLerpInterpolator;
    private bool _isAccelerating;
    private string _currentGroundType;
    private float _currentSpeedMultiplier = 1f;
    private bool _isSpinning;

    private void Update()
    {
        if (!_canMove || _isSpinning) return;

        HandleRotation();
        HandleAcceleration();
        HandleBoost();
        DetectGround();
    }

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
            string groundTag = hit.collider.tag; // Déclaration de la variable manquante
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
    // Nouvelle méthode de détection qui évite l'erreur
        if (other.CompareTag("Boost"))
        {
        // Récupère le composant seulement si le tag est bon
            BoostObject boost = other.GetComponent<BoostObject>();
            if (boost != null)
            {
                ApplyBoostEffect(boost.boostType, boost.boostDuration);
                Destroy(other.gameObject);
                return;
            }
        }

        // Système de dégâts existant
        DamageObject damage = other.GetComponent<DamageObject>();
        if (damage != null)
        {
            ApplyDamageEffect(damage.damageType, damage.slowdownDuration);
            Destroy(other.gameObject);
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
        if (_boostParticles != null) _boostParticles.Play();

        if (!float.IsInfinity(duration))
        {
            yield return new WaitForSeconds(duration);
            _speedMax = originalSpeed;
            if (_boostParticles != null) _boostParticles.Stop();
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
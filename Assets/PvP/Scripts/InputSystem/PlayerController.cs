using Cinemachine;
using Mirror;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : NetworkBehaviour
{
    [Header("Distance of dash")]
    [SerializeField] private float _dashPower;

    [Header("Player Speed")]
    [SerializeField] private float _playerSpeed;

    [SerializeField] private CinemachineFreeLook _cinemachineFreeLook;
    [SerializeField] private Animator _playerAnimator;

    private CharacterController _characterController;
    private PlayerControls _playerControls;
    private Camera _mainCamera;

    private Vector2 _movementInput = Vector2.zero;
    private Vector2 _lookInput = Vector2.zero;

    private bool _canDash = true;
    private float _dashingTime = 0.2f;
    private float _dashingCooldown = 1f;

    private float _topClamp = 70.0f;
    private float _bottomClamp = -30.0f;
    private float _cameraAngleOverride = 0.0f;

    private float _cinemachineTargetYaw;
    private float _cinemachineTargetPitch;

    private const float _threshold = 0.01f;
    private float _currentAngle = 0.0f;
    private float _rotationVelocity;
    private float _rotationSmoothTime = 0.5f;

    public bool IsDashing { get; private set; }

    private void OnEnable()
    {
        _playerControls = new PlayerControls();
        _playerControls.Enable();
        _mainCamera = Camera.main;
    }

    public override void OnStartAuthority()
    {
        base.OnStartAuthority();
        PlayerInput playerInput = GetComponent<PlayerInput>();
        playerInput.enabled = true;
    }

    private void Start()
    {
        if (isLocalPlayer)
        {
            _cinemachineFreeLook = CinemachineFreeLook.FindObjectOfType<CinemachineFreeLook>();
            _cinemachineFreeLook.LookAt = gameObject.transform;
            _cinemachineFreeLook.Follow = gameObject.transform;
            Cursor.lockState = CursorLockMode.Locked;
        }
        _characterController = gameObject.GetComponent<CharacterController>();
    }

    private void CameraRotation()
    {
        if (_lookInput.sqrMagnitude >= _threshold)
        {
            _cinemachineTargetYaw += _lookInput.x * Time.deltaTime;
            _cinemachineTargetPitch += _lookInput.y * Time.deltaTime;
        }

        _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
        _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, _bottomClamp, _topClamp);
        _cinemachineFreeLook.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch + _cameraAngleOverride, _cinemachineTargetYaw, 0.0f);
    }

    void Update()
    {
        if (!isLocalPlayer) return;
        if (isOwned)
        {
            if (IsDashing) { return; }

            Move();

            if (_playerControls.Player.Dash.IsPressed() && _canDash)
            {
                StartCoroutine(Dash());
            }
        }
    }

    private void LateUpdate()
    {
        if (_cinemachineFreeLook != null)
            CameraRotation();
    }

    private void Move()
    {
        _movementInput = _playerControls.Player.Movement.ReadValue<Vector2>();
        Vector3 move = new Vector3(_movementInput.x, 0, _movementInput.y).normalized;

        if (move.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(move.x, move.z) * Mathf.Rad2Deg + _mainCamera.transform.eulerAngles.y;
            _currentAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref _rotationVelocity, _rotationSmoothTime);
            transform.rotation = Quaternion.Euler(0, _currentAngle, 0);
            Vector3 rotatedMovement = Quaternion.Euler(0, targetAngle, 0) * Vector3.forward;
            _characterController.Move(rotatedMovement * _playerSpeed * Time.deltaTime);
        }

        _playerAnimator.SetBool("isWalking", move != Vector3.zero);
    }

    private IEnumerator Dash()
    {
        _canDash = false;
        IsDashing = true;
        _playerAnimator.SetBool("isDashing", IsDashing);

        _characterController.Move(gameObject.transform.forward * _dashPower);

        yield return new WaitForSeconds(_dashingTime);
        IsDashing = false;
        _playerAnimator.SetBool("isDashing", IsDashing);

        yield return new WaitForSeconds(_dashingCooldown);

        _canDash = true;
    }

    private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
    {
        if (lfAngle < -360f) lfAngle += 360f;
        if (lfAngle > 360f) lfAngle -= 360f;
        return Mathf.Clamp(lfAngle, lfMin, lfMax);
    }
}

using Mirror;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : NetworkBehaviour
{
    [Header("Distance of dash")]
    [SerializeField] private float _dashPower = 250f;

    [Header("Player Speed")]
    [SerializeField] private float playerSpeed = 2.0f;

    private CharacterController _characterController;
    private PlayerControls _playerControls;
    private Rigidbody _playerRigidbody;
    private Animator _playerAnimator;

    private Vector3 _playerVelocity;   
    private Vector2 _movementInput = Vector2.zero;

    private bool _canDash = true;
    private bool _isDashing; 
    private float _dashingTime = 0.2f;
    private float _dashingCooldown = 1f;

    private void OnEnable()
    {
        _playerControls = new PlayerControls();
        _playerControls.Enable();
    }

    public override void OnStartAuthority()
    {
        base.OnStartAuthority();
        PlayerInput playerInput = GetComponent<PlayerInput>();
        playerInput.enabled = true;
    }

    private void Start()
    {
        _characterController = gameObject.GetComponent<CharacterController>();
        _playerRigidbody= gameObject.GetComponent<Rigidbody>();
        _playerAnimator = gameObject.GetComponent<Animator>();
    }

    void Update()
    {   
        if(isOwned)
        {
            if (_isDashing) { return; }

            Move();      

            if (_playerControls.Player.Dash.IsPressed() && _canDash)
            {
                StartCoroutine(Dash());
            }
        }
    }

    private void Move()
    {
        if (_playerVelocity.y < 0)
        {
            _playerVelocity.y = 0f;
        }
        _movementInput = _playerControls.Player.Movement.ReadValue<Vector2>();
        Vector3 move = new Vector3(_movementInput.x, 0, _movementInput.y);

        _characterController.Move(move * Time.deltaTime * playerSpeed);

        if (move != Vector3.zero)
        {
            gameObject.transform.forward = move;
        }

        _characterController.Move(_playerVelocity * Time.deltaTime);
    }

    IEnumerator Dash()
    {
        _canDash = false;
        _isDashing = true;

        _playerRigidbody.velocity = gameObject.transform.forward * _dashPower;

        yield return new WaitForSeconds(_dashingTime);
        _isDashing = false;

        yield return new WaitForSeconds(_dashingCooldown);
        _canDash = true;
    }
}

using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform _orientationtransform;
    [Header("Movement Settings")]
    [SerializeField] private KeyCode _movementkey;
    [SerializeField] private float _movementspeed;
    private Rigidbody _playerrigidbody;
    [Header("jump settings")]
    [SerializeField] private KeyCode _jumpKey;
    [SerializeField] private float _jumpforce;
    [SerializeField] private float _airmultiplier;
    [SerializeField] private float _airDrag;
    [SerializeField] private bool _canjump;
    [SerializeField] private float _jumpcooldown;
    [Header("Ground Check Settings")]
    [SerializeField] private float _playerheight;
    [SerializeField] private LayerMask _groundlayer;
    [SerializeField] private float _groundDrag;
    [Header("Slider Settings")]
    [SerializeField] private KeyCode _slideKey;
    [SerializeField] private float _slidemultiplayer;
    [SerializeField] private float _slideDrag;

    private StateController _stateController;


    private float _horizoninput, _verticalinput; //virgul yazarak kisaltabilirin
    private Vector3 _movementdirection;
    private bool _isSliding;//boyle birakirsak false baslar
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        _stateController = GetComponent<StateController>();
        _playerrigidbody = GetComponent<Rigidbody>();
        _playerrigidbody.freezeRotation = true;
    }

    private void SetInputs()
    {

        _horizoninput = Input.GetAxisRaw("Horizontal");
        _verticalinput = Input.GetAxisRaw("Vertical");

        if (Input.GetKeyDown(_slideKey))
        {
            _isSliding = true;
        }
        else if (Input.GetKeyDown(_movementkey))
        {

            _isSliding = false;
        }
        else if (Input.GetKey(_jumpKey) && _canjump && IsGrounded()) //means while _canjump is true  
        {
            _canjump = false;
            //ziplama eylemi gerceklesecek
            Setplayerjumping();
            Invoke(nameof(resetjumping), _jumpcooldown);//delay ile ayni isleve sahip. INVOKE, cooldowndan sonra calistirir resetjmpingi.
        }
    }


    // Update is called once per frame
    void Update()
    {
        SetInputs();
        setplayerdrag();
        LimitPlayerSpeed();
        SetStates();
    }
    private void FixedUpdate()
    {
        Setplayermovement();

    }

    private void Setplayermovement()
    {
        _movementdirection = _orientationtransform.forward * _verticalinput + _orientationtransform.right * _horizoninput;

        float forceMultiplayer = _stateController.GetCurrentState() switch
        {
            PlayerState.Move => 1f, //birle carpar hicbir extra etkisi yok yani
            PlayerState.Slide => _slidemultiplayer,
            PlayerState.Jump => _airmultiplier,
            _ => 1f
        };

            _playerrigidbody.AddForce(_movementdirection.normalized * _movementspeed * forceMultiplayer, ForceMode.Force);

    }
    private void setplayerdrag()
    {
        _playerrigidbody.linearDamping = _stateController.GetCurrentState() switch
        {

            PlayerState.Move => _groundDrag,
            PlayerState.Slide => _slideDrag,
            PlayerState.Jump => _airDrag,
            _ => _playerrigidbody.linearDamping
        };

    }

    private void LimitPlayerSpeed()
    {
        Vector3 flatVelocity = new Vector3(_playerrigidbody.linearVelocity.x, 0f, _playerrigidbody.linearVelocity.z);
        if (flatVelocity.magnitude > _movementspeed)
        {
            Vector3 limitedVelocity = flatVelocity.normalized * _movementspeed;
            _playerrigidbody.linearVelocity = new Vector3(limitedVelocity.x, _playerrigidbody.linearVelocity.y, limitedVelocity.z);
        }
    }


    private void Setplayerjumping()
    {
        _playerrigidbody.linearVelocity = new Vector3(_playerrigidbody.linearVelocity.x, 0f, _playerrigidbody.linearVelocity.z);
        _playerrigidbody.AddForce(transform.up * _jumpforce, ForceMode.Impulse);
    }

    private void resetjumping()
    {
        _canjump = true;
    }
    private bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, _playerheight * 0.5f + 0.2f, _groundlayer);//0.5f ve 0.2f adam bunlari deneme yanilmayla bulmus 
    }

    private Vector3 GetMovementDirection()
    {
        return _movementdirection.normalized;
    }

    private void SetStates()
    {
        var movementdirection = GetMovementDirection();
        var İsGrounded = IsGrounded();
        var currentState = _stateController.GetCurrentState();
        var isSliding = IsSliding();
        var newState = currentState switch
        {
            _ when movementdirection == Vector3.zero && IsGrounded() && !IsSliding() => PlayerState.Idle,
            _ when movementdirection != Vector3.zero && IsGrounded() && !IsSliding() => PlayerState.Move,
            _ when movementdirection != Vector3.zero && IsGrounded() && IsSliding() => PlayerState.Slide,
            _ when movementdirection == Vector3.zero && IsGrounded() && IsSliding() => PlayerState.SlideIdle,
            _ when !_canjump && !İsGrounded => PlayerState.Jump,
            _ => currentState

        };

        if (newState != currentState)
        {
            _stateController.ChangeState(newState);
        }
    }
    private bool IsSliding() {
        return _isSliding;
    }


}

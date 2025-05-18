
using System;
using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{


    [SerializeField] private Animator _playeranimator;
    private PlayerController _playerController;
    private StateController _stateController;



    private void Awake()
    {
        _playerController = GetComponent<PlayerController>();
        _stateController = GetComponent<StateController>();
    }

    private void Start()
    {
        _playerController.OnPlayerJumped += Ahmet_OnPlayerJumped; // += de abone oluyoruz sonraki kısımda da event'e isim veriyoruz ;
    }



    private void Update()
    {
        SetPlayerAni();
    }

    private void Ahmet_OnPlayerJumped()
    {
        _playeranimator.SetBool(Consts.PlayerAnimations.IS_JUMPING, true);
        Invoke(nameof(ResetJumping), 0.5f);
    }

    private void ResetJumping()
    {
        _playeranimator.SetBool(Consts.PlayerAnimations.IS_JUMPING, false);
    }

    private void SetPlayerAni()
    {
        var currentstate = _stateController.GetCurrentState();

        switch (currentstate)
        {
            case PlayerState.Idle:
                _playeranimator.SetBool(Consts.PlayerAnimations.IS_SLIDING, false);
                _playeranimator.SetBool(Consts.PlayerAnimations.IS_MOVING, false);
                break;

            case PlayerState.Move:
                _playeranimator.SetBool(Consts.PlayerAnimations.IS_SLIDING, false);
                _playeranimator.SetBool(Consts.PlayerAnimations.IS_MOVING, true);
                break;

            case PlayerState.Slide:
                _playeranimator.SetBool(Consts.PlayerAnimations.IS_SLIDING, true);
                _playeranimator.SetBool(Consts.PlayerAnimations.IS_SLIDING_ACTIVE, true);
                break;

            case PlayerState.SlideIdle:
                _playeranimator.SetBool(Consts.PlayerAnimations.IS_SLIDING_ACTIVE, false);
                _playeranimator.SetBool(Consts.PlayerAnimations.IS_SLIDING, true);
                break;
        }
    }

}
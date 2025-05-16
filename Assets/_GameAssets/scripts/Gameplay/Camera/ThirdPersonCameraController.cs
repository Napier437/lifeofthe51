using UnityEngine;

public class ThirdPersonCameraController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform _playertransform;
    [SerializeField] private Transform _orientationtransform;
    [SerializeField] private Transform _playerVisualtransform;
    [Header("Settings")]
    [SerializeField] private float _rotationSpeed;
    private void Update()
    {
        Vector3 viewDirection = _playertransform.position - new Vector3 (transform.position.x, _playertransform.position.y,transform.position.z);
        _orientationtransform.forward=viewDirection.normalized;
        float horizontalinput =Input.GetAxisRaw("Horizontal");
        float verticalinput =Input.GetAxisRaw("Vertical");
        Vector3 inputdirection 
        = _orientationtransform.forward * verticalinput + _orientationtransform.right * horizontalinput;

        if(inputdirection != Vector3.zero)
        {_playerVisualtransform.forward 
        =Vector3.Slerp(_playerVisualtransform.forward, inputdirection.normalized, Time.deltaTime * _rotationSpeed);}
        

    }
}

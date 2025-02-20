using System;
using CodeBlack.UI;
using PlazmaGames.Animation;
using PlazmaGames.Attribute;
using PlazmaGames.Core;
using PlazmaGames.UI;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

namespace CodeBlack.Player
{
	[RequireComponent(typeof(PlayerInput), typeof(CharacterController))]
	public sealed class PlayerController : MonoBehaviour
	{
		[Header("References")]
		[SerializeField] private PlayerSettings _playerSettings;
		[SerializeField] private CharacterController _characterController;
		[SerializeField] private PlayerInput _playerInput;
		[SerializeField] private AudioSource _audioSource;

		[Header("Body Part References")]
		[SerializeField] private GameObject _head;

		[Header("Physics")]
		[SerializeField] private float _gravityMultiplier = 3.0f;
		private float _gravity = -9.81f;
		private float _velY;

		private InputAction _moveAction;
		private InputAction _lookAction;
		private InputAction _pauseAction;
        private InputAction _crouchAction;
        private InputAction _shiftAction;

        private Vector3 _playerRotation;
		private Vector3 _headRotation;

		private Vector2 _rawMovementInput;
		private Vector2 _rawViewInput;

		private Vector3 _movementSpeed;
		private Vector3 _movementSpeedVel;
        private bool _inCartRange;
        private bool _pushingCart;
        private InputAction _interactAction;
        [SerializeField] Transform _cart;

        [SerializeField, ReadOnly] private bool _isCrouching = false;
        [SerializeField] private float _crouchDist = 0.6f;
        [SerializeField] private float _crouchDuration = 2f;
        private float _crouchPos = 0f;
        private float _crouchStartPos;
        private float _crouchEndPos;
        
        private bool _running = false;
        [SerializeField] private float _cartForward;

        //[SerializeField] private float _pushPower = 2.0f;
        //[SerializeField] private float _weight = 6.0f;

        public bool IsPushingCart()
        {
            return _pushingCart;
        }

        private void HandleMovementAction(InputAction.CallbackContext e)
		{
			_rawMovementInput = e.ReadValue<Vector2>();
		}

		private void HandleLookAction(InputAction.CallbackContext e)
		{
			_rawViewInput = e.ReadValue<Vector2>();
		}

		private void HandleShiftAction(InputAction.CallbackContext e)
        {
            _running = true;
        }
        
		private void HandleUnshiftAction(InputAction.CallbackContext e)
        {
            _running = false;
        }

        private float RunningModifyer() => _running ? _playerSettings.runningModifyer : 1f;
        
		private void ProcessMovement()
		{
            if (!_pushingCart)
            {
                float verticalSpeed = (_rawMovementInput.y == 1) ? _playerSettings.walkingForwardSpeed : _playerSettings.walkingBackwardSpeed;
                verticalSpeed *= RunningModifyer();
                float horizontalSpeed = _playerSettings.walkingStrateSpeed;
                horizontalSpeed *= RunningModifyer();

                verticalSpeed *= _playerSettings.speedEffector;
                horizontalSpeed *= _playerSettings.speedEffector;

                _movementSpeed = Vector3.SmoothDamp(
                    _movementSpeed,
                    new Vector3(-horizontalSpeed * _rawMovementInput.x * Time.deltaTime, 0, -verticalSpeed * _rawMovementInput.y * Time.deltaTime),
                    ref _movementSpeedVel,
                    _playerSettings.movementSmoothing
                );
            }
            else
            {
                float turn = _rawMovementInput.x;
                if (_rawMovementInput.y < 0 || _movementSpeed.x > 0.001f) turn *= -1;
                float turnAmount = ((_playerSettings.invertedViewX) ? -1 : 1) * _playerSettings.cartTurnSpeed * turn * Time.deltaTime;
                _playerRotation.y += turnAmount;
                transform.localRotation = Quaternion.Euler(_playerRotation);
                
                _movementSpeed = Vector3.SmoothDamp(
                    _movementSpeed,
                    new Vector3(_playerSettings.cartSpeed * -_rawMovementInput.y * Time.deltaTime, 0, 0),
                    ref _movementSpeedVel,
                    _playerSettings.cartSmoothing
                );
            }
		}

		private void ProcessView()
		{
			_headRotation.x -= ((_playerSettings.invertedViewY) ? 1 : -1) * _playerSettings.sensitivityY * _rawViewInput.y * Time.deltaTime;
			_headRotation.x = Mathf.Clamp(_headRotation.x, _playerSettings.minViewY, _playerSettings.maxViewY);
            if (_pushingCart)
            {
                _headRotation.y += ((_playerSettings.invertedViewX) ? -1 : 1) * _playerSettings.sensitivityX * _rawViewInput.x * Time.deltaTime;
            }
			_head.transform.localRotation = Quaternion.Euler(_headRotation);

            if (!_pushingCart)
            {
                _playerRotation.y += ((_playerSettings.invertedViewX) ? -1 : 1) * _playerSettings.sensitivityX * _rawViewInput.x * Time.deltaTime;
                transform.localRotation = Quaternion.Euler(_playerRotation);
            }
		}

		private void ProcessGravity()
		{
			if (_characterController.isGrounded && _velY < 0.0f) _velY = -1.0f;
			else _velY += _gravity * _gravityMultiplier * Time.deltaTime;
			_movementSpeed.y = _velY;
		}
		
		public void ZeroInput()
		{
			_rawMovementInput = Vector2.zero;
			_rawViewInput = Vector2.zero;
		}

		public void Disable()
		{
			_characterController.enabled = false;
		}

		public void Enable()
		{
			_characterController.enabled = true;
		}

		public void Rotate(float angle)
		{
			_playerRotation = (transform.localRotation * Quaternion.Euler(0f, angle, 0f)).eulerAngles;
		}

		public PlayerSettings GetPlayerSetings()
		{
			return _playerSettings;
		}

		private void OnCollisionEnter(Collision collision)
		{
			Debug.Log(collision.gameObject.name);
		}

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("CartHandle")) _inCartRange = true;
        }
        
        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("CartHandle")) _inCartRange = false;
        }

        private void Awake()
        {
            _cart = GameObject.FindWithTag("Cart").transform;
			if (_playerSettings == null) _playerSettings = new PlayerSettings();
			if (_characterController == null) _characterController = GetComponent<CharacterController>();
			if (_playerInput == null) _playerInput = GetComponent<PlayerInput>();
			if (_audioSource == null) _audioSource = GetComponent<AudioSource>();
            
			_headRotation = _head.transform.localRotation.eulerAngles;
            _crouchStartPos = _head.transform.localPosition.y;
            _crouchEndPos = _crouchStartPos - _crouchDist;

			_moveAction = _playerInput.actions["Move"];
			_lookAction = _playerInput.actions["Look"];
            _interactAction = _playerInput.actions["MoveCart"];
            _crouchAction = _playerInput.actions["Crouch"];
            _shiftAction = _playerInput.actions["Sprint"];

            _interactAction.performed += HandleInteractAction;
            _interactAction.canceled += HandleEndInteractAction;
            _moveAction.performed += HandleMovementAction;
			_lookAction.performed += HandleLookAction;
            _crouchAction.performed += HandleStartCrouchAction;
            _crouchAction.canceled += HandleEndCrouchAction;
			_shiftAction.performed += HandleShiftAction;
			_shiftAction.canceled += HandleUnshiftAction;
        }


        private void CrouchAnimation(float p)
        {
            _head.transform.localPosition = new Vector3(_head.transform.localPosition.x, Mathf.Lerp(_crouchStartPos, _crouchEndPos, p), _head.transform.localPosition.z);
        }

        private void HandleStartCrouchAction(InputAction.CallbackContext e)
        {
            if (!_isCrouching)
            {
                GameManager.GetMonoSystem<IAnimationMonoSystem>().StopAllAnimations(this);
                float startCrouchPos = _crouchPos;
                GameManager.GetMonoSystem<IAnimationMonoSystem>().RequestAnimation(
                    this,
                    _crouchDuration * (1f - _crouchPos),
                    (float progress) =>
                    {
                        _crouchPos = Mathf.Lerp(startCrouchPos, 1.0f, progress);
                        CrouchAnimation(_crouchPos);
                    });
                _isCrouching = true;
            }
        }

        private void HandleEndCrouchAction(InputAction.CallbackContext e)
        {
            if (_isCrouching)
            {
                GameManager.GetMonoSystem<IAnimationMonoSystem>().StopAllAnimations(this);
                float startCrouchPos = _crouchPos;
                GameManager.GetMonoSystem<IAnimationMonoSystem>().RequestAnimation(
                    this,
                    _crouchDuration * _crouchPos,
                    (float progress) =>
                    {
                        _crouchPos = Mathf.Lerp(startCrouchPos, 0.0f, progress);
                        CrouchAnimation(_crouchPos);
                    });
                _isCrouching = false;
            }
        }

        private void HandleInteractAction(InputAction.CallbackContext e)
        {
            if (_inCartRange)
            {
                if (!_pushingCart) EnterCart();
            }
        }

        private void HandleEndInteractAction(InputAction.CallbackContext e)
        {
            if (_inCartRange)
            {
                if (_pushingCart) ExitCart();
            }
        }

        private void ExitCart()
        {
            Debug.Log("exit cart");
            _pushingCart = false;
            
            _cart.SetParent(null, true);
            //_cart.GetComponent<Collider>().enabled = true;
            
            _characterController.center = Vector3.zero;

            _playerRotation.y = _head.transform.rotation.eulerAngles.y;
            transform.localRotation = Quaternion.Euler(_playerRotation);

            _headRotation.y = 0;
            _head.transform.localRotation = Quaternion.Euler(_headRotation);
        }

        private void EnterCart()
        {
            Debug.Log("enter cart");
            _pushingCart = true;
            
            //_cart.GetComponent<Collider>().enabled = false;
            
            Vector3 oldRot = _playerRotation;
            Vector3 d = _cart.right;
            d.y = 0;
            d = d.normalized;
            _playerRotation.y = -Vector3.SignedAngle(d, Vector3.left, Vector3.up);
            transform.rotation = Quaternion.Euler(_playerRotation);

            _headRotation.y += oldRot.y - _playerRotation.y;
			_head.transform.localRotation = Quaternion.Euler(_headRotation);
            
            
            _characterController.center = new Vector3(_cartForward, 0, 0);

            _cart.SetParent(transform, true);
            _movementSpeed = Vector3.zero;
        }

        private void OnControllerColliderHit(ControllerColliderHit hit)
        {
            if (hit.transform.TryGetComponent(out Door door))
                door.Open(transform);
            else if (hit.transform.TryGetComponent(out DoubleDoorHandle handle))
                handle.Open(transform);
        }

        private void Update()
		{
            if (!_characterController.enabled) return;

            ProcessView();
			ProcessMovement();
			ProcessGravity();
			_characterController.Move(transform.TransformDirection(_movementSpeed));

            if (_interactAction.IsInProgress()) HandleInteractAction(default);
		}

        private void LateUpdate()
        {
            if (_inCartRange && !IsPushingCart()) GameManager.GetMonoSystem<IUIMonoSystem>().GetView<GameView>().SetHint("Hold 'Space' To Push");
        }
    }
}

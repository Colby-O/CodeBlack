using PlazmaGames.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using CodeBlack.Inspectables;
using CodeBlack.Player;

namespace CodeBlack.Player
{
	[RequireComponent(typeof(PlayerInput))]
	internal class Inspector : MonoBehaviour
	{
		[Header("References")]
		[SerializeField] private PlayerInput _playerInput;
		[SerializeField] private GameObject _offset;
		[SerializeField] private PlayerController _controller;
		[SerializeField] private GameObject _head;

        private Vector3 _lastMousePosition;
		private Transform _examinedObject;
		private ExamineType _type;
		private GameObject _objectOffset;

		private Vector3 _origHeadPosition;
		private Quaternion _origHeadRotation;

		private Dictionary<Transform, Vector3> _origPositions = new Dictionary<Transform, Vector3>();
		private Dictionary<Transform, Quaternion> _origRotations = new Dictionary<Transform, Quaternion>();

		private bool _isExaming;
		private bool _movingBack = false;

        private bool _isMoveable = false;

        private bool _readable = false;
        private bool _reading = false;
		private string _text = string.Empty;

        public bool IsExaming => _isExaming;

        public bool IsMoveable => _isMoveable;

		private bool _firstLoop = true;

        public void StartExamine(Transform obj, ExamineType type, GameObject offset, string text = "", bool isMoveable = false)
		{
			if (obj == null && !_isExaming) return;

            _firstLoop = true;

            _isExaming = true;

			_isMoveable = isMoveable;

            _readable = text.Length > 0;
			_text = text;

           _examinedObject = obj;
			_type = type;
			_objectOffset = offset;

			_origHeadPosition = _head.transform.position;
			_origHeadRotation = _head.transform.rotation;

			_origPositions[_examinedObject] = _examinedObject.position;
			_origRotations[_examinedObject] = _examinedObject.rotation;

			_lastMousePosition = Input.mousePosition;
            if (!_isMoveable) Cursor.lockState = CursorLockMode.None;
            if (!_isMoveable) Cursor.visible = true;

            if (!_isMoveable) _playerInput.enabled = false;
            if (!_isMoveable) _controller.Disable();

            Rigidbody rb = obj.GetComponent<Rigidbody>();
			if (rb)
			{
				rb.isKinematic = true;
			}

            Collider colider = obj.GetComponent<Collider>();
            if (colider)
            {
                colider.enabled= false;
            }
        }

		public void EndExamine()
		{
			if (_type == ExamineType.ComeTo)
			{
				_head.transform.position = _origHeadPosition;
				_head.transform.rotation = _origHeadRotation;
			}

			_isExaming = false;
            Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
			_playerInput.enabled = true;
			_controller.Enable();
			
			if (_isMoveable)
			{
                Rigidbody rb = _examinedObject.GetComponent<Rigidbody>();
                if (rb)
                {
                    rb.isKinematic = false;
					rb.transform.parent = null;
                }

                Collider colider = _examinedObject.GetComponent<Collider>();
                if (colider)
                {
                    colider.enabled = true;
                }

                return;
			}

			_movingBack = true;
		}

		void Examine()
		{
			if (_examinedObject != null)
			{

				if (_type == ExamineType.Goto)
				{
					if (_isMoveable)
					{
                        _examinedObject.position = _offset.transform.position;
						if (Keyboard.current[Key.RightArrow].wasPressedThisFrame) _examinedObject.Rotate(Vector3.right, 90f);
                        else if (Keyboard.current[Key.LeftArrow].wasPressedThisFrame) _examinedObject.Rotate(Vector3.right, -90f);
                        if (Keyboard.current[Key.UpArrow].wasPressedThisFrame) _examinedObject.Rotate(Vector3.up, 90f);
                        else if (Keyboard.current[Key.DownArrow].wasPressedThisFrame) _examinedObject.Rotate(Vector3.up, -90f);
                        return;

                    }
                    _examinedObject.position = Vector3.Lerp(_examinedObject.position, _offset.transform.position, 0.2f);
					Vector3 deltaMouse = Input.mousePosition - _lastMousePosition;
					float rotationSpeed = 1.0f;
					_examinedObject.Rotate(deltaMouse.x * rotationSpeed * Vector3.up, Space.World);
					_examinedObject.Rotate(deltaMouse.y * rotationSpeed * Vector3.left, Space.World);
					_lastMousePosition = Input.mousePosition;
				}
				else if (_type == ExamineType.ComeTo)
				{
					_head.transform.rotation = Quaternion.LookRotation(-(_examinedObject.transform.position - _objectOffset.transform.position).normalized);
					_head.transform.position = Vector3.Lerp(_head.transform.position, _objectOffset.transform.position, 0.2f);
				}
			}
		}

		private void CancelExamine()
		{
			if (_examinedObject == null) return;

			if (_origPositions.ContainsKey(_examinedObject))
			{
				_examinedObject.position = Vector3.Lerp(_examinedObject.position, _origPositions[_examinedObject], 0.2f);
			}

			if (_origRotations.ContainsKey(_examinedObject))
			{
				_examinedObject.rotation = Quaternion.Slerp(_examinedObject.rotation, _origRotations[_examinedObject], 0.2f);
			}

			if (
				(_examinedObject.position - _origPositions[_examinedObject]).magnitude < 0.01
			)
			{
				Rigidbody rb = _examinedObject.GetComponent<Rigidbody>();
				if (rb)
				{
					rb.isKinematic = false;
				}

                Collider colider = _examinedObject.GetComponent<Collider>();
                if (colider)
                {
                    colider.enabled = true;
                }

                _movingBack = false;
			}
		}

		private void StartRead()
		{
			_reading = true;
            //GameManager.GetMonoSystem<IUIMonoSystem>().GetView<TextView>().SetText(_text);
            //GameManager.GetMonoSystem<IUIMonoSystem>().Show<TextView>();
        }

        private void StopRead()
        {
            _reading = false;
            //GameManager.GetMonoSystem<IUIMonoSystem>().ShowLast();
        }

        private void Awake()
		{
            if (_playerInput == null) _playerInput = GetComponent<PlayerInput>();
			if (_controller == null) _controller = GetComponent<PlayerController>();
        }

		private void Update()
		{
            if (!_firstLoop && _isMoveable && _isExaming && Input.GetKeyDown(KeyCode.E)) EndExamine();

            if (_readable && _isExaming && Input.GetKeyDown(KeyCode.R)) StartRead();

			if (_reading && _isExaming && Input.GetKeyDown(KeyCode.Escape)) StopRead();
			else if (!_reading && _isExaming && Input.GetKeyDown(KeyCode.Escape)) EndExamine();

			if (_isExaming) Examine();
			else if (_movingBack && !_isMoveable) CancelExamine();

			_firstLoop = false;

        }
	}

}

using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Runtime.Constants;
using Runtime.Models;
using Snake.Scripts.Runtime.Tools;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace Runtime.Snake
{
    public class Snake : MonoBehaviour
    {
        public event UnityAction<Turn> Turned;
        
        [SerializeField] private SnakePart _snakePartPrefab;
        [SerializeField] private float _movementSpeed;
        [SerializeField] private float _turnDelaySeconds = 0.2f;
        
        private readonly List<SnakePart> _parts = new();

        private SnakePart _mainPart;
        private Vector2 _direction = new(1, 0);
        [SerializeField] private Timer _turnTimer;
        
        public float MovementSpeed => _movementSpeed;

        private CachedInput _cachedInput;
        
        #region INPUT_ACTIONS

        private InputAction _moveAction;
        private InputAction _horizontalClickAction;
        private InputAction _verticalClickAction;

        #endregion

        private void Awake()
        {
            _turnTimer = new Timer(_turnDelaySeconds);
            _turnTimer.Finished.AddListener(OnTurnDelayFinished);
        }
        
        private void Start()
        {
            _mainPart = GetComponent<SnakePart>();
            
            _parts.Add(_mainPart);
            
            _turnTimer.Start();
            
            InitInputs();
            
            SpawnPart();
            SpawnPart();
            SpawnPart();
            
            Turned?.Invoke(new Turn(transform.position, _direction));
        }

        private void Update()
        {
            _turnTimer.Tick();
            RotateToDirection();
        }

        private void FixedUpdate()
        {
            transform.Translate(_direction * (_movementSpeed * Time.fixedDeltaTime), Space.World);
            _parts.ForEach(part => part.ApplyMovement());
        }

        private void InitInputs()
        {
            _moveAction = InputSystem.actions.FindAction(InputNames.Move);
            _horizontalClickAction = InputSystem.actions.FindAction(InputNames.HorizontalClick);
            _verticalClickAction = InputSystem.actions.FindAction(InputNames.VerticalClick);

            _horizontalClickAction.started += _ => OnInput(Vector2.right);
            _verticalClickAction.started += _ => OnInput(Vector2.up);
        }

        private void OnInput(Vector2 direction)
        {
            var input = _moveAction.ReadValue<Vector2>();
            if (!_turnTimer.IsRunning) Turn(direction, input);
            else _cachedInput = new CachedInput { Direction = direction, Input = input };
        }

        private void Turn(Vector2 direction, Vector2 input)
        {
            Vector2 previousDirection = _direction;

            if (Mathf.Approximately(-_direction.x, -input.x) || Mathf.Approximately(_direction.y, -direction.y)) return;
            
            _direction = direction * input;
            
            if (previousDirection != _direction)
            {
                _turnTimer.Start();
                Turned?.Invoke(new Turn(transform.position, _direction));
            }

            _cachedInput = null;
        }

        private void OnTurnDelayFinished()
        {
            if (_cachedInput is not null) Turn(_cachedInput.Direction, _cachedInput.Input);
        }

        private void RotateToDirection()
        {
            if (Mathf.Abs(_direction.y) > 0)
            {
                transform.localRotation = Quaternion.Euler(0f, 0f, Mathf.Approximately(_direction.y, -1) ? 180 : 0);
                return;
            }

            if (Mathf.Abs(_direction.x) > 0)
            {
                transform.localRotation = Quaternion.Euler(0f, 0f, -90f * _direction.x);
            }
        }

        private void SpawnPart()
        {
            SnakePart lastPart = _parts.Last();
            
            Vector2 spawnPosition = lastPart.transform.position + -lastPart.transform.up;
            
            SnakePart snakePart = Instantiate(_snakePartPrefab, spawnPosition, Quaternion.identity);

            snakePart.Init(this);
            
            _parts.Add(snakePart);
        }
    }
}
using System.Linq;
using System.Collections.Generic;
using Runtime.Constants;
using Runtime.FruitScripts;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using Runtime.Models;
using Tools;

namespace Runtime.SnakeScripts
{
    [RequireComponent(typeof(RoundedLineRenderer))]
    public class Snake : MonoBehaviour
    {
        public event UnityAction<Turn> Turned;
        public event UnityAction Died;
        
        [Header("Parts")]
        [SerializeField] private SnakePart _snakePartPrefab;
        [SerializeField] private int _initialPartsCount = 10;
        [SerializeField] private float _firstPartOffsetMultiplier = 0.3f;
        [SerializeField] private float _defaultPartOffsetMultiplier = 0.5f;
        
        [Header("Movement")]
        [SerializeField] private float _movementSpeed;
        [SerializeField] private float _turnDelaySeconds = 0.2f;
        
        private List<SnakePart> _parts;

        private Timer _turnTimer;
        private SnakePart _mainPart;
        private Vector2 _direction = new(0, 1);
        private RoundedLineRenderer _lineRendererRenderer;
        
        public float MovementSpeed => _movementSpeed;

        private CachedInput _cachedInput;
        
        public bool IsDead { get; private set; }
        
        #region INPUT_ACTIONS

        private InputAction _moveAction;
        private InputAction _horizontalClickAction;
        private InputAction _verticalClickAction;

        #endregion
        
        #region INITIALIZATION
        
        private void Awake()
        {
            _parts = new List<SnakePart>();
            _turnTimer = new Timer(_turnDelaySeconds);
            _lineRendererRenderer = GetComponent<RoundedLineRenderer>();
            _turnTimer.Finished.AddListener(OnTurnDelayFinished);
        }
        
        private void Start()
        {
            _mainPart = GetComponent<SnakePart>();
            
            _parts.Add(_mainPart);
            
            _turnTimer.Start();
            
            InitInputs();

            for (int i = 0; i < _initialPartsCount; i++)
            {
                if (i is 0)
                {
                    SpawnPart(_firstPartOffsetMultiplier);
                    continue;
                }

                SpawnPart();
            }
            
            Turned?.Invoke(new Turn(transform.position, _direction));
        }
        
        private void InitInputs()
        {
            _moveAction = InputSystem.actions.FindAction(InputNames.Move);
            _horizontalClickAction = InputSystem.actions.FindAction(InputNames.HorizontalClick);
            _verticalClickAction = InputSystem.actions.FindAction(InputNames.VerticalClick);

            var debugSpawnAction = InputSystem.actions.FindAction(InputNames.Interact);
            
            _horizontalClickAction.started += OnHorizontalInput;
            _verticalClickAction.started += OnVerticalInput;
            debugSpawnAction.started += OnInteract;
        }

        private void OnDestroy()
        {
            _horizontalClickAction.started -= OnHorizontalInput;
            _verticalClickAction.started -= OnVerticalInput;
            var debugSpawnAction = InputSystem.actions.FindAction(InputNames.Interact);
            debugSpawnAction.started -= OnInteract;
        }
        
        private void OnHorizontalInput(InputAction.CallbackContext _) => OnInput(Vector2.right);
        private void OnVerticalInput(InputAction.CallbackContext _) => OnInput(Vector2.up);
        private void OnInteract(InputAction.CallbackContext _) => SpawnPart();
        
        #endregion

        private void Update()
        {
            _turnTimer.Tick();
            RotateToDirection();
            UpdateLine();
        }

        private void FixedUpdate()
        {
            ApplyMovement();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.GetComponent<SnakePart>() is not null)
            {
                IsDead = true;
                Died?.Invoke();
                return;
            }

            if (other.GetComponent<Fruit>() is Fruit fruit)
            {
                fruit.Eat();
                SpawnPart();
            }
        }

        private void OnInput(Vector2 direction)
        {
            var input = _moveAction.ReadValue<Vector2>();
            if (!_turnTimer.IsRunning) Turn(direction, input);
            else _cachedInput = new CachedInput { Direction = direction, Input = input };
        }

        private void ApplyMovement()
        {
            if (IsDead) return;
            transform.Translate(_direction * (_movementSpeed * Time.fixedDeltaTime), Space.World);
        }

        private void Turn(Vector2 direction, Vector2 input)
        {
            if (IsDead) return;
            
            Vector2 previousDirection = _direction;

            if (Mathf.Approximately(-_direction.x, -input.x) || Mathf.Approximately(_direction.y, -direction.y)) return;
            
            _direction = (direction * input).normalized;
            
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
            if (_direction.y != 0)
            {
                transform.localRotation = Quaternion.Euler(0f, 0f, Mathf.Approximately(_direction.y, 1) ? 0 : 180);
                return;
            }

            if (_direction.x != 0)
            {
                transform.localRotation = Quaternion.Euler(0f, 0f, Mathf.Approximately(_direction.x, 1) ? -90 : 90);
            }
        }

        private void SpawnPart(float offsetMultiplier = -1)
        {
            float multiplier = offsetMultiplier is -1 ? _defaultPartOffsetMultiplier : offsetMultiplier;
            
            SnakePart lastPart = _parts.Last();
            
            Vector2 spawnPosition = lastPart.transform.position + -lastPart.transform.up * multiplier;
            
            SnakePart snakePart = Instantiate(_snakePartPrefab, spawnPosition, Quaternion.identity);

            snakePart.Init(this, lastPart);
            
            _parts.Add(snakePart);
        }

        private void UpdateLine()
        {
            var points = new List<Vector2>();
            
            for (int i = 0; i < _parts.Count; i++)
            {
                SnakePart part = _parts[i];
                points.Add(part.transform.position);
                if (i is 0)
                {
                    points.Add(transform.position + -transform.up * _firstPartOffsetMultiplier);
                }
            }

            _lineRendererRenderer.SetPositions(points);
        }
    }
}
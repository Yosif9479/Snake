using System.Collections.Generic;
using UnityEngine;

namespace Runtime.Snake
{
    public class SnakePart : MonoBehaviour
    {
        private const float MinDistanceToNextPart = 0.5f;
        private const float MaxDistanceToNextPart = 0.5f;
        
        private Snake _snake;
        private SnakePart _nextPart;

        private Vector2 _direction = Vector2.up;
        private const float EnoughDistanceToTurn = 0.05f;

        private Queue<Turn> Turns { get; set; } = new();

        private Turn TargetTurn { get; set; }

        private Vector2 Direction => _direction;

        public void Init(Snake snake, SnakePart nextPart)
        {
            _snake = snake;
            _nextPart = nextPart;
            Turns = new Queue<Turn>(_nextPart.Turns);
            TargetTurn = _nextPart.TargetTurn;
            _direction = _nextPart.Direction;
            _snake.Turned += OnTurned;
        }

        private void OnDestroy()
        {
            if (_snake is null) return;
            _snake.Turned -= OnTurned;
        }

        private void Update()
        {
            CheckTurn();
            RotateToDirection();
        }

        private void FixedUpdate()
        {
            ApplyMovement();
        }

        private void OnTurned(Turn turn)
        {
            Turns.Enqueue(turn);

            if (Turns.Count == 1 && TargetTurn is null) TargetTurn = Turns.Dequeue();
        }

        private void CheckTurn()
        {
            if (TargetTurn is null) return;

            if (!IsCloseEnoughTo(TargetTurn.Position)) return;
            
            _direction = TargetTurn.Direction;
            
            transform.position = TargetTurn.Position;
            
            TargetTurn = Turns.Count > 0 ? Turns.Dequeue() : null;
        }
        
        private void ApplyMovement()
        {
            if (_snake is null) return;

            float adjustedSpeed = _snake.MovementSpeed;
    
            if (TargetTurn != null)
            {
                float distanceToNext = Vector2.Distance(transform.position, _nextPart.transform.position);

                if (distanceToNext < MinDistanceToNextPart) adjustedSpeed *= 0.5f;
                
                if (distanceToNext > MaxDistanceToNextPart) adjustedSpeed *= 1.5f;
            }

            transform.Translate(_direction * (adjustedSpeed * Time.fixedDeltaTime), Space.World);
        }


        private void RotateToDirection()
        {
            if (_snake is null) return;
            
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

        private bool IsCloseEnoughTo(Vector2 position)
        {
            return Vector2.Distance(transform.position, position) <= EnoughDistanceToTurn;
        }
    }
}
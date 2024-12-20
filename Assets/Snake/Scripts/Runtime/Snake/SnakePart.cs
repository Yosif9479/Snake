using System.Collections.Generic;
using UnityEngine;

namespace Runtime.Snake
{
    public class SnakePart : MonoBehaviour
    {
        private Snake _snake;

        private readonly Queue<Turn> _turns = new();
        private Turn _targetTurn;

        private Vector2 _direction = Vector2.up;
        private const float EnoughDistanceToTurn = 0.01f;

        public void Init(Snake snake)
        {
            _snake = snake;
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

        private void OnTurned(Turn turn)
        {
            _turns.Enqueue(turn);

            if (_turns.Count == 1 && _targetTurn is null) _targetTurn = _turns.Dequeue();
        }

        private void CheckTurn()
        {
            if (_targetTurn is null) return;

            if (IsCloseEnoughTo(_targetTurn.Position))
            {
                _direction = _targetTurn.Direction;
                transform.position = _targetTurn.Position;
                _targetTurn = _turns.Count > 0 ? _turns.Dequeue() : null;
            }
        }
        
        public void ApplyMovement()
        {
            if (_snake is null) return;
            transform.Translate(_direction * (_snake.MovementSpeed * Time.fixedDeltaTime), Space.World);
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
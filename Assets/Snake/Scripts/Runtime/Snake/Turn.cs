using UnityEngine;

namespace Runtime.Snake
{
    public class Turn
    {
        public Turn(Vector2 position, Vector2 direction)
        {
            Direction = direction;
            Position = position;
        }

        public Vector2 Direction { get; set; }
        public Vector3 Position { get; set; }
    }
}
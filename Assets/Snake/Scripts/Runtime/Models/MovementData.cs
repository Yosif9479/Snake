using System;
using UnityEngine;

namespace Runtime.Models
{
    [Serializable]
    public struct MovementData
    {
        public float Speed { get; set; }
        public Vector2 Direction { get; set; }
    }
}
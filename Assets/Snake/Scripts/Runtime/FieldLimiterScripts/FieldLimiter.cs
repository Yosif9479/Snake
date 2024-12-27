using System.Collections.Generic;
using UnityEngine;

namespace Runtime.FieldLimiterScripts
{
    [RequireComponent(typeof(EdgeCollider2D))]
    public class FieldLimiter : MonoBehaviour
    {
        private Camera _camera;
        private EdgeCollider2D _edge;
        
        private void Awake()
        {
            _edge = GetComponent<EdgeCollider2D>();
            _camera = Camera.main;
            LimitField();   
        }

        private void LimitField()
        {
            Vector2 leftBottom = _camera.ScreenToWorldPoint(new Vector2(0, 0));
            Vector2 rightBottom = _camera.ScreenToWorldPoint(new Vector2(Screen.width, 0));
            Vector2 leftTop = _camera.ScreenToWorldPoint(new Vector2(0, Screen.height));
            Vector2 rightTop = _camera.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height));

            var points = new List<Vector2> { leftBottom, rightBottom, rightTop, leftTop, leftBottom };
            
            _edge.SetPoints(points);
        }
    }
}
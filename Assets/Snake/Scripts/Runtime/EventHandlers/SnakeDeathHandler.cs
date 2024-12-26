using Runtime.SnakeScripts;
using UnityEngine;

namespace Runtime.EventHandlers
{
    public class SnakeDeathHandler : MonoBehaviour
    {
        [SerializeField] private Snake _snake;
        [SerializeField] private GameObject _deathPanel;

        private void Start()
        {
            _deathPanel.SetActive(false);
        }
        
        private void OnEnable()
        {
            _snake.Died += OnSnakeDied;
        }

        private void OnDisable()
        {
            _snake.Died -= OnSnakeDied;
        }

        private void OnSnakeDied()
        {
            _deathPanel.SetActive(true);
        }
    }
}
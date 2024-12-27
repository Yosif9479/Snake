using UnityEngine;
using UnityEngine.Events;

namespace Runtime.FruitScripts
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class Fruit : MonoBehaviour
    {
        public static event UnityAction Eaten;
        public void Eat()
        {
            Destroy(gameObject);
            Eaten?.Invoke();
        }
    }
}
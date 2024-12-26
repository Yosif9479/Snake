using UnityEngine;

namespace Runtime.FruitScripts
{
    public class Fruit : MonoBehaviour
    {
        public void Eat()
        {
            Destroy(gameObject);
        }
    }
}
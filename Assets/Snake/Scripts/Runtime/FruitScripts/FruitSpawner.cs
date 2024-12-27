using UnityEngine;

namespace Runtime.FruitScripts
{
    public class FruitSpawner : MonoBehaviour
    {
        [SerializeField] private Fruit[] _fruitPrefabs;

        private Vector2 _fieldFirstCorner;
        private Vector2 _fieldLastCorner;
        private Camera _camera;

        private void Awake()
        {
            _camera = Camera.main;
            InitField();
        }

        private void Start()
        {
            SpawnFruit();
        }
        
        private void OnEnable() => Fruit.Eaten += OnFruitEaten;
        
        private void OnDisable() => Fruit.Eaten -= OnFruitEaten;

        private void OnFruitEaten()
        {
            SpawnFruit();
        }

        private void SpawnFruit()
        {
            Fruit prefab = GetRandomFruitPrefab();
            Vector2 position = GetRandomSpawnPosition(prefab.GetComponent<SpriteRenderer>());
            Instantiate(prefab, position, Quaternion.identity);
        }

        private Fruit GetRandomFruitPrefab()
        {
            int index = Random.Range(0, _fruitPrefabs.Length);

            return _fruitPrefabs[index];
        }

        private Vector2 GetRandomSpawnPosition(SpriteRenderer prefabSprite)
        {
            float prefabDiameter = Mathf.Min(prefabSprite.bounds.size.x, prefabSprite.bounds.size.y);
            
            while (true)
            {
                Vector2 position = GeneratePosition();
                Collider2D other = Physics2D.OverlapCircle(position, prefabDiameter);
                if (other is not null) continue;
                return position;
            }
        }

        private Vector2 GeneratePosition()
        {
            float x = Random.Range(_fieldFirstCorner.x, _fieldLastCorner.x);
            float y = Random.Range(_fieldFirstCorner.y, _fieldLastCorner.y);
            return new Vector2(x, y);
        }

        private void InitField()
        {
            _fieldFirstCorner = _camera.ScreenToWorldPoint(new Vector3(0, 0, 0));
            _fieldLastCorner = _camera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));
        }
    }
}
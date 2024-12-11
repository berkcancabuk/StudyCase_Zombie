using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Controllers;
using Data;
using UnityEngine;

namespace Managers
{
    public class SpawnManager : MonoBehaviour
    {
        private LevelData _currentLevelData;
        public EnemyData enemyData;
        [SerializeField] private Transform player;
        [SerializeField] private float minSpawnDistance = 10f;
        [SerializeField] private float maxSpawnDistance = 15f;
        [SerializeField] private float forwardViewAngle = 90f;

        [SerializeField] private int initialPoolSize = 20; // Başlangıç havuz boyutu
        [SerializeField] private int maxPoolSize = 50; // Maksimum havuz boyutu
        private Queue<GameObject> enemyPool;
        private List<GameObject> activeEnemies;
        private Coroutine spawnCoroutine;
        private int _totalEnemiesInWave;
        private int _remainingEnemies;
        private int _killedEnemies;

        private void Awake()
        {
            enemyPool = new Queue<GameObject>();
            activeEnemies = new List<GameObject>();
            InitializePool();
        }

        private void InitializePool()
        {
            // Başlangıçta belirli sayıda düşman oluştur
            for (int i = 0; i < initialPoolSize; i++)
            {
                CreateNewEnemy();
            }
        }

        private void CreateNewEnemy()
        {
            if (enemyPool.Count < maxPoolSize)
            {
                GameObject enemy = Instantiate(enemyData.prefab, transform);
                var enemyController = enemy.GetComponent<EnemyController>();
                enemyController.moveSpeed = enemyData.speed;
                enemyController.health = enemyData.health;
                enemyController.damage = enemyData.damage;
                enemy.SetActive(false);
                enemyPool.Enqueue(enemy);
            }
        }

        public void SetLevelParameters(LevelData levelData)
        {
            _currentLevelData = levelData;
            _totalEnemiesInWave = levelData.enemyCount;
            _remainingEnemies = _totalEnemiesInWave;
            _killedEnemies = 0;
        
            UIManager.Instance.UpdateWaveCount(_remainingEnemies);
        }
        
        public void OnEnemyKilled()
        {
            _killedEnemies++;
            _remainingEnemies--;
        
            // UI'ı güncelle
            var uiManager = UIManager.Instance;
            var gameManager = GameManager.Instance;
            uiManager.UpdateWaveCount(_remainingEnemies);

            // Tüm düşmanlar öldüyse
            if (_killedEnemies >= _totalEnemiesInWave)
            {
                // Wave tamamlandı, oyunu bitir
                gameManager.StateMachine.ChangeState(new GameWonState(gameManager,uiManager));
            }
        }

        public void StartSpawning()
        {
            if (spawnCoroutine != null)
            {
                StopCoroutine(spawnCoroutine);
            }

            spawnCoroutine = StartCoroutine(SpawnEnemies());
        }

        public void StopSpawning()
        {
            if (spawnCoroutine != null)
            {
                StopCoroutine(spawnCoroutine);
                spawnCoroutine = null;
            }
        }

        private IEnumerator SpawnEnemies()
        {
            for (int i = 0; i < _currentLevelData.enemyCount; i++)
            {
                SpawnEnemy();
                yield return new WaitForSeconds(_currentLevelData.spawnInterval);
            }
        }

        private void SpawnEnemy()
        {
            // Havuzda düşman yoksa ve maksimum sayıya ulaşılmadıysa yeni düşman oluştur
            if (enemyPool.Count == 0 && activeEnemies.Count < maxPoolSize)
            {
                CreateNewEnemy();
            }

            // Havuzdan düşman al
            if (enemyPool.Count > 0)
            {
                GameObject enemy = enemyPool.Dequeue();
                enemy.transform.position = GetRandomSpawnPosition();
                enemy.SetActive(true);
                enemy.GetComponent<EnemyController>().enabled = true;
                activeEnemies.Add(enemy);
            }
        }

        public void ReturnEnemyToPool(GameObject enemy)
        {
            if (enemy != null)
            {
                enemy.SetActive(false);
                activeEnemies.Remove(enemy);
                enemyPool.Enqueue(enemy);
            }
        }

        public void DeactivateAllEnemies()
        {
            // Tüm aktif düşmanları havuza geri döndür
            foreach (var enemy in activeEnemies.ToList())
            {
                ReturnEnemyToPool(enemy);
            }

            activeEnemies.Clear();
        }

        private void OnDisable()
        {
            StopSpawning();
        }

        // görüş alanı dışında bir yerde yaratılıyor !! 
        private Vector3 GetRandomSpawnPosition()
        {
            Vector3 playerPosition = player.position;
            Vector3 playerForward = player.forward;

            float randomAngle;
            Vector3 spawnPosition;
            float angle;

            do
            {
                randomAngle = Random.Range(0f, 360f);
                float randomDistance = Random.Range(minSpawnDistance, maxSpawnDistance);

                float rad = randomAngle * Mathf.Deg2Rad;
                Vector3 direction = new Vector3(Mathf.Sin(rad), 0f, Mathf.Cos(rad));
                spawnPosition = playerPosition + direction * randomDistance;

                Vector3 directionToSpawn = (spawnPosition - playerPosition).normalized;
                angle = Vector3.Angle(playerForward, directionToSpawn);
            } while (angle < forwardViewAngle / 2);

            return spawnPosition;
        }

        // private void OnDrawGizmosSelected()
        // {
        //     if (player != null)
        //     {
        //         Gizmos.color = Color.yellow;
        //         Gizmos.DrawWireSphere(player.position, minSpawnDistance);
        //         Gizmos.DrawWireSphere(player.position, maxSpawnDistance);
        //
        //         Gizmos.color = Color.red;
        //         Vector3 leftDir = Quaternion.Euler(0, -forwardViewAngle / 2, 0) * player.forward;
        //         Vector3 rightDir = Quaternion.Euler(0, forwardViewAngle / 2, 0) * player.forward;
        //         Gizmos.DrawLine(player.position, player.position + leftDir * maxSpawnDistance);
        //         Gizmos.DrawLine(player.position, player.position + rightDir * maxSpawnDistance);
        //     }
        // }
    }
}
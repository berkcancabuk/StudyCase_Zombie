using System.Collections.Generic;
using UnityEngine;

namespace Controllers
{
    public class BulletPool : MonoBehaviour
    {
        [SerializeField] private GameObject bulletPrefab;
        [SerializeField] private int poolSize = 20;

        private Queue<GameObject> bulletPool;

        private void Awake()
        {
            bulletPool = new Queue<GameObject>();
            CreatePool();
        }

        private void CreatePool()
        {
            for (int i = 0; i < poolSize; i++)
            {
                GameObject bullet = Instantiate(bulletPrefab,transform);
                bullet.GetComponent<Bullet>().SetPool(this);
                bullet.SetActive(false);
                bulletPool.Enqueue(bullet);
            }
        }

        public GameObject GetBullet()
        {
            if (bulletPool.Count == 0)
            {
                CreatePool();
            }
            return bulletPool.Dequeue();
        }

        public void ReturnBullet(GameObject bullet)
        {
            bullet.SetActive(false);
            bulletPool.Enqueue(bullet);
        }
    }
}
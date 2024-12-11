using System.Collections;
using States.Interface;
using UnityEngine;

namespace Controllers
{
    public class Bullet : MonoBehaviour
    {
        [SerializeField] private float lifetime = 3f;
        [SerializeField] private int damage = 10;
        private BulletPool pool;

        private void OnEnable()
        {
            StartCoroutine(DeactivateAfterTime());
        }

        private IEnumerator DeactivateAfterTime()
        {
            yield return new WaitForSeconds(lifetime);
            pool.ReturnBullet(gameObject);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out IDamageable damageable))
            {
                damageable.TakeDamage(damage);
            }
            
            // poola geri ekledim
            pool.ReturnBullet(gameObject);
        }

        public void SetPool(BulletPool bulletPool)
        {
            pool = bulletPool;
        }
    }
}


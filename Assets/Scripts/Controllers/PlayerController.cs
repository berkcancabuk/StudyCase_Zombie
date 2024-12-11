// PlayerController.cs

using UnityEngine;
using System;
using Managers;
using States.Interface;
using UnityEngine.Serialization;

namespace Controllers
{
    
    public class PlayerController : MonoBehaviour, IDamageable
    {
        private static readonly int Die1 = Animator.StringToHash("Die");
        private static readonly int Horizontal = Animator.StringToHash("Horizontal");
        private static readonly int Vertical = Animator.StringToHash("Vertical");

        [FormerlySerializedAs("_rigidbody")] [Header("Movement Components")] [SerializeField]
        private Rigidbody rigidbody;

        [FormerlySerializedAs("_joystick")] [SerializeField]
        private FixedJoystick joystick;

        [SerializeField] private Transform playerChildTransform;
        [SerializeField] private float moveSpeed;
        [SerializeField] private Animator animator;

        [Header("Player Stats")] [SerializeField]
        private int maxHealth = 100;

        [SerializeField] private float invincibilityTime = 1f;

        [Header("Auto Shooting Settings")] [SerializeField]
        private float fireRate = 0.25f;
        [SerializeField] private ParticleSystem muzzleFlash; // Yeni eklenen satır
        [SerializeField] private float bulletSpeed = 20f;
        [SerializeField] private Transform firePoint;
        [SerializeField] private BulletPool bulletPool;

        private float _horizontal;
        private float _vertical;
        public float _currentHealth;
        private float _lastDamageTime;
        private float _lastAttackTime;
        private float _nextFireTime;

        [SerializeField] private float detectionRadius = 10f;
        [SerializeField] private LayerMask enemyLayer;
        [SerializeField] private float targetSwitchDelay = 0.5f;
        [SerializeField] private float rotationSpeed = 5f;

        private Transform _currentTarget;
        private Transform _nearestEnemy;
        private bool _isEnemyInRange = false;
        private float _lastTargetSwitchTime;




        private void Start()
        {
            _currentHealth = maxHealth;
        }

        private void Update()
        {
            if (_currentHealth <= 0) return;

            GetMovementInput();
            FindNearestEnemy();
    
            if (_isEnemyInRange && _currentTarget != null)
            {
                Vector3 directionToEnemy = (_currentTarget.position - transform.position).normalized;
                directionToEnemy.y = 0;
        
                Quaternion targetRotation = Quaternion.LookRotation(directionToEnemy);
                Quaternion currentYRotation = Quaternion.Euler(0, playerChildTransform.rotation.eulerAngles.y, 0);
                
                playerChildTransform.rotation = Quaternion.Slerp(
                    currentYRotation, 
                    targetRotation, 
                    rotationSpeed * Time.deltaTime
                );
                
                float angleToTarget = Quaternion.Angle(playerChildTransform.rotation, targetRotation);
                if (angleToTarget < 30f)
                {
                    AutoFire();
                }
            }
            else
            {
                SetRotation();
            }
    
            UpdateAnimator();
        }

        private void UpdateAnimator()
        {
            Vector3 input = new Vector3(_horizontal, 0f, _vertical);
            Vector3 localInput = playerChildTransform.InverseTransformDirection(input);
            animator.SetFloat(Horizontal, localInput.x);
            animator.SetFloat(Vertical, localInput.z);
        }

        private void AutoFire()
        {
            if (Time.time >= _nextFireTime)
            {
                FireBullet();
                _nextFireTime = Time.time + fireRate;
            }
        }

        private void FireBullet()
        {
            if (bulletPool == null) return;

            GameObject bullet = bulletPool.GetBullet();

            if (bullet != null)
            {
                bullet.transform.position = firePoint.position;
                // bullet.transform.rotation = firePoint.localRotation;
                bullet.SetActive(true);
                if (bullet.TryGetComponent(out Rigidbody bulletRb))
                {
                    bulletRb.velocity = firePoint.forward * bulletSpeed;
                }
                if (muzzleFlash != null)
                {
                    muzzleFlash.Play();
                }
                AudioManager.Instance.Play(SoundType.PlayerAttack);
            }
        }

        private void FindNearestEnemy()
        {
            Collider[] enemies = Physics.OverlapSphere(transform.position, detectionRadius, enemyLayer);

            float nearestDistance = float.MaxValue;
            _nearestEnemy = null;

            foreach (var enemy in enemies)
            {
                float distance = Vector3.Distance(transform.position, enemy.transform.position);
                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    _nearestEnemy = enemy.transform;
                }
            }

            if (_nearestEnemy != null)
            {
                if (_currentTarget == null) 
                {
                    _currentTarget = _nearestEnemy;
                    _lastTargetSwitchTime = Time.time;
                }
                else if (Time.time - _lastTargetSwitchTime >= targetSwitchDelay)
                {
                    float currentTargetDistance = Vector3.Distance(transform.position, _currentTarget.position);
                    float nearestEnemyDistance = Vector3.Distance(transform.position, _nearestEnemy.position);

                    if (nearestEnemyDistance < currentTargetDistance * 0.8f)
                    {
                        _currentTarget = _nearestEnemy;
                        _lastTargetSwitchTime = Time.time;
                    }
                }
            }
            else
            {
                _currentTarget = null;
            }

            _isEnemyInRange = _currentTarget != null;
        }

        private void FixedUpdate()
        {
            if (_currentHealth <= 0) return;

            SetMovement();
            SetRotation();
        }

        private void GetMovementInput()
        {
            _horizontal = joystick.Horizontal;
            _vertical = joystick.Vertical;
        }

        private void SetMovement()
        {
            rigidbody.velocity = GetNewVelocity();
        }

        private void SetRotation()
        {
            if (!_isEnemyInRange && (_horizontal != 0 || _vertical != 0))
            {
                playerChildTransform.rotation = Quaternion.LookRotation(GetNewVelocity());
            }
        }

        private Vector3 GetNewVelocity()
        {
            return new Vector3(_horizontal, rigidbody.velocity.y, _vertical) * (moveSpeed * Time.fixedDeltaTime);
        }

        public void TakeDamage(int damageAmount)
        {
            if (Time.time - _lastDamageTime < invincibilityTime) return;

            _currentHealth -= damageAmount;
            _lastDamageTime = Time.time;

            if (_currentHealth <= 0)
            {
                Die();
            }
        }

        private void Die()
        {
            animator.SetTrigger(Die1);
            enabled = false;
            OnDeath();
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, detectionRadius);
        
            if (_currentTarget != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(transform.position, _currentTarget.position);
            }
        }
        
        private void OnDeath()
        {
            GameManager.Instance.OnPlayerDeath();
            
        }

        public void ResetPlayer()
        {
            animator.Rebind();
            animator.Update(0f);
            _currentHealth = maxHealth;
        }
    }
}
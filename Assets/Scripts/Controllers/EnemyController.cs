using System;
using Managers;
using States.Interface;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace Controllers
{
    public class EnemyController : MonoBehaviour ,IDamageable
    {
        [Header("Movement Settings")]
        [Tooltip("Desired stopping distance from player")]
        public float attackStopDistance = 2f;

        [Tooltip("Base movement speed")]
        [Range(1f, 5f)]
        public float moveSpeed = 2f;

        [Header("Combat Settings")]
        public float damage = 10f;
        public float attackCooldown = 1f;
        public int health = 10;
        public int currentHealth;

        [Header("References")]
        public NavMeshAgent navMeshAgent;
        public Transform player;
        public Animator animator;
        private Collider enemyCollider;
        [SerializeField] private ParticleSystem bloodEffect;
        
        private float _updateInterval = 0.2f;
        private float _nextUpdateTime;
        private float _lastAttackTime;

        void Start()
        {
            
            enemyCollider = GetComponent<Collider>();
            navMeshAgent = GetComponent<NavMeshAgent>();
            player = GameObject.FindGameObjectWithTag("Player")?.transform;
            animator = GetComponent<Animator>();

            navMeshAgent.speed = moveSpeed;
            navMeshAgent.stoppingDistance = attackStopDistance;
            navMeshAgent.angularSpeed = 360f;

            ConfigureAnimationSync();
            currentHealth = health;;
        }

        void ConfigureAnimationSync()
        {
            if (animator != null)
            {
                // Match animation speed with NavMeshAgent speed
                animator.speed = moveSpeed*2;
            }
        }

        void Update()
        {
            if (player == null) return;

            if (Time.time >= _nextUpdateTime)
            {
                _nextUpdateTime = Time.time + _updateInterval;
                UpdateEnemyMovement();
            }
        }

        void UpdateEnemyMovement()
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);

            if (distanceToPlayer > attackStopDistance)
            {
                navMeshAgent.SetDestination(player.position);
                UpdateMovementAnimation(true);
            }
            else
            {
                navMeshAgent.SetDestination(transform.position);
                UpdateMovementAnimation(false);
                PerformAttack();
            }
        }

        void UpdateMovementAnimation(bool isMoving)
        {
            if (animator != null)
            {
                animator.SetBool("walk", isMoving);
            }
        }

        void PerformAttack()
        {
            if (Time.time - _lastAttackTime >= attackCooldown)
            {
                _lastAttackTime = Time.time;

                // Trigger attack animation
                if (animator != null)
                {
                    animator.SetBool("attack",true);
                }

                AudioManager.Instance.Play(SoundType.ZombieAttack);
                // Damage player
                var playerController = player.GetComponent<PlayerController>();
                if (playerController != null)
                {
                    playerController.TakeDamage((int)damage);
                }
                
            }
        }

        public void TakeDamage(int damageAmount)
        {
            currentHealth -= damageAmount;

            if (bloodEffect != null)
            {
                bloodEffect.Play();
            }
            if (currentHealth <= 0)
            {
                Die();
            }
        }

        void Die()
        {
            if (animator != null)
            {
                animator.SetTrigger("die");
            }

            navMeshAgent.enabled = false;

            if (TryGetComponent<Collider>(out var collider))
            {
                collider.enabled = false;
            }

            enabled = false;

            GameManager.Instance.IncrementDefeatedEnemies();
            if (GameManager.Instance.spawnManager != null)
            {
                GameManager.Instance.spawnManager.ReturnEnemyToPool(gameObject);
            }
            
            GameManager.Instance.spawnManager.OnEnemyKilled();
        }
        
        private void ResetEnemy()
        {
            enabled = true;

            if (navMeshAgent != null)
            {
                navMeshAgent.enabled = true;
                navMeshAgent.ResetPath();
                navMeshAgent.isStopped = false;
            }

            if (enemyCollider != null)
            {
                enemyCollider.enabled = true;
            }

            if (animator != null)
            {
                animator.Rebind();
                animator.Update(0f);
                animator.SetTrigger("walk");
            }
        }

        private void OnEnable()
        {
            ResetEnemy();
        }
    }
}

using System.Collections.Generic;
using _Scripts.Managers.Pooling;
using _Scripts.Units.Blocks.Direction;
using UnityEngine;

namespace _Scripts.Units.Player {
    public class DetectEnemy : MonoBehaviour
    {
        [SerializeField] private List<GameObject> enemies = new List<GameObject>();
        private BoxCollider boxCollider;

        private void Awake()
        {
            boxCollider = GetComponent<BoxCollider>();
            boxCollider.isTrigger = true; // Ensure that the collider works as a trigger
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Enemy") && !enemies.Contains(other.gameObject))
            {
                enemies.Add(other.gameObject);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Enemy"))
            {
                enemies.Remove(other.gameObject);
            }
        }

        public void AttackRunnerEnemy()
        {
            if (enemies.Count == 0)
                return;

            var enemy = enemies[0];
            enemies.RemoveAt(0);  // Remove the attacked enemy from the list
            enemy.GetComponent<DirectionBlock>().BlockHit();  // Apply the hit logic to the enemy
            
            var particle = ParticlePool.pool.GetParticle();  // Get a particle effect from the pool
            particle.transform.position = enemy.transform.position;  // Set the particle position to the enemy's position
        }

        public bool IsEnemyDetected()
        {
            return enemies.Count > 0;
        }
    }
}
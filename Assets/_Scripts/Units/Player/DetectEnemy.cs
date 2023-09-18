using _Scripts.Managers.Blocks.Runner;
using System.Collections.Generic;
using UnityEngine;

namespace _Scripts.Units.Player {
    public class DetectEnemy : MonoBehaviour
    {
        [SerializeField] private bool enemyDetected = false;
        [SerializeField] private List<GameObject> enemies = new List<GameObject>();
    
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Enemy"))
            {
                enemyDetected = true;
                enemies.Add(other.gameObject);
            }
        }
    
        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Enemy"))
            {
                enemyDetected = false;
                enemies.Remove(other.gameObject);
            }
        }
        
        public void AttackRunnerEnemy(){
            if (enemies.Count == 0)
                return;
            
            var enemy = enemies[0];
            enemies.Remove(enemy);
            enemy.GetComponent<RunnerBlock>().BlockHit();
            
        }
    }
}

using _Scripts.Managers.Blocks.Runner;
using UnityEngine;

namespace _Scripts.Managers.Spawners {
    public class RunnerSpawner : MonoBehaviour
    {
        [Header("Runner Settings")]
        [SerializeField] private GameObject blockPrefab;
        [SerializeField] private float spawnRate = 1f;
        private float nextSpawn = 0f;
        private RunnerBlock block;
        
        [SerializeField] private Transform topSpawnPoint;
        [SerializeField] private Transform bottomSpawnPoint;
        
        private void Start(){
            block = blockPrefab.GetComponentInChildren<RunnerBlock>();
        }
        
        private void Update(){
            if(!(Time.time > nextSpawn))
                return;
            nextSpawn = Time.time + spawnRate;
            SpawnBlock();
        }
        
        private void SpawnBlock() {
            var spawnPosition = Random.Range(0, 2) == 0 ? topSpawnPoint.position : bottomSpawnPoint.position;
            RunnerBlock newblock = Instantiate(block, spawnPosition, Quaternion.identity);
        }
    }
}

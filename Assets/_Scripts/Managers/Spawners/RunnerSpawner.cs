using _Scripts.Units.Blocks.Direction;
using UnityEngine;

namespace _Scripts.Managers.Spawners {
    public class RunnerSpawner : MonoBehaviour
    {
        [Header("Runner Settings")]
        [SerializeField] private GameObject blockPrefab;
        [SerializeField] private float spawnRate = 1f;
        [SerializeField] private Vector3 direction = Vector3.left;
        private float nextSpawn = 0f;
        private DirectionBlock block;
        
        [SerializeField] private Transform topSpawnPoint;
        [SerializeField] private Transform bottomSpawnPoint;
        
        private void Start(){
            block = blockPrefab.GetComponentInChildren<DirectionBlock>();
        }
        
        private void Update(){
            // if(!(Time.time > nextSpawn))
            //     return;
            // nextSpawn = Time.time + spawnRate;
            // SpawnBlock();
        }
        
        public void SpawnBlock(int pos){
            var spawnPosition = pos == 0 ? topSpawnPoint.position : bottomSpawnPoint.position;
            DirectionBlock newBlock = Instantiate(block, spawnPosition, Quaternion.identity);
            newBlock.SetDirection(direction);
        }
        private void OnDrawGizmos(){
            Gizmos.color = Color.red;
            Gizmos.DrawRay(topSpawnPoint.position, direction * 1);
            Gizmos.DrawRay(bottomSpawnPoint.position, direction * 1);
        }
    }
}

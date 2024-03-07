using _Scripts.Units.Blocks.Dodge;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace _Scripts.Managers.Spawners {
    public class DodgeSpawner : MonoBehaviour
    {
        [Header("Spawner Settings")]
        [SerializeField] private GameObject blockPrefab;
        [SerializeField] private float spawnRate = 1f;
        private float nextSpawn = 0f;
        private DodgeBlock dodgeBlock;

        [SerializeField] private Transform[] spawnPoints;
        [SerializeField] private Transform[] endPoints;

        Vector3 difference;
        private void Start(){
            dodgeBlock = blockPrefab.GetComponentInChildren<DodgeBlock>();
            
            // Get distance difference between blockprefab and block
            var blockPrefabPosition = blockPrefab.transform.position;
            var blockPosition = dodgeBlock.transform.position;
            difference = blockPrefabPosition - blockPosition;
            difference = new Vector3(0, Mathf.Abs(difference.y), 0);

        }

        private void Update(){
            // if(!(Time.time > nextSpawn))
            //     return;
            // nextSpawn = Time.time + spawnRate;
            // int spawnPositionIndex = Random.Range(0, spawnPoints.Length);
            // SpawnBlock(spawnPositionIndex);
        }
        
        public void SpawnBlock(int spawnPositionIndex = 2) {
            var newPosition = spawnPoints[spawnPositionIndex].position + difference;
            DodgeBlock newblock = Instantiate(dodgeBlock, newPosition, Quaternion.identity);
            newblock.SetTargetPosition(endPoints[spawnPositionIndex].position + difference);
        }
    }
}

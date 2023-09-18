using _Scripts.Units.Player;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Scripts.Managers.Blocks.Runner {
    public class RunnerBlock : MonoBehaviour
    {
        #region Private Variables
        private Rigidbody rb;
        private BoxCollider col;
        #endregion
        
        #region Unity Methods
        private void Awake(){
            rb = GetComponent<Rigidbody>();
            col = GetComponent<BoxCollider>();
        }
        
        private void Start(){
            StartBlock();
        }
        
        private void Update(){
            MoveBlock();
        }
        
        private void OnTriggerEnter(Collider other){
            if (other.CompareTag("Player")){
                var health = other.GetComponent<HealthController>();
                health.TakeDamage(10);
                print("Hit!" + other.name);
            }
        }
        #endregion
        
        #region Block
        [Header("Block Settings")]
        [SerializeField] private float speed = 5f;
        [SerializeField] private float damage = 10f;

        private void StartBlock(){
            rb.isKinematic = true;
            
            col.isTrigger = true;
        }
        
        private void MoveBlock(){
            var step = speed * Time.deltaTime;
            var direction = Vector3.left;
            
            transform.position += direction * (step * 2);
        }
        
        public void BlockHit(){
            Destroy(gameObject);
        }
        #endregion
    }
}

using _Scripts.Units.Player;
using UnityEngine;

namespace _Scripts.Managers.Blocks.Direction {
    public class DirectionBlock : MonoBehaviour
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
        [SerializeField] private Vector3 direction = Vector3.left;

        private void StartBlock(){
            rb.isKinematic = true;
            
            col.isTrigger = true;
        }
        
        private void MoveBlock(){
            var step = speed * Time.deltaTime;
            
            transform.position += direction * (step * 2);
        }
        
        public void BlockHit(){
            Destroy(gameObject);
        }
        
        public void SetDirection(Vector3 dir){
            direction = dir;
        }
        
        public void SetSpeed(float spd){
            speed = spd;
        }
        
        public void SetDamage(float dmg){
            damage = dmg;
        }
        #endregion
    }
}

using _Scripts.Units.Player;
using UnityEngine;

namespace _Scripts.Units.Blocks.Dodge {
    public class DodgeBlock : MonoBehaviour
    {
        #region Private Variables
        private BoxCollider col;
        private Rigidbody rb;
        #endregion
        
        #region Unity Methods

        private void Awake()
        {
            col = GetComponentInChildren<BoxCollider>();
            rb = GetComponentInChildren<Rigidbody>();
        }
        
        private void Start()
        {
            StartBlock();
        }

        private void Update(){
            MoveBlock();
        }
        
        private void OnTriggerEnter(Collider other){
            if (other.CompareTag("Player")){
                var health = other.GetComponent<HealthController>();
                health.TakeDamage(10);
            }
        
            print("Hit!" + other.name);
        }
        #endregion

        #region Block
        [Header("Block Settings")]
        [SerializeField] private float _speed = 5f;

        private Vector3 targetPosition;
        private Vector3 startPosition;
        
        [SerializeField] private bool _jumpable = true;
        [SerializeField] private bool _returnable = false;
        
        [SerializeField] private float _damage = 10;

        private void StartBlock(){
            startPosition = transform.position;
        }
        
        public void SetTargetPosition(Vector3 targetPosition){
            this.targetPosition = targetPosition;
        }

        private void MoveBlock(){
            var step = _speed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, step);
            
            if (Vector3.Distance(transform.position, targetPosition) < 0.001f){
                Destroy(this.gameObject);
            }
        }
        #endregion
    }
}

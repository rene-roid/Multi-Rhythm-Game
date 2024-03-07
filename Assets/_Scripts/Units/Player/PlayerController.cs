using rene_roid;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

public enum PlayerMode{
    Dodge,
    Runner
}
namespace _Scripts.Units.Player {    
    public class PlayerController : MonoBehaviour
    {
        #region Internal Variables
        [Header("Player Settings")]
        [SerializeField] private PlayerMode playerMode = PlayerMode.Dodge;
        
        private BoxCollider col;
        private Rigidbody rb;
        
        private int fixedFrame = 0;
        #endregion

        #region Unity Methods
        private void Start()
        {
            col = GetComponent<BoxCollider>();
            rb = GetComponent<Rigidbody>();

            switch (playerMode){
                case PlayerMode.Dodge:
                    DodgeStart();
                    break;
                case PlayerMode.Runner:
                    RunnerStart();
                    break;
            }
        }

        private void Update()
        {
            InputController();
        }

        private void FixedUpdate(){
            fixedFrame++;
            
            switch (playerMode){
                case PlayerMode.Dodge:
                    DodgeFixedUpdate();
                    break;
                case PlayerMode.Runner:
                    RunnerFixedUpdate();
                    break;
            }
        }
        #endregion

        public void SwitchPlayerMode(PlayerMode mode){
            playerMode = mode;
            
            switch (playerMode){
                case PlayerMode.Dodge:
                    DodgeStart();
                    break;
                case PlayerMode.Runner:
                    RunnerStart();
                    break;
            }
        }
        
        private void InputController()
        {
            switch (playerMode){
                case PlayerMode.Dodge:
                    DodgeInput();
                    break;
                case PlayerMode.Runner:
                    RunnerInput();
                    break;
            }
        }

        #region Dodge
        [Header("Dodge Settings")]
        public Vector3[] dodgePositions;
        [SerializeField] private float dodgeSpeed = 5f;

        private int indexPosition = 2;
        private int lastIndex = 2;
        
        private float clock = 0f;
        private bool isDodging = false;

        private void DodgeStart(){
            transform.position = dodgePositions[indexPosition];
        }

        private void DodgeFixedUpdate()
        {
            if (!isDodging)
            {
                transform.position = dodgePositions[indexPosition];
            }
        }

        private void DodgeInput(){
            if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
            {
                DodgeMovement(-1);
            }
            if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
            {
                DodgeMovement(1);
            }
        }

        private void DodgeMovement(int direction){
            Vector3 pos = dodgePositions[indexPosition];
            
            isDodging = true;
            lastIndex = indexPosition;
            indexPosition += direction;
            
            indexPosition = Mathf.Clamp(indexPosition, 0, dodgePositions.Length - 1);
            pos = dodgePositions[lastIndex];
            transform.position = pos;

            StartCoroutine(move(dodgePositions[indexPosition]));
            
            IEnumerator move(Vector3 target){
                var curr = transform.position;
                var initPos = curr;
                
                var timer = 0f;

                while (curr != target){
                    if (playerMode == PlayerMode.Runner) yield break;

                    curr = Vector3.Lerp(initPos, target, timer / dodgeSpeed);
                    timer += Time.deltaTime;
                    transform.position = curr;
                    
                    yield return null;
                }
                
                isDodging = false;
            }
        }
        
        #endregion

        #region Runner
        [Header("Runner Settings")]
        [SerializeField] private Vector3 runnerPosition;
        [SerializeField] private float jumpDistance = 5f;
        [SerializeField] private float jumpSpeed = .1f;
        [SerializeField] private float jumpApexTime = .05f;
        [SerializeField] private float fallForce = 5f;

        [SerializeField] private GameObject enemyDetectors;
        private DetectEnemy topEnemyDetector;
        private DetectEnemy bottomEnemyDetector;
        
        private Vector3 groundPosition;
        private Vector3 ceilingPosition;
        
        private bool jumping = false;
        private bool grounded = true;
        private bool falling = false;
        
        private void RunnerStart(){
            transform.position = runnerPosition;
            
            groundPosition = transform.position;
            ceilingPosition = groundPosition + Vector3.up * jumpDistance;
            
            topEnemyDetector = enemyDetectors.transform.GetChild(0).GetComponent<DetectEnemy>();
            bottomEnemyDetector = enemyDetectors.transform.GetChild(1).GetComponent<DetectEnemy>();
        }
        
        private void RunnerInput(){
            if (JumpInput() && grounded && !jumping && !falling){
                print("jump");
                topEnemyDetector.AttackRunnerEnemy();
                RunnerMovement(1);
            }
            
            if (FallInput() && !jumping && !grounded){
                print("fall");
                bottomEnemyDetector.AttackRunnerEnemy();
                RunnerMovement(-1);
            } else if(FallInput() && grounded){
                bottomEnemyDetector.AttackRunnerEnemy();
            }

            bool JumpInput(){
                if (Input.GetKeyDown(KeyCode.Space)) return true;
                if (Input.GetKeyDown(KeyCode.UpArrow)) return true;
                if (Input.GetKeyDown(KeyCode.W)) return true;
                return false;
            }
            
            bool FallInput(){
                if (Input.GetKeyDown(KeyCode.DownArrow)) return true;
                if (Input.GetKeyDown(KeyCode.S)) return true;
                return false;
            }
        }

        public void DoubleJump(){
            topEnemyDetector.AttackRunnerEnemy();
            RunnerMovement(-1);
            RunnerMovement(1);
        }
        
        private void RunnerMovement(int direction = 0){
            if (direction == 1) StartCoroutine(Jump());
            else if(direction==-1){
                //StopCoroutine(jump());
                StartCoroutine(Fall());
            }
            
            IEnumerator Jump(){
                jumping = true;
                grounded = false;
                
                transform.position = groundPosition;
                
                var curr = transform.position;
                var initPos = groundPosition;
                var target = ceilingPosition;
                
                var timer = 0f;
                
                while (curr != target){
                    curr = Vector3.Lerp(initPos, target, timer / jumpSpeed);
                    timer += Time.deltaTime;
                    transform.position = curr;
                    
                    yield return null;
                }
                
                jumping  = false;
                
                yield return Helpers.GetWait(jumpApexTime);
                
                StartCoroutine(Fall());
            }
            
            IEnumerator Fall(){
                if (falling || grounded || playerMode == PlayerMode.Dodge) yield break;
                
                falling = true;
                
                transform.position = ceilingPosition;
                
                var curr = transform.position;
                var initPos = ceilingPosition;
                var target = groundPosition;
                
                var timer = 0f;
                
                while (curr != target){
                    curr = Vector3.Lerp(initPos, target, timer / fallForce);
                    timer += Time.deltaTime;
                    transform.position = curr;
                    
                    yield return null;
                }
                
                grounded = true;
                falling = false;
            }
        }

        private void RunnerFixedUpdate(){
            if (grounded)
            {
                transform.position = groundPosition;
            }
        }
        
        #endregion
    }
}

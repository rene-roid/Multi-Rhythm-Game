using rene_roid;
using System;
using System.Collections;
using _Scripts.Managers.Input;
using _Scripts.Managers.Pooling;
using UnityEngine;
using UnityEngine.Serialization;

public enum PlayerMode{
    Dodge,
    Runner
}
namespace _Scripts.Units.Player {
    [RequireComponent(typeof(PlayerInput))]
    public class PlayerController : MonoBehaviour
    {
        #region Internal Variables
        [Header("Player Settings")]
        [SerializeField] private PlayerMode playerMode = PlayerMode.Dodge;
        
        private BoxCollider col;
        private Rigidbody rb;
        
        private int fixedFrame = 0;
        
        private PlayerInput playerInput;
        private FrameInput frameInput;
        #endregion

        #region Unity Methods

        private void Awake()
        {
            playerInput = GetComponent<PlayerInput>();
        }

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
            if (playerMode == mode) return;
            
            StopAllCoroutines();
            playerMode = mode;
            
            switch (playerMode){
                case PlayerMode.Dodge:
                    SetupDodge();
                    DodgeStart();
                    break;
                case PlayerMode.Runner:
                    SetupRunner();
                    RunnerStart();
                    break;
            }

            void SetupDodge()
            {
                // Reset Runner
                jumping = false;
                grounded = true;
                falling = false;
                
                // Setup Dodge
                transform.position = dodgePositions[indexPosition];
                visual = transform.GetChild(0).gameObject;
                isJumping = false;
            }
            
            void SetupRunner()
            {
                // Reset Dodge
                isDodging = false;
                
                // Setup Runner
                jumping = false;
                grounded = true;
                falling = false;
                transform.position = runnerPosition;
                groundPosition = runnerPosition;
                visual.transform.localPosition = new Vector3(0, -0.5f, 0);
                ceilingPosition = groundPosition + Vector3.up * jumpDistance;
            }
        }
        
        private void InputController()
        {
            frameInput = playerInput.frameInput;
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
        [SerializeField] private float dodgeUpSpeed = .13f;

        private int indexPosition = 2;
        private int lastIndex = 2;
        
        private float clock = 0f;
        private bool isDodging = false;
        public bool isJumping { get => jumping;
            private set => jumping = value;
        }
        private GameObject visual;

        private void DodgeStart(){
            transform.position = dodgePositions[indexPosition];
            visual = transform.GetChild(0).gameObject;
            isJumping = false;
        }

        private void DodgeFixedUpdate()
        {
            if (!isDodging)
            {
                transform.position = dodgePositions[indexPosition];
            }
        }

        private void DodgeInput(){
            if (frameInput.DodgeLeft)
            {
                DodgeMovement(-1);
            }
            if (frameInput.DodgeRight)
            {
                DodgeMovement(1);
            }

            if (frameInput.DodgeJump)
            {
                Jump();
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
        
        private void Jump() {
            if (isJumping) return;
            
            isJumping = true;
            StartCoroutine(JumpRoutine());

            IEnumerator JumpRoutine(){
                var curr = visual.transform.localPosition;
                var initPos = new Vector3(0, -0.5f, 0);
                var target = new Vector3(0, -0.5f + 1, 0);
                
                var timer = 0f;

                while (curr != target){
                    if (playerMode == PlayerMode.Runner) yield break;

                    curr = Vector3.Lerp(initPos, target, timer / dodgeUpSpeed);
                    timer += Time.deltaTime;
                    visual.transform.localPosition = new Vector3(0, curr.y, 0);
                    
                    yield return null;
                }
                
                // yield return Helpers.GetWait(.1f);
                
                curr = visual.transform.localPosition;
                initPos = new Vector3(0, curr.y, 0);
                target = new Vector3(0, -0.5f, 0);
                
                timer = 0f;

                while (curr != target){
                    if (playerMode == PlayerMode.Runner) yield break;

                    curr = Vector3.Lerp(initPos, target, timer / dodgeUpSpeed);
                    timer += Time.deltaTime;
                    visual.transform.localPosition = new Vector3(0, curr.y, 0);
                    
                    yield return null;
                }
                
                isJumping = false;
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
            
            // Reset Jump if Airborne and there is an enemy
            if (JumpInput() && !grounded && topEnemyDetector.IsEnemyDetected()){
                print("reset jump");
                topEnemyDetector.AttackRunnerEnemy();
                RunnerMovement(2);
            }

            if (FallInput() && !grounded){
                print("fall");
                bottomEnemyDetector.AttackRunnerEnemy();
                RunnerMovement(-1);
            } else if(FallInput() && grounded){
                bottomEnemyDetector.AttackRunnerEnemy();
            }

            bool JumpInput(){
                if (frameInput.RunnerUp) return true;
                return false;
            }
            
            bool FallInput(){
                if (frameInput.RunnerDown) return true;
                return false;
            }
        }
        
        private void RunnerMovement(int direction = 0){
            StopAllCoroutines();
            jumping = false;
            falling = false;
            grounded = true;
            
            if (direction == 1) StartCoroutine(Jump());
            else if(direction==-1) StartCoroutine(Fall());
            else if (direction == 2) StartCoroutine(ResetJump());
            
            IEnumerator Jump(){
                jumping = true;
                grounded = false;
                falling = false;
                
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
                jumping = false;
                grounded = false;
                
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

            IEnumerator ResetJump()
            {
                jumping = true;
                grounded = false;
                falling = false;
                
                transform.position = ceilingPosition;
                
                yield return Helpers.GetWait(jumpApexTime);
                
                StartCoroutine(Fall());
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

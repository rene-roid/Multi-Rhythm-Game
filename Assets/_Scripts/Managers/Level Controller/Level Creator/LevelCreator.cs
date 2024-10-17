using System.Collections.Generic;
using _Scripts.Managers.Game;
using _Scripts.Managers.Spawners;
using _Scripts.Units.Blocks.Dodge;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Scripts.Managers.Level_Controller.Level_Creator
{
    public class LevelCreator : MonoBehaviour
    {
        [Header("General Settings")]
        public float levelDuration = 0f;
        public float currentTime = 0f;
        public AudioClip levelMusic;
        public AudioSource audioSource;

        public Button playStopButton;
        public Slider timeSlider;
        public TMP_Text timeText;

        #region Audio
        private void OnAudioLoad()
        {
            levelDuration = levelMusic.length;
            audioSource.clip = levelMusic;

            PauseLevelMusic();
            SetupAudioUI();
        }

        public void PlayStopLevelMusic()
        {
            if (audioSource.isPlaying)
            {
                PauseLevelMusic();
                playStopButton.GetComponentInChildren<TextMeshProUGUI>().text = ">";
            }
            else
            {
                PlayLevelMusic();
                playStopButton.GetComponentInChildren<TextMeshProUGUI>().text = "||";
            }
        }

        private void SetupAudioUI()
        {
            playStopButton.onClick.AddListener(PlayStopLevelMusic);
            timeSlider.onValueChanged.AddListener(SetAudioTime);

            timeSlider.maxValue = levelDuration;
        }

        private void AudioUpdate()
        {
            timeSlider.value = audioSource.time;
            currentTime = audioSource.time;

            timeText.text = $"{Mathf.Floor(currentTime / 60).ToString("00")}:{(currentTime % 60).ToString("00")}" + " / " +
                            $"{Mathf.Floor(levelDuration / 60).ToString("00")}:{(levelDuration % 60).ToString("00")}";
        }

        public void PlayLevelMusic()
        {
            audioSource.Play();
            InstantiateBlockStart();
        }

        public void StopLevelMusic()
        {
            audioSource.Stop();
        }

        public void PauseLevelMusic()
        {
            audioSource.Pause();
        }

        public void SetAudioTime(float time)
        {
            time = Mathf.Clamp(time, 0, levelDuration);
            audioSource.time = time;
        }
        #endregion


        [Header("Level")]
        public Level level = new Level();
        private Vector3[] dodgePlayerPos = new Vector3[5] {
            new Vector3(-4.0f, 0.71f, 0.0f),
            new Vector3(-2.0f, 0.71f, 0.0f),
            new Vector3(0.0f, 0.71f, 0.0f),
            new Vector3(2.0f, 0.71f, 0.0f),
            new Vector3(4.0f, 0.71f, 0.0f)
        };

        private Vector3[] dodgeSpawns = new Vector3[5] {
            new Vector3(-4.0f, 5.0f, 9.7f),
            new Vector3(-2.0f, 5.0f, 9.7f),
            new Vector3(0.0f, 5.0f, 9.7f),
            new Vector3(2.0f, 5.0f, 9.7f),
            new Vector3(4.0f, 5.0f, 9.7f)
        };

        private Vector3[] dodgeEnd = new Vector3[5] {
            new Vector3(-4.0f, -2.0f, -2.0f),
            new Vector3(-2.0f, -2.0f, -2.0f),
            new Vector3(0.0f, -2.0f, -2.0f),
            new Vector3(2.0f, -2.0f, -2.0f),
            new Vector3(4.0f, -2.0f, -2.0f)
        };

        #region Create Blocks
        private void CreateNewBlock(int spawnPos, float speed, BlockType blockType)
        {
            if (blockType == BlockType.DirectionUp || blockType == BlockType.DirectionDown)
                spawnPos = Mathf.Clamp(spawnPos, 0, 1);
            else
                spawnPos = Mathf.Clamp(spawnPos, 0, 4);

            BlockProperties blockProperties = new BlockProperties(blockType, spawnPos, speed);

            float spawnTime = currentTime;

            if (!spawnCurrentTime)
                spawnTime -= TimeSpawnToReachPlayer(speed, spawnPos, blockType);

            LevelEvents existingLevelEvent = level.levelEvents.Find(le => Mathf.Abs(le.time - spawnTime) <= 0.1f);

            if (existingLevelEvent != null)
            {
                existingLevelEvent.AddBlock(blockProperties);
            }
            else
            {
                // LevelEvents levelEvent = new LevelEvents(spawnTime, ReturnCorrectLevelType(blockType));
                LevelEvents levelEvent = new LevelEvents(spawnTime, LevelType.Null);
                levelEvent.AddBlock(blockProperties);
                level.levelEvents.Add(levelEvent);
            }

            LevelType ReturnCorrectLevelType(BlockType blockType)
            {
                switch (blockType)
                {
                    case BlockType.DirectionUp:
                    case BlockType.DirectionDown:
                        return LevelType.Direction;
                    case BlockType.Dodge1:
                    case BlockType.Dodge2:
                    case BlockType.Dodge3:
                    case BlockType.Dodge4:
                    case BlockType.Dodge5:
                        return LevelType.Dodge;
                    default:
                        return LevelType.Null;
                }
            }

            float TimeSpawnToReachPlayer(float speed, int spawnPosition, BlockType b)
            {
                if (b == BlockType.DirectionUp || b == BlockType.DirectionDown)
                {
                    return 0;
                }
                float distance = Vector3.Distance(dodgePlayerPos[spawnPosition], dodgeSpawns[spawnPosition]);
                return distance / speed;
            }
        }

        private void ChangePrespectiveEvent(bool isOrtho)
        {
            LevelEvents levelEvent = new LevelEvents(currentTime);
            levelEvent.levelType = isOrtho ? LevelType.Direction : LevelType.Dodge;
            level.levelEvents.Add(levelEvent);
        }

        /*
        private void CreateBlocksInput()
        {
            // Dodge, keys 1-2-3-4-5
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                CreateNewBlock(0, speed, BlockType.Dodge1);
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                CreateNewBlock(1, speed, BlockType.Dodge2);
            }
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                CreateNewBlock(2, speed, BlockType.Dodge3);
            }
            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                CreateNewBlock(3, speed, BlockType.Dodge4);
            }
            if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                CreateNewBlock(4, speed, BlockType.Dodge5);
            }

            // Direction, keys K-L
            if (Input.GetKeyDown(KeyCode.K))
            {
                CreateNewBlock(0, speed, BlockType.DirectionUp);
            }
            if (Input.GetKeyDown(KeyCode.L))
            {
                CreateNewBlock(1, speed, BlockType.DirectionDown);
            }

            // Change prespective, key Z - X
            if (Input.GetKeyDown(KeyCode.Z))
            {
                ChangePrespectiveEvent(false);
            }

            if (Input.GetKeyDown(KeyCode.X))
            {
                ChangePrespectiveEvent(true);
            }
        }
        */
        
        #endregion


        [Header("Default Level Settings")]
        [SerializeField] private float speed = 3.0f;
        public TMP_InputField speedInput;
        [SerializeField] private int spawnPosition = 0;
        public TMP_InputField spawnPositionInput;
        [SerializeField] private bool spawnCurrentTime = true;
        public Toggle spawnCurrentTimeToggle;

        #region Default Level Settings
        private void StartDefaultLevelSettings()
        {
            speedInput.onValueChanged.AddListener(SetSpeed);
            spawnPositionInput.onValueChanged.AddListener(SetSpawnPosition);
            spawnCurrentTimeToggle.onValueChanged.AddListener(SetSpawnCurrentTime);

            speedInput.text = speed.ToString();
            spawnPositionInput.text = (spawnPosition + 1).ToString();
            spawnCurrentTimeToggle.isOn = spawnCurrentTime;

            void SetSpeed(string value)
            {
                speed = float.Parse(value);

                speedInput.text = speed.ToString();
            }

            void SetSpawnPosition(string value)
            {
                spawnPosition = int.Parse(value) - 1;
                spawnPosition = Mathf.Clamp(spawnPosition, 0, 4);

                spawnPositionInput.text = (spawnPosition + 1).ToString();
            }

            void SetSpawnCurrentTime(bool value)
            {
                spawnCurrentTime = value;
            }
        }
        #endregion

        [Header("Spawners")]
        public DodgeSpawner dodgeSpawner;
        public RunnerSpawner runnerSpawner;
        public GameManager gameManager;
        public float SpawnDelay = 0.5f;

        #region Spawn Blocks

        /// <summary>
        /// Instantiate the block in the correct time
        /// 
        /// Control movement of blocks (back and forth)
        /// 
        /// If the time is skipped must find the blocks that should be spawned and spawn them in the correct position, also destroy the other blocks
        /// </summary>

        [System.Serializable]
        class BlockEditor
        {
            public BlockType blockType;
            public int spawnPosition;
            public float speed;
            public GameObject block;

            public Vector3 start;
            public Vector3 end;

            public BlockEditor(BlockType blockType, int spawnPosition, float speed, GameObject block)
            {
                this.blockType = blockType;
                this.spawnPosition = spawnPosition;
                this.speed = speed;
                this.block = block;
            }

            public BlockEditor(BlockType blockType, int spawnPosition, float speed)
            {
                this.blockType = blockType;
                this.spawnPosition = spawnPosition;
                this.speed = speed;
            }

            public void SetBlock(GameObject block)
            {
                this.block = block;
            }

            public void SetEnd(Vector3 end)
            {
                this.end = end;
            }
        }

        [SerializeField] private List<BlockEditor> blocks = new List<BlockEditor>();
        public GameObject blockPrefab;
        private Vector3 blockPrefabDirection;

        private void InstantiateBlockStart()
        {
            var dodgeBlock = blockPrefab.GetComponentInChildren<DodgeBlock>();

            // Get distance difference between blockprefab and block
            var blockPrefabPosition = blockPrefab.transform.position;
            var blockPosition = dodgeBlock.transform.position;
            Vector3 difference = blockPrefabPosition - blockPosition;
            difference = new Vector3(0, Mathf.Abs(difference.y), 0);

            print("Instantiating blocks: " + level.levelEvents.Count);

            foreach (var levelEvent in level.levelEvents)
            {
                foreach (var block in levelEvent.blockProperties)
                {
                    Vector3 start = dodgeSpawns[block.spawnPosition];
                    Vector3 end = dodgeEnd[block.spawnPosition];
                    Vector3 direction = (end - start) * -1;
                    direction.Normalize();

                    blockPrefabDirection = direction;

                    // Calculate distance traveled in x time with x speed
                    float speed = block.speed;
                    float time = levelEvent.time;
                    float distance = speed * time;

                    // Calculate the position of the block
                    Vector3 position = dodgeSpawns[block.spawnPosition] + direction * distance;

                    GameObject temp = Instantiate(blockPrefab.gameObject, position, Quaternion.identity);
                    DodgeBlock blockObj = temp.GetComponentInChildren<DodgeBlock>();
                    blockObj.SetTargetPosition(dodgeEnd[block.spawnPosition] + difference);
                    blockObj.SetSpeed(0);

                    // Deactiivate dodgeblock script to control the movement of the block
                    blockObj.enabled = false;

                    BlockEditor newBlock = new BlockEditor(block.blockType, block.spawnPosition, block.speed, blockObj.gameObject);
                    newBlock.SetEnd(dodgeEnd[block.spawnPosition] + difference);
                    newBlock.start = position;

                    blocks.Add(newBlock);
                    print("Instantiated block: " + block.blockType + " at " + block.spawnPosition);
                }
            }
        }

        private float lastAudioTime = 0;
        private void MoveBlocks()
        {
            var delta = currentTime - lastAudioTime;
            foreach (var block in blocks)
            {
                if (block.block == null)
                    continue;

                block.block.transform.position = block.block.transform.position - blockPrefabDirection * block.speed * delta;
                // Move block towards the dodgeEnd
                // if (block.block.transform.position.y > block.end.y) {
                //     block.block.transform.position = Vector3.MoveTowards(block.block.transform.position, block.end, block.speed * delta);
                // } else {
                // }
            }

            lastAudioTime = currentTime;
        }

        #endregion

        private void Start()
        {
            OnAudioLoad();
            StartDefaultLevelSettings();

            LoadLevel();
        }


        private void Update()
        {
            AudioUpdate();
            //CreateBlocksInput();

            MoveBlocks();

            // if (Input.GetKeyDown(KeyCode.S)) {
            //     SaveLevel();
            // }
        }

        #region Save Load Level
        public void SaveLevel()
        {
            string json = JsonUtility.ToJson(level);
            var dateNow = System.DateTime.Now;
            System.IO.File.WriteAllText(Application.dataPath + "/Resources/Levels/Level" + dateNow.ToString("yyyy-MM-dd-HH-mm-ss") + ".json", json);
        }

        public void LoadLevel()
        {
            string json = System.IO.File.ReadAllText(Application.dataPath + "/Resources/Levels/Level2024-03-12-18-22-39.json");
            level = JsonUtility.FromJson<Level>(json);
        }
        #endregion
    }
}
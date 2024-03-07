using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;
using _Scripts.Managers.Camera;
using _Scripts.Managers.Game;
using _Scripts.Managers.Spawners;
using TMPro;
using UnityEngine.UI;

namespace _Scripts.Managers.Level_Controller
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
        private void OnAudioLoad() {
            levelDuration = levelMusic.length;
            audioSource.clip = levelMusic;

            PauseLevelMusic();
            SetupAudioUI();
        }

        public void PlayStopLevelMusic() {
            if (audioSource.isPlaying) {
                PauseLevelMusic();
                playStopButton.GetComponentInChildren<TextMeshProUGUI>().text = ">";
            }
            else {
                PlayLevelMusic();
                playStopButton.GetComponentInChildren<TextMeshProUGUI>().text = "||";
            }
        }

        private void SetupAudioUI() {
            playStopButton.onClick.AddListener(PlayStopLevelMusic);
            timeSlider.onValueChanged.AddListener(SetAudioTime);

            timeSlider.maxValue = levelDuration;
        }

        private void AudioUpdate() {
            timeSlider.value = audioSource.time;
            currentTime = audioSource.time;

            timeText.text = $"{Mathf.Floor(currentTime / 60).ToString("00")}:{(currentTime % 60).ToString("00")}" + " / " +
                            $"{Mathf.Floor(levelDuration / 60).ToString("00")}:{(levelDuration % 60).ToString("00")}";
        }

        public void PlayLevelMusic() {
            audioSource.Play();
        }

        public void StopLevelMusic() {
            audioSource.Stop();
        }

        public void PauseLevelMusic() {
            audioSource.Pause();
        }

        public void SetAudioTime(float time) {
            time = Mathf.Clamp(time, 0, levelDuration);
            audioSource.time = time;
        }
        #endregion


        [Header("Level")]
        public Level level = new Level();

        #region Create Blocks
        private void CreateNewBlock(int spawnPos, float speed, BlockType blockType) {
            if (blockType == BlockType.DirectionUp || blockType == BlockType.DirectionDown)
                spawnPos = Mathf.Clamp(spawnPos, 0, 1);
            else
                spawnPos = Mathf.Clamp(spawnPos, 0, 4);

            BlockProperties blockProperties = new BlockProperties(blockType, spawnPos, speed);

            float spawnTime = currentTime;
            

            LevelEvents existingLevelEvent = level.levelEvents.Find(le => Mathf.Abs(le.time - spawnTime) <= 0.1f);
            
            if (existingLevelEvent != null) {
                existingLevelEvent.AddBlock(blockProperties);
            } else {
                LevelEvents levelEvent = new LevelEvents(spawnTime, ReturnCorrectLevelType(blockType));
                levelEvent.AddBlock(blockProperties);
                level.levelEvents.Add(levelEvent);
            }

            LevelType ReturnCorrectLevelType(BlockType blockType) {
                switch (blockType) {
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
        }

        private void ChangePrespectiveEvent(bool isOrtho) {
            LevelEvents levelEvent = new LevelEvents(currentTime);
            levelEvent.levelType = isOrtho ? LevelType.Direction : LevelType.Dodge;
            level.levelEvents.Add(levelEvent);
        }

        private void CreateBlocksInput() {
            // Dodge, keys 1-2-3-4-5
            if (Input.GetKeyDown(KeyCode.Alpha1)) {
                CreateNewBlock(0, speed, BlockType.Dodge1);
            }
            if (Input.GetKeyDown(KeyCode.Alpha2)) {
                CreateNewBlock(1, speed, BlockType.Dodge2);
            }
            if (Input.GetKeyDown(KeyCode.Alpha3)) {
                CreateNewBlock(2, speed, BlockType.Dodge3);
            }
            if (Input.GetKeyDown(KeyCode.Alpha4)) {
                CreateNewBlock(3, speed, BlockType.Dodge4);
            }
            if (Input.GetKeyDown(KeyCode.Alpha5)) {
                CreateNewBlock(4, speed, BlockType.Dodge5);
            }

            // Direction, keys K-L
            if (Input.GetKeyDown(KeyCode.K)) {
                CreateNewBlock(0, speed, BlockType.DirectionUp);
            }
            if (Input.GetKeyDown(KeyCode.L)) {
                CreateNewBlock(1, speed, BlockType.DirectionDown);
            }

            // Change prespective, key Z - X
            if (Input.GetKeyDown(KeyCode.Z)) {
                ChangePrespectiveEvent(false);
            }

            if (Input.GetKeyDown(KeyCode.X)) {
                ChangePrespectiveEvent(true);
            }
        }
        #endregion


        [Header("Default Level Settings")]
        [SerializeField] private float speed = 3.0f;
        public TMP_InputField speedInput;
        [SerializeField] private int spawnPosition = 0;
        public TMP_InputField spawnPositionInput;
        [SerializeField] private bool spawnCurrentTime = true;
        public Toggle spawnCurrentTimeToggle;

        #region Default Level Settings
        private void StartDefaultLevelSettings() {
            speedInput.onValueChanged.AddListener(SetSpeed);
            spawnPositionInput.onValueChanged.AddListener(SetSpawnPosition);
            spawnCurrentTimeToggle.onValueChanged.AddListener(SetSpawnCurrentTime);

            speedInput.text = speed.ToString();
            spawnPositionInput.text = (spawnPosition + 1).ToString();
            spawnCurrentTimeToggle.isOn = spawnCurrentTime;

            void SetSpeed(string value) {
                speed = float.Parse(value);

                speedInput.text = speed.ToString();
            }

            void SetSpawnPosition(string value) {
                spawnPosition = int.Parse(value) - 1;
                spawnPosition = Mathf.Clamp(spawnPosition, 0, 4);

                spawnPositionInput.text = (spawnPosition + 1).ToString();
            }

            void SetSpawnCurrentTime(bool value) {
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

        #endregion

        private void Start() {
            OnAudioLoad();
            StartDefaultLevelSettings();

            LoadLevel();
        }

        private void Update() {
            AudioUpdate();
            CreateBlocksInput();

            if (Input.GetKeyDown(KeyCode.S)) {
                SaveLevel();
            }
        }

        #region Save Load Level
        public void SaveLevel() {
            string json = JsonUtility.ToJson(level);
            var dateNow = System.DateTime.Now;
            System.IO.File.WriteAllText(Application.dataPath + "/Resources/Levels/Level" + dateNow.ToString("yyyy-MM-dd-HH-mm-ss") + ".json", json);
        }

        public void LoadLevel() {
            string json = System.IO.File.ReadAllText(Application.dataPath + "/Resources/Levels/Level2024-03-05-19-04-35.json");
            level = JsonUtility.FromJson<Level>(json);
        }
        #endregion
    }
}
using _Scripts.Managers.Spawners;
using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;
using _Scripts.Managers.Camera;
using _Scripts.Managers.Game;

namespace _Scripts.Managers.Level_Controller
{
    // This class controls the level creation and management
    public class LevelController : MonoBehaviour
    {
        [Header("General Settings")]
        public float levelDuration = 0f; 
        private float currentTime = 0f;
        public AudioClip levelMusic;
        public AudioSource audioSource;

        [Header("Level Info")]
        [SerializeField] private TextAsset levelJson;
        public Level level;
        
        [Header("Controllers")]
        public DodgeSpawner dodgeSpawner;
        public RunnerSpawner runnerSpawner;
        public GameManager gameManager;

        #region Unity Functions
        void Start()
        {
            OnAudioLoad();
            LoadLevel();
            SetupLevel();
        }
        
        void Update()
        {
            currentTime += Time.deltaTime;

            InstanceBlocks();
        }
        
        private void OnApplicationQuit()
        {
            currentLevel = null;
        }
        #endregion
        
        #region Setup Level
        private void OnAudioLoad()
        {
            levelDuration = levelMusic.length;
            audioSource.clip = levelMusic;
            audioSource.Play();
        }
        
        public void LoadLevel() {
            string json = levelJson.text;
            level = JsonUtility.FromJson<Level>(json);
        }
        
        private void SetupLevel() {
            currentLevel = level;
            currentTime = 0f;
        }
        #endregion


#region Level Instance
        private Level currentLevel;
        private LevelType currentLevelType = LevelType.Null;
        
        private void InstanceBlocks() {
            List<LevelEvents> eventsToRemove = new List<LevelEvents>();

            foreach (var levelEvent in currentLevel.levelEvents) {
                // exit if the event time is greater than the current time
                if (levelEvent.time > currentTime)
                    break;

                if (levelEvent.time > currentTime)
                    continue;

                foreach (var block in levelEvent.blockProperties) {
                    SpawnBlock(block);
                }
                
                if (currentLevelType != levelEvent.levelType) 
                    UpdatePrespective(levelEvent);
   
                eventsToRemove.Add(levelEvent);
            }

            foreach (var levelEvent in eventsToRemove) {
                currentLevel.levelEvents.Remove(levelEvent);
            }
            

            void SpawnBlock(BlockProperties block) {
                switch (block.blockType) {
                    case BlockType.DirectionUp:
                        runnerSpawner.SpawnBlock(0);
                        break;
                    case BlockType.DirectionDown:
                        runnerSpawner.SpawnBlock(1);
                        break;
                    case BlockType.Dodge1:
                    case BlockType.Dodge2:
                    case BlockType.Dodge3:
                    case BlockType.Dodge4:
                    case BlockType.Dodge5:
                        dodgeSpawner.SpawnBlock(block.spawnPosition);
                        break;
                }
            }

            void UpdatePrespective(LevelEvents levelEvent) {
                switch (levelEvent.levelType) {
                    case LevelType.Direction:
                        gameManager.ChangeOrtho();
                        currentLevelType = LevelType.Direction;
                        break;
                    case LevelType.Dodge:
                        gameManager.ChangePrespective();
                        currentLevelType = LevelType.Dodge;
                        break;
                }
            }
        }

#endregion
    }
}
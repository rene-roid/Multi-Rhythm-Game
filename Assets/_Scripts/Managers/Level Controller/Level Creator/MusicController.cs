using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace _Scripts.Managers.Level_Controller.Level_Creator
{
    public class MusicController : MonoBehaviour
    {
        [Header("Music")]
        public AudioSource audioSource;
        public float musicTime = 0.0f;
        public float musicLength = 0.0f;

        public float bpm = 120;
        
        private PlayerInputActions inputActions;
        private InputAction lmb, rmb;

        [Header("Waveform")] 
        public int width = 2048;
        public int height = 256;
        public int widthMultiplier = 100;
        public Color background = Color.clear;
        public Color foreground = Color.white;
        
        public bool autoGenWidth = true;
        
        public Image waveformImage;
        private int sampleSize;
        private float[] samples;
        private float[] waveform;
        
        private Vector2 waveformStartPos;
        private Vector2 waveformEndPos;
        
        [SerializeField] private bool isDragging = false;
        
        [Header("Music Controls")]
        public Button playPauseButton;
        public Sprite playSprite;
        public Sprite pauseSprite;
        public Button startButton;
        public Button endButton;
        public Slider musicSlider;
        public TMP_InputField musicTimeInput;
        public TextMeshProUGUI musicPlaceHolder;
        public TMP_InputField bpmInput;
        
        [Header("BPM Grid")]
        public GridLayoutGroup bpmGrid;
        public GameObject bpmElementPrefab;

        private void Awake()
        {
            inputActions = new PlayerInputActions();
            lmb = inputActions.UI.LMB;
            rmb = inputActions.UI.RMB;
        }

        private void OnEnable()
        {
            lmb.Enable();
            rmb.Enable();
        }

        private void OnDisable()
        {
            lmb.Disable();
            rmb.Disable();
        }

        private void Start()
        {
            musicTime = audioSource.time;
            musicLength = audioSource.clip.length - 1;

            WaveformStart();
            MusicControlsStart();
            StartBpmGrid();
        }

        private void Update()
        {
            MusicControlsUpdate();
            UpdateWaveform();
            UpdateBpm();
        }

        #region Waveform
        private void WaveformStart()
        {
            if (autoGenWidth) width = (int)audioSource.clip.length * widthMultiplier;

            waveformImage.rectTransform.sizeDelta = new Vector2(width, height);
            waveformImage.rectTransform.anchoredPosition = new Vector2(width * 0.5f, 10);

            Texture2D[] textures = CreateWaveform();
            for (int i = 0; i < textures.Length; i++)
            {
                GameObject newWaveformImage = new GameObject("WaveformImagePart" + i);
                Image imageComponent = newWaveformImage.AddComponent<Image>();
                Rect rect = new Rect(0, 0, textures[i].width, height); // Update rect dimensions for each texture
                imageComponent.sprite = Sprite.Create(textures[i], rect, Vector2.zero);
                RectTransform rectTransform = newWaveformImage.GetComponent<RectTransform>();
                rectTransform.sizeDelta = new Vector2(textures[i].width, height);
                rectTransform.anchoredPosition = new Vector2(i * textures[i].width, 10);
                newWaveformImage.transform.SetParent(waveformImage.transform);
            }

            waveformStartPos = waveformImage.rectTransform.anchoredPosition;
            waveformEndPos = new Vector2(-width * 0.5f, 10);
        }

        private void UpdateWaveform()
        {
            // Move the waveform to the left or right based on the music time
            waveformImage.rectTransform.anchoredPosition = Vector2.Lerp(waveformStartPos, waveformEndPos, musicTime / musicLength);
        }
        
        public void DragWaveform(Vector3 newGameObjectPos)
        {
            // Adjust the waveform's position, clamped within its bounds
            Vector2 newPos = new Vector2(newGameObjectPos.x, waveformImage.rectTransform.anchoredPosition.y);
            newPos.x = Mathf.Clamp(newPos.x, -width * 0.5f, width * 0.5f); // Ensure it's clamped within the waveform's width
            waveformImage.rectTransform.anchoredPosition = newPos;

            // Convert the waveform position to a percentage and adjust the music time accordingly
            float dragPercentage = Mathf.InverseLerp(width * 0.5f, -width * 0.5f, newPos.x);
            musicTime = dragPercentage * musicLength;
            musicTime = Mathf.Clamp(musicTime, 0, musicLength);
            audioSource.time = musicTime; // Update the AudioSource's time based on the drag
        }

        public void StartDrag()
        {
            isDragging = true;
            audioSource.Pause();
        }

        public void StopDrag(bool isLmb = false)
        {
            isDragging = false;
            if (isLmb) audioSource.Play();
        }

        private Texture2D[] CreateWaveform()
        {
            int maxTextureWidth = 8192;
            int numTextures = Mathf.CeilToInt((float)width / maxTextureWidth);
            int textureWidth = width / numTextures;
            int halfHeight = height / 2;
            float heightScale = (float)height * 0.75f;
            
            var gridLayout = waveformImage.GetComponent<GridLayoutGroup>();
            if (gridLayout)
            {
                Vector2 cellSize = gridLayout.cellSize;
                cellSize.x = width / numTextures;
                gridLayout.cellSize = cellSize;
            }

            Texture2D[] textures = new Texture2D[numTextures];
            waveform = new float[width];

            sampleSize = audioSource.clip.samples * audioSource.clip.channels;
            samples = new float[sampleSize];
            audioSource.clip.GetData(samples, 0);

            int packSize = (sampleSize / width);
            for (int w = 0; w < width; w++)
            {
                waveform[w] = Mathf.Abs(samples[w * packSize]);
            }

            for (int t = 0; t < numTextures; t++)
            {
                textures[t] = new Texture2D(textureWidth, height, TextureFormat.RGBA32, false);
                int startX = t * textureWidth;
                int endX = Mathf.Min(startX + textureWidth, width);

                for (int x = 0; x < textureWidth; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        textures[t].SetPixel(x, y, background);
                    }
                }

                for (int x = startX; x < endX; x++)
                {
                    int localX = x - startX;
                    int y = (int)(waveform[x] * heightScale);
                    for (int i = 0; i < y; i++)
                    {
                        textures[t].SetPixel(localX, halfHeight + i, foreground);
                        textures[t].SetPixel(localX, halfHeight - i, foreground);
                    }
                }

                textures[t].Apply();
            }

            return textures;
        }
        #endregion

        #region Music Controls

        private void MusicControlsStart()
        {
            playPauseButton.onClick.AddListener(PlayPauseButton);
            startButton.onClick.AddListener(StartButton);
            endButton.onClick.AddListener(EndButton);
            musicSlider.onValueChanged.AddListener(MusicSlider);
            musicTimeInput.onEndEdit.AddListener(MusicTimeInput);
            bpmInput.onEndEdit.AddListener(BpmInput);
        }
        
        private void MusicControlsUpdate()
        {
            if (audioSource.clip == null)
            {
                Debug.LogError("AudioSource clip is null");
                return;
            }

            musicTime = audioSource.time;

            if (musicLength == 0)
            {
                Debug.LogError("Audio clip length is zero");
                return;
            }

            musicSlider.onValueChanged.RemoveListener(MusicSlider);
            musicSlider.value = musicTime / musicLength;
            musicSlider.onValueChanged.AddListener(MusicSlider);

            musicPlaceHolder.text = musicTime.ToString("F2");
        }
        
        private void PlayPauseButton()
        {
            if (audioSource.isPlaying)
            {
                audioSource.Pause();
                playPauseButton.image.sprite = playSprite;
            }
            else
            {
                audioSource.Play();
                playPauseButton.image.sprite = pauseSprite;
            }
        }
        
        private void StartButton()
        {
            audioSource.time = 0;
            audioSource.Pause();
        }
        
        private void EndButton()
        {
            audioSource.time = musicLength;
            audioSource.Pause();
        }
        
        private void MusicSlider(float value)
        {
            audioSource.Pause();
            audioSource.time = musicLength * value;
        }
        
        private void MusicTimeInput(string value)
        {
            if (float.TryParse(value, out float result))
            {
                audioSource.Pause();
                audioSource.time = Mathf.Clamp(result, 0, audioSource.clip.length - 1);
                musicTimeInput.text = "";
            }
        }

        private void BpmInput(string value)
        {
            if (float.TryParse(value, out float result))
            {
                bpm = result;
                UpdateBpmGrid();
            }
        }
        #endregion

        #region BPM
        private void StartBpmGrid()
        {
            UpdateBpmGrid();
        }
        
        private void UpdateBpm()
        {
            RectTransform bpmGridRectTransform = bpmGrid.GetComponent<RectTransform>();
            bpmGridRectTransform.anchoredPosition = waveformImage.rectTransform.anchoredPosition;
            bpmGridRectTransform.sizeDelta = waveformImage.rectTransform.sizeDelta;
        }

        private void UpdateBpmGrid()
        {
            var bpmCount = (int)(musicLength * (bpm / 60));
            RectTransform bpmGridRectTransform = bpmGrid.GetComponent<RectTransform>();
            var availableWidth = waveformImage.rectTransform.rect.width - (bpmCount * bpmGrid.cellSize.x);
            var bpmElementSpacing = availableWidth / (bpmCount - 1);

            bpmGrid.spacing = new Vector2(bpmElementSpacing, 0);
            bpmGridRectTransform.sizeDelta = waveformImage.rectTransform.sizeDelta;

            // Clean up existing children
            foreach (Transform child in bpmGrid.transform)
                Destroy(child.gameObject);
    
            // Instantiate new BPM elements
            for (int i = 0; i < bpmCount; i++)
            {
                var bpmElement = Instantiate(bpmElementPrefab, bpmGrid.transform);
                bpmElement.name = (i * (int)(1000 / bpm)).ToString();
            }
        }
        #endregion
    }
}

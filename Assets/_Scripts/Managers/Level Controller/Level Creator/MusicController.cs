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
        [SerializeField] private Vector2 initialMousePos;
        [SerializeField] private Vector2 initialWaveformPos;
        
        [Header("Music Controls")]
        public Button playPauseButton;
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
            
            // Subscribe to LMB and RMB press/release events
            lmb.performed += _ => StartDrag();
            lmb.canceled += _ => StopDrag();
            
            rmb.performed += _ => StartDrag();
            rmb.canceled += _ => StopDrag(true);

            WaveformStart();
            MusicControlsStart();
            StartBpmGrid();
        }

        private void Update()
        {
            MusicControlsUpdate();
            UpdateWaveform();
        }

        #region Waveform
        private void WaveformStart()
        {
            if (autoGenWidth) width = (int)audioSource.clip.length * 100;
            Texture2D texture = CreateWaveform();
            Rect rect = new Rect(Vector2.zero, new Vector2(width, height));
            waveformImage.sprite = Sprite.Create(texture, rect, Vector2.zero);
            waveformImage.rectTransform.sizeDelta = new Vector2(width, height);
            waveformImage.rectTransform.anchoredPosition = new Vector2(width * 0.5f, 10);
            
            waveformStartPos = waveformImage.rectTransform.anchoredPosition;
            waveformEndPos = new Vector2(-width * 0.5f, 10);
        }

        private void UpdateWaveform()
        {
            // Move the waveform to the left or right based on the music time
            waveformImage.rectTransform.anchoredPosition = Vector2.Lerp(waveformStartPos, waveformEndPos, musicTime / musicLength);
            
            // Start dragging when LMB or RMB is pressed
            if ((lmb.IsPressed() || rmb.IsPressed()) && IsMouseOverWaveform())
                isDragging = true;
            
            if (isDragging) DragWaveform();
        }
        
        private void DragWaveform()
        {
            Vector2 currentMousePos = Mouse.current.position.ReadValue();
            float dragDistance = currentMousePos.x - initialMousePos.x;

            // Adjust the waveform's position, clamped within its bounds
            Vector2 newPos = new Vector2(initialWaveformPos.x + dragDistance, waveformImage.rectTransform.anchoredPosition.y);
            newPos.x = Mathf.Clamp(newPos.x, -width * 0.5f, width * 0.5f); // Ensure it's clamped within the waveform's width
            waveformImage.rectTransform.anchoredPosition = newPos;

            // Convert the waveform position to a percentage and adjust the music time accordingly
            float dragPercentage = Mathf.InverseLerp(width * 0.5f, -width * 0.5f, newPos.x);
            musicTime = dragPercentage * musicLength;
            musicTime = Mathf.Clamp(musicTime, 0, musicLength);
            audioSource.time = musicTime; // Update the AudioSource's time based on the drag
        }

        private bool IsMouseOverWaveform()
        {
            Vector2 mousePos = Mouse.current.position.ReadValue();
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                waveformImage.rectTransform, mousePos, null, out Vector2 localPos);
            
            return waveformImage.rectTransform.rect.Contains(localPos);
        }

        private void StartDrag()
        {
            if (IsMouseOverWaveform())
            {
                isDragging = true;
                initialMousePos = Mouse.current.position.ReadValue();
                initialWaveformPos = waveformImage.rectTransform.anchoredPosition;
                audioSource.Pause();
            }
        }

        private void StopDrag(bool isLmb = false)
        {
            if (!isDragging) return;
            isDragging = false;
            if (isLmb)
                audioSource.Play();
        }

        private Texture2D CreateWaveform()
        {
            int halfHeight = height / 2;
            float heightScale = (float)height * 0.75f;

            Texture2D texture2D = new Texture2D(width, height, TextureFormat.RGBA32, false);
            waveform = new float[width];
            
            sampleSize = audioSource.clip.samples * audioSource.clip.channels;
            samples = new float[sampleSize];
            audioSource.clip.GetData(samples, 0);
            
            int packSize = (sampleSize / width);
            for (int w = 0; w < width; w++)
            {
                waveform[w] = Mathf.Abs(samples[w * packSize]);
            }
            
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    texture2D.SetPixel(x, y, background);
                }
            }
            
            for (int x = 0; x < width; x++)
            {
                int y = (int)(waveform[x] * heightScale);
                for (int i = 0; i < y; i++)
                {
                    texture2D.SetPixel(x, halfHeight + i, foreground);
                    texture2D.SetPixel(x, halfHeight - i, foreground);
                }
            }
            
            texture2D.Apply();
            return texture2D;
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
                audioSource.Pause();
            else
                audioSource.Play();
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

        private void UpdateBpmGrid()
        {
            var bpmCount = (int)(musicLength * (bpm / 60));
            var availableWidth = waveformImage.rectTransform.rect.width - (bpmCount * bpmGrid.cellSize.x);
            var bpmElementSpacing = availableWidth / (bpmCount - 1);
    
            bpmGrid.spacing = new Vector2(bpmElementSpacing, 0);

            foreach (Transform child in bpmGrid.transform)
                Destroy(child.gameObject);
            
            for (int i = 0; i < bpmCount; i++)
            {
                var bpmElement = Instantiate(bpmElementPrefab, bpmGrid.transform);
                bpmElement.name = (i * (int)(1000 / bpm)).ToString();
            }
        }



        #endregion
    }
}

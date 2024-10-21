using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Scripts.Managers.Audio_Visualizer
{
    public class FFTObjectArrayGrow : MonoBehaviour
    {
        public FrequencyBandAnalyser fft;
        public FrequencyBandAnalyser.Bands freqBands = FrequencyBandAnalyser.Bands.Eight;

        public List<GameObject> fftGroups = new List<GameObject>();
        
        [Header("Scaling")]
        public bool changeScale = false;
        private Vector3 scale;
        public Vector3 minScale = Vector3.one;
        public Vector3 scalingStrength = Vector3.up;

        [Header("Color")]
        public bool changeColor = false;
        private readonly List<Color> restColors = new List<Color>();
        public float beatStrength = 0.5f;
        
        public Color[] colors;

        private void Start()
        {
            if (changeColor)
            {
                foreach (var fftGroup in fftGroups)
                {
                    foreach (var obj in fftGroup.transform)
                    {
                        var fftObject = (Transform)obj;
                        restColors.Add(fftObject.GetComponent<Renderer>().material.color);
                    }
                }
            }
        }

        private void Update()
        {
            if (changeScale)
            {
                for (int i = 0; i < fftGroups.Count; i++)
                {
                    foreach (var obj in fftGroups[i].transform)
                    {
                        var fftObject = (Transform)obj;
                        fftObject.localScale = minScale + (scalingStrength * fft.GetBandValue(i, freqBands));
                    }
                }
            }


            if (changeColor)
            {
                for (int i = 0; i < fftGroups.Count; i++)
                {
                    bool beat = fft.GetBandValue(i, freqBands) > beatStrength;
                    
                    foreach (var obj in fftGroups[i].transform)
                    {
                        var fftObject = (Transform)obj;
                        var renderer = fftObject.GetComponent<Renderer>();

                        renderer.material.color = Color.Lerp(renderer.material.color, restColors[i], Time.deltaTime);
                        if (!beat) continue;

                        renderer.material.color = Color.Lerp(renderer.material.color, colors[Random.Range(0, colors.Length)], fft.GetBandValue(Random.Range(0, (int)FrequencyBandAnalyser.Bands.SixtyFour), FrequencyBandAnalyser.Bands.SixtyFour));
                    }
                }
            }
        }
    }
}

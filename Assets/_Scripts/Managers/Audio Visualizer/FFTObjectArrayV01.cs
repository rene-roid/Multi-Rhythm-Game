using System;
using UnityEngine;

namespace _Scripts.Managers.Audio_Visualizer
{
    public class FFTObjectArrayV01 : MonoBehaviour
    {
        public enum Shape { Line, Circle }
        public FrequencyBandAnalyser fft;
        public FrequencyBandAnalyser.Bands freqBands = FrequencyBandAnalyser.Bands.Eight;
        
        private GameObject[] fftObjects;
        public GameObject objectPrefab;
        public float spacing = 1;

        private Vector3 scale;
        public Vector3 scalingStrength = Vector3.up;

        public Shape shape = Shape.Line;
        
        private void Start()
        {
            fftObjects = new GameObject[(int)freqBands];
            scale = objectPrefab.transform.localScale;

            if (shape == Shape.Line)
            {
                for (int i = 0; i < fftObjects.Length; i++)
                {
                    GameObject obj = Instantiate(objectPrefab, transform, true);
                    obj.transform.localPosition = new Vector3(i * spacing, 0, 0);
                    fftObjects[i] = obj;
                }
            } else if (shape == Shape.Circle)
            {
                double angleSpacing = 2f * Math.PI / fftObjects.Length;
                
                for (int i = 0; i < fftObjects.Length; i++)
                {
                    float angle = (float) (i * angleSpacing);
                    Vector3 pos = new Vector3(Mathf.Cos(angle) * spacing, Mathf.Sin(angle) * spacing, 0);
                    
                    GameObject obj = Instantiate(objectPrefab, transform, true);
                    fftObjects[i] = obj;
                    obj.transform.localPosition = pos;
                    
                    obj.transform.LookAt(transform.position);
                    obj.transform.localRotation *= Quaternion.Euler(-90, 0, 0);
                }
            }
        }

        private void Update()
        {
            for (int i = 0; i < fftObjects.Length; i++)
            {
                fftObjects[i].transform.localScale = scale + (scalingStrength * fft.GetBandValue(i, freqBands));
            }
        }
    }
}

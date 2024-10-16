using System;
using UnityEngine;

namespace _Scripts.Managers.Audio_Visualizer
{
    [RequireComponent(typeof(LineRenderer))]
    public class FFTLineRendererV01 : MonoBehaviour
    {
        public enum Shape { Line, Circle }
        public FrequencyBandAnalyser fft;
        public FrequencyBandAnalyser.Bands freqBands = FrequencyBandAnalyser.Bands.Eight;

        private LineRenderer line;
        public float length = 4;
        float spacing = .2f;
        public float strength = 4;
        
        public Shape shape = Shape.Line;

        private void Start()
        {
            line = GetComponent<LineRenderer>();
            line.positionCount = (int)freqBands;
            spacing = length / (int)freqBands;
        }

        private void Update()
        {
            if (shape == Shape.Line)
            {
                for (int i = 0; i < line.positionCount; i++)
                {
                    float xPos = i * spacing;
                    float yPos = fft.GetBandValue(i, freqBands) * strength;
                
                    Vector3 pos = new Vector3(xPos, yPos, 0);
                    line.SetPosition(i, pos);
                }
            } else if (shape == Shape.Circle)
            {
                double angleSpacing = 2f * Math.PI / line.positionCount;
                
                for (int i = 0; i < line.positionCount; i++)
                {
                    float angle = (float) (i * angleSpacing);
                    float radius = length + fft.GetBandValue(i, freqBands) * strength;
                    Vector3 pos = new Vector3(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius, 0);
                    
                    line.SetPosition(i, pos);
                }
                
                // Join the last point to the first point
                line.loop = true;
            }
        }
    }
}

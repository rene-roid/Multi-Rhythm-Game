using _Scripts.Managers.Pooling;
using UnityEngine;

namespace _Scripts.Temp
{
    public class PoolParticlesOnClick : MonoBehaviour
    {
        void Update()
        {
            // Detect when any type of input is pressed
            if (Input.GetMouseButtonDown(0))
            {
                // Get a particle from the pool
                var particle = ParticlePool.pool.GetParticle();
                
                // Set the particle position to the mouse position
                if (Camera.main)
                    particle.transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            }
        }
    }
}

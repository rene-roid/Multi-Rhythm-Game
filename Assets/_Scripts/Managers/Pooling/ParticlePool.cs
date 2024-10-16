using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace _Scripts.Managers.Pooling
{
    public class ParticlePool : MonoBehaviour
    {
        public static ParticlePool pool;

        public readonly List<ParticleSystem> PooledParticles = new List<ParticleSystem>();
        public int poolSize = 10;

        [SerializeField] private ParticleSystem particlePrefab;

        private void Awake()
        {
            if (pool == null) pool = this;
        }

        private void Start()
        {
            for (int i = 0; i < poolSize; i++)
            {
                ParticleSystem particle = Instantiate(particlePrefab, transform);
                particle.gameObject.SetActive(false);
                PooledParticles.Add(particle);
            }
        }

        public ParticleSystem GetParticle()
        {
            foreach (var particle in PooledParticles)
            {
                if (!particle.gameObject.activeInHierarchy)
                {
                    particle.gameObject.SetActive(true);
                    StartCoroutine(DeactivateAfterPlaying(particle)); // Start coroutine to handle deactivation
                    return particle;
                }
            }

            ParticleSystem newParticle = Instantiate(particlePrefab, transform);
            newParticle.gameObject.SetActive(true);
            PooledParticles.Add(newParticle);
            StartCoroutine(DeactivateAfterPlaying(newParticle)); // Start coroutine for new particles
            return newParticle;
        }

        private IEnumerator DeactivateAfterPlaying(ParticleSystem particle)
        {
            yield return new WaitUntil(() => !particle.isPlaying);
            ReturnParticle(particle);
        }

        public void ReturnParticle(ParticleSystem particle)
        {
            particle.gameObject.SetActive(false);
        }
    }
}
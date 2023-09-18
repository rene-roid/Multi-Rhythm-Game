using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Scripts.Units.Player {
    public class HealthController : MonoBehaviour
    {
        private BoxCollider col;

        void Start()
        {
            col = GetComponentInChildren<BoxCollider>();
        }


        void Update()
        {
            HealthRegen();
        }

        #region Health Controller
        [Header("Health")]
        [SerializeField] private int maxHealth = 100;
        public int currentHealth = 100;
    
        [Header("Passive Healing")]
        [SerializeField] private int healthRegenAmount = 1;
    
        [SerializeField] private float healthRegenRate = 0.5f;
        [SerializeField] private float healthRegenDelay = .2f;
        private float healthRegenTimer = 0f;
        private bool isRegenerating = false;
        
        private int lastHitAtFrame = 0;

        private int fixedFrame = 0;
        private int inmuneFrames = 8;

        private void FixedUpdate(){
            fixedFrame++;
        }

        private void HealthRegen(){
            if (currentHealth >= maxHealth)
                return;
        
            if (isRegenerating){
                healthRegenTimer += Time.deltaTime;
                if (healthRegenTimer >= healthRegenRate){
                    healthRegenTimer = 0f;
                    Heal(healthRegenAmount);
                }
                return;
            }
            
            healthRegenTimer += Time.deltaTime;
            if (healthRegenTimer >= healthRegenDelay){
                healthRegenTimer = 0f;
                isRegenerating = true;
            }
        }
    
        public void Heal(int heal){
            currentHealth += heal;
            if (currentHealth > maxHealth){
                currentHealth = maxHealth;
            }
        }
        #endregion
    
        public void TakeDamage(int damage){
            if (fixedFrame - lastHitAtFrame < inmuneFrames)
                return;
            
            currentHealth -= damage;
            if (currentHealth <= 0){
                Die();
            }
        
            isRegenerating = false;
        }
    
        private void Die(){
            Destroy(this.gameObject);
        }
    
    }
}

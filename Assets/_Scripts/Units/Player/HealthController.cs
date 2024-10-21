using System;
using rene_roid;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace _Scripts.Units.Player {
    public class HealthController : MonoBehaviour
    {
        private BoxCollider col;
        private PlayerController playerController;

        void Start()
        {
            col = GetComponentInChildren<BoxCollider>();
            playerController = GetComponent<PlayerController>();
        }


        void Update()
        {
            HealthRegen();
            UpdateUI();
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
    
        public void TakeDamage(int damage, bool jumpable = false){
            if (fixedFrame - lastHitAtFrame < inmuneFrames)
                return;

            if (playerController.isJumping && jumpable)
                return;
            
            currentHealth -= damage;
            if (currentHealth <= 0){
                Die();
            }
            
            damageImage.color = new Color(1, 0, 0, 0.18f);
        
            isRegenerating = false;
        }
    
        private void Die(){
            Destroy(this.gameObject);
        }
    
        #region UI
        [SerializeField] Image healthBar;
        [SerializeField] Image damageImage;

        private void UpdateUI()
        {
            float current = Helpers.FromRangeToPercentage(currentHealth, 0, maxHealth, true);
            LeanTween.value(gameObject, healthBar.fillAmount, current, 0.5f)
                .setEase(LeanTweenType.easeOutCirc)
                .setOnUpdate((float val) => {
                    healthBar.fillAmount = val;
                });
            
            if (damageImage.color.a > 0){
                damageImage.color = Color.Lerp(damageImage.color, new Color(1, 0, 0, 0), Time.deltaTime * 2);
            }
        }
        #endregion
    }
}

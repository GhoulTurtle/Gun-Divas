using System;
using UnityEngine;

public class Health : MonoBehaviour{
    [Header("Required References")]
    [SerializeField] private PlayerStatsSO playerStatsSO;

    public Action OnGameOver;
    public Action OnHealthUpdated;

    public void TakeDamage(int damageAmount){
        playerStatsSO.currentHealth -= damageAmount * (1 - playerStatsSO.damageReductionPercent);
        if(playerStatsSO.currentHealth <= 0){
            OnGameOver?.Invoke();
            return;
        }

        OnHealthUpdated?.Invoke();
    }

    public void HealHealth(int healAmount){
        playerStatsSO.currentHealth += healAmount;
        if(playerStatsSO.currentHealth > playerStatsSO.baseMaxHealth){
            playerStatsSO.currentHealth = playerStatsSO.baseMaxHealth;
        }

        OnHealthUpdated?.Invoke();
    }
}
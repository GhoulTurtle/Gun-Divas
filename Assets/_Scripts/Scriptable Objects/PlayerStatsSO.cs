using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Player Stats", fileName = "NewPlayerStats")]
public class PlayerStatsSO : ScriptableObject{
    [Header("Base Stats")]
    public float baseMovementSpeed;
    public int baseMaxHealth = 100;
    public int baseCash = 20;

    [Header("Multipliers")]
    public float damageReductionPercent;
    public float movementSpeedIncreasePercent;
    public float reloadSpeedIncreasePercent;
    public float healPerKillPercent;

    [Header("Run Based Stats")]
    public float currentHealth;
    public int currentCash;
    public BaseDivaSO currentEquippedDiva;
    public BaseWeaponSO currentEquippedWeapon;
    public List<PlayerUpgrade> playerUpgradesList = new List<PlayerUpgrade>();

    [ContextMenu("Reset Player Stats")]
    public void ResetPlayerStats(){
        currentHealth = baseMaxHealth;
        currentCash = baseCash;
        currentEquippedDiva = null;
        currentEquippedWeapon = null;
        playerUpgradesList.Clear();

        damageReductionPercent = 0f;
        movementSpeedIncreasePercent = 0f;
        reloadSpeedIncreasePercent = 0f;
    }

    public void AddUpgrade(PlayerUpgradeType playerUpgradeType){
        //See if the upgrade is in the upgrade list
        bool added = false;
        for (int i = 0; i < playerUpgradesList.Count; i++){
            if(playerUpgradesList[i].IsUpgradeType(playerUpgradeType)){
                //Add one to the upgrade stack
                playerUpgradesList[i].AddUpgradeStack();
                added = true;
            }
        }

        if(!added){
            //Add the new upgrade to the list
            playerUpgradesList.Add(new PlayerUpgrade(playerUpgradeType));
        }
    }
}
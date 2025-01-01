using System.Collections.Generic;

public class PlayerUpgrade{
    public PlayerStatsSO playerStatsSO;
    public PlayerUpgradeType upgradeType;
    public int upgradeStackAmount;

    public float Value {get; private set;}  

    public PlayerUpgrade(PlayerUpgradeType _upgradeType){
        upgradeType = _upgradeType;
        upgradeStackAmount = 1;

        CalculateValue();
    }

    public bool IsUpgradeType(PlayerUpgradeType type){
        return upgradeType == type;
    }
    
    public void AddUpgradeStack(){
        upgradeStackAmount += 1;
        CalculateValue();
    }

    private void CalculateValue(){
        float baseVal = PlayerUpgradeIncrease[upgradeType];

        Value = baseVal * upgradeStackAmount;

        switch (upgradeType){
            case PlayerUpgradeType.Movement_Speed: playerStatsSO.movementSpeedIncreasePercent = Value;
                break;
            case PlayerUpgradeType.Kill_To_Heal: playerStatsSO.healPerKillPercent = Value;
                break;
            case PlayerUpgradeType.Damage_Reduction: playerStatsSO.damageReductionPercent = Value;
                break;
            case PlayerUpgradeType.Reload_Speed: playerStatsSO.reloadSpeedIncreasePercent = Value;
                break;
        }
    }

    //Base values for the different upgrades
    public static Dictionary<PlayerUpgradeType, float> PlayerUpgradeIncrease = new Dictionary<PlayerUpgradeType, float>(){
        {PlayerUpgradeType.Movement_Speed, 0.05f},
        {PlayerUpgradeType.Kill_To_Heal, 0.05f},
        {PlayerUpgradeType.Damage_Reduction, 0.05f},
        {PlayerUpgradeType.Reload_Speed, 0.03f}
    };
}

using UnityEngine;

[CreateAssetMenu(menuName = "Weapon Stats/Basic Gun", fileName = "NewGunStatsSO")]
public class BaseGunStatsSO : ScriptableObject{
    [Header("Required References")]
    public GunBehavior gunPrefab;

    [Header("Mechanic Variables")]
    [Tooltip("The rate the weapon will shoot in seconds.")]
    public float weaponFireRateInSeconds;
    [Tooltip("The time it takes to reload in seconds.")]
    public float weaponReloadTimeInSeconds;
    [Tooltip("The amount of damage dealt to a damageable object.")]
    public float weaponAttackDamage;
    [Tooltip("The base bloom angle that the weapon will use when doing bloom calculations. 0 is perfect accuracy.")]
    public float baseBloomAngle;
    [Tooltip("The minimum amount of shots that are needed to be fired in the kick back start window to begin the kickback of the weapon.")]
    public int minKickBackShotAmount;
    [Tooltip("The time window in seconds that the minimum kickback shot amount must be fired to trigger the kickback amount.")]
    public float kickBackWindowInSeconds;
    [Tooltip("The amount of kickback that is applied to the Y input of the player. 0 is no kickback added.")]
    public float kickBackAmount;
    [Tooltip("The max amount of ammo that the weapon has.")]    
    public int maxAmmo;
}
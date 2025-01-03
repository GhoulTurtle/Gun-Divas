using UnityEngine;

[CreateAssetMenu(menuName = "Weapon Stats/Shotgun", fileName = "NewShotgunStatsSO")]
public class ShotgunStatsSO : BaseGunStatsSO{
    [Header("Shotgun Stats")]
    public int pelletAmount;
}

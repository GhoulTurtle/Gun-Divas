using UnityEngine;

public abstract class GunBehavior : MonoBehaviour{
    public abstract void AttemptFireWeapon();
    public abstract void AttemptAltFireWeapon();
    public abstract void AttemptReloadWeapon();
    public abstract void DespawnWeapon();

}

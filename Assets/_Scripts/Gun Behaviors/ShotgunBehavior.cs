using UnityEngine;

public class ShotgunBehavior : GunBehavior{
    public override void AttemptAltFireWeapon(){

    }

    public override void AttemptFireWeapon(){

    }

    public override void AttemptReloadWeapon(){

    }

    public override void DespawnWeapon(){
        StopAllCoroutines();
        Destroy(gameObject);
    }
}

using UnityEngine;
using UnityEngine.InputSystem;
/// <summary>
/// Handles the input and the spawning/despawning of equipped weapons.
/// </summary>
public class PlayerWeaponHandler : MonoBehaviour{
    [Header("Required References")]
    [SerializeField] private PlayerStatsSO playerStatsSO;
    [SerializeField] private Transform gunEquippedParent;

    private GunBehavior currentEquippedWeapon;

    private void Awake() {
        if(playerStatsSO != null){
            playerStatsSO.OnWeaponEquipped += () => EquipWeapon(playerStatsSO.currentEquippedWeapon.gunPrefab);
        }
    }

    private void OnDestroy() {
        if(playerStatsSO != null){
            playerStatsSO.OnWeaponEquipped -= () => EquipWeapon(playerStatsSO.currentEquippedWeapon.gunPrefab);
        }
    }

    public void EquipWeapon(GunBehavior gunBehavior){
        if(currentEquippedWeapon != null){
            currentEquippedWeapon.DespawnWeapon();
            currentEquippedWeapon = null;
        }

        currentEquippedWeapon = Instantiate(gunBehavior, gunEquippedParent);
    }

    public void OnReloadInput(InputAction.CallbackContext context){
        if(currentEquippedWeapon == null) return;
        if(context.phase == InputActionPhase.Performed){
            currentEquippedWeapon.AttemptReloadWeapon();
        }
    }

    public void OnFireInput(InputAction.CallbackContext context){
        if(currentEquippedWeapon == null) return;
        if(context.phase == InputActionPhase.Performed){
            currentEquippedWeapon.AttemptFireWeapon();
        }
    }

    public void OnAltFireInput(InputAction.CallbackContext context){
        if(currentEquippedWeapon == null) return;
        if(context.phase == InputActionPhase.Performed){
            currentEquippedWeapon.AttemptAltFireWeapon();
        }
    }
}

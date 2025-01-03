using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponSwitcher : MonoBehaviour{
    [Header("Required References")]
    [SerializeField] private List<BaseGunStatsSO> gunsToSwitch;
    [SerializeField] private PlayerWeaponHandler playerWeaponHandler;

    public void OnWeaponSwitchInput(InputAction.CallbackContext context){
        if(context.phase != InputActionPhase.Performed) return;

        var key = Keyboard.current;

        int switchVal = -1;

        if(key.digit1Key.wasPressedThisFrame){
            switchVal = 0;
        }
        else if(key.digit2Key.wasPressedThisFrame){
            switchVal = 1;
        }
        else if(key.digit3Key.wasPressedThisFrame){
            switchVal = 2;
        }
    
        if(gunsToSwitch[switchVal] != null){
            playerWeaponHandler.EquipWeapon(gunsToSwitch[switchVal].gunPrefab);
        }    
    }
}
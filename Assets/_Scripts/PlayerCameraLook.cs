using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCameraLook : MonoBehaviour{
	[Header("Cam Variables")]
	[SerializeField, Range(0.1f, 100)] private float camSens;
	[SerializeField] private bool lockCursor;

	[Header("Required Reference")]
	[SerializeField] private Transform cameraRoot;
	[SerializeField] private Transform characterOrientation;

	private const float YClamp = 90f;

	private float camX;
	private float camY;

    private void Start() {
		Cursor.lockState = lockCursor ? CursorLockMode.Locked : CursorLockMode.None;
	}

	public void Update(){
		cameraRoot.localRotation = Quaternion.Euler(camY, camX, 0);

		characterOrientation.transform.localRotation = Quaternion.Euler(0, camX, 0);
	}

	public void OnLookInput(InputAction.CallbackContext context){
		var inputVector = context.ReadValue<Vector2>();
		camX += inputVector.x * camSens * Time.deltaTime;
		camY -= inputVector.y * camSens * Time.deltaTime;
		camY = Mathf.Clamp(camY, -YClamp, YClamp);
	}

	public void LookInputInjected(Vector2 inputVector){
		camX += inputVector.x;
		camY -= inputVector.y;
		camY = Mathf.Clamp(camY, -YClamp, YClamp);
	}
}
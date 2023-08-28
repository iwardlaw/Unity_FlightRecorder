using System.Collections;
using UnityEngine;

namespace temp.iaw
{
	public class SimpleFlightController : MonoBehaviour
	{
		[field: SerializeField] public bool CanMove { get; private set; } = false;
		public bool ShouldMove = false;
		public bool IsMoving => CanMove && ShouldMove;

		[field: Header("Inverse Control")]
		[field: SerializeField] public bool ApplyInverseMovement { get; private set; } = false;
		private Transform worldTransform;

		[field: Header("Parameters")]
		[field: SerializeField] public float Speed { get; private set; } = 1f;
		[field: SerializeField] public float MinSpeed { get; private set; } = 0f;
		[field: SerializeField] public float MaxSpeed { get; private set; } = 10f;
		[field: SerializeField] public float Acceleration { get; private set; } = 1f;

		[field: Space(10)]
		[field: SerializeField] public float PitchRate { get; private set; } = 10f;
		[field: SerializeField] public float MaxPitch { get; private set; } = 35f;
		[field: SerializeField] public float RollRate { get; private set; } = 10f;
		[field: SerializeField] public float MaxRoll { get; private set; } = 90f;
		[field: SerializeField] public float BankTurnMultiplier { get; private set; } = 0.2f;
		[field: SerializeField] public float YawRate { get; private set; } = 5f;
		public float Pitch { get; private set; }
		public float Heading => Vector3.Angle(Vector3.forward, Vector3.ProjectOnPlane(transform.forward, Vector3.up));
		public float Yaw { get; private set; }
		public float Roll { get; private set; }

		[Header("Controls")]
		[SerializeField] private string pitchAxis = "Vertical";
		[SerializeField] private string yawAxis = "Yaw";
		[SerializeField] private string rollAxis = "Horizontal";
		[SerializeField] private string speedAxis = "Speed";

		[Space(10)]
		[SerializeField] private KeyCode resetPitchKey = KeyCode.R;
		[SerializeField] private KeyCode resetRollKey = KeyCode.F;

		private float pitchInput, yawInput, rollInput, speedInput;
		private float pitchDiff, yawDiff, rollDiff;
		private float targetPitch, targetRoll;

		private Coroutine resetPitchCoroutine, resetRollCoroutine;

		private void Start()
		{
			if(ApplyInverseMovement) {
				GameObject worldTransformObj = GameObject.FindWithTag("WorldTransform");
				if(worldTransformObj != null) {
					worldTransform = worldTransformObj.transform;
				}
				else {
					Debug.LogError("Attempting to apply inverse motion with flight controller, but no world transform found.");
				}
			}
		}

		private void Update()
		{
			if(!IsMoving) {
				return;
			}

			pitchInput = Input.GetAxis(pitchAxis);
			if(pitchInput != 0f) {
				CancelResetPitch();

				pitchDiff = pitchInput * Time.deltaTime * PitchRate;
				targetPitch = Pitch + pitchDiff;
				if(targetPitch > MaxPitch) { pitchDiff = MaxPitch - Pitch; }
				else if(targetPitch < -MaxPitch) { pitchDiff = -MaxPitch - Pitch; }

				Pitch = Mathf.Clamp(Pitch + pitchDiff, -MaxPitch, MaxPitch);
			}
			else {
				pitchDiff = 0f;
				if(Input.GetKeyDown(resetPitchKey)) {
					ResetPitch();
				}
			}

			yawInput = Input.GetAxis(yawAxis);
			if(yawInput != 0f) {
				yawDiff = yawInput * Time.deltaTime * YawRate;
				Yaw += yawDiff;
			}
			else {
				yawDiff = 0f;
			}

			rollInput = Input.GetAxis(rollAxis);
			if(rollInput != 0f) {
				CancelResetRoll();

				rollDiff = rollInput * Time.deltaTime * RollRate;
				targetRoll = Roll + rollDiff;
				if(targetRoll > MaxRoll) { rollDiff = MaxRoll - Roll; }
				else if(targetRoll < -MaxRoll) { rollDiff = -MaxRoll - Roll; }

				Roll = Mathf.Clamp(Roll + rollDiff, -MaxRoll, MaxRoll);
			}
			else {
				rollDiff = 0f;
				if(Input.GetKeyDown(resetRollKey)) {
					ResetRoll();
				}
			}

			transform.Rotate(pitchDiff, yawDiff, -rollDiff, Space.Self);
			transform.Rotate(Vector3.up, Roll * BankTurnMultiplier * Time.deltaTime, Space.World);

			speedInput = Input.GetAxis(speedAxis);
			if(speedInput != 0f) {
				Speed = Mathf.Clamp(Speed + speedInput * Time.deltaTime * Acceleration, MinSpeed, MaxSpeed);
			}

			if(ApplyInverseMovement) {
				worldTransform.Translate(-Speed * Time.deltaTime * transform.forward, Space.World);
			}
			else {
				transform.Translate(Speed * Time.deltaTime * transform.forward, Space.World);
			}
		}

		private void ResetPitch() => resetPitchCoroutine ??= StartCoroutine(ResetPitch_Async());

		private void CancelResetPitch()
		{
			if(resetPitchCoroutine != null) {
				StopCoroutine(resetPitchCoroutine);
			}
			resetPitchCoroutine = null;
		}

		private IEnumerator ResetPitch_Async()
		{
			if(Pitch == 0f) {
				resetPitchCoroutine = null;
				yield break;
			}

			float resetPitchDiff = Pitch > 0f ? Time.deltaTime * -PitchRate : Time.deltaTime * PitchRate;

			while(Pitch != 0f) {
				if((Pitch > 0f && Pitch < -resetPitchDiff) || (Pitch < 0f && Pitch > -resetPitchDiff)) {
					resetPitchDiff = -Pitch;
					Pitch = 0f;
				}
				else {
					Pitch += resetPitchDiff;
				}
				transform.Rotate(Vector3.right, resetPitchDiff, Space.Self);
				yield return null;
			}

			transform.localEulerAngles = new Vector3(0f, transform.localEulerAngles.y, transform.localEulerAngles.z);
			Pitch = 0f;
			resetPitchCoroutine = null;
		}

		private void ResetRoll() => resetRollCoroutine ??= StartCoroutine(ResetRoll_Async());

		private void CancelResetRoll()
		{
			if(resetRollCoroutine != null) {
				StopCoroutine(resetRollCoroutine);
			}
			resetRollCoroutine = null;
		}

		private IEnumerator ResetRoll_Async()
		{
			if(Roll == 0f) {
				resetRollCoroutine = null;
				yield break;
			}

			float resetRollDiff = Roll > 0f ? Time.deltaTime * -RollRate : Time.deltaTime * RollRate;

			while(Roll != 0f) {
				if((Roll > 0f && Roll < -resetRollDiff) || (Roll < 0f && Roll > -resetRollDiff)) {
					resetRollDiff = -Roll;
					Roll = 0f;
				}
				else {
					Roll += resetRollDiff;
				}
				transform.Rotate(Vector3.back, resetRollDiff, Space.Self);
				yield return null;
			}

			transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, 0f);
			Roll = 0f;
			resetRollCoroutine = null;
		}
	}
}

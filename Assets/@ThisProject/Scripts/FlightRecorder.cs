using UnityEditor.Animations;
using UnityEngine;

namespace temp.iaw
{
  public class FlightRecorder : MonoBehaviour
  {
		[SerializeField] GameObject aircraft;
    [SerializeField] private AnimationClip aircraftClip, worldTransformClip;

    private GameObjectRecorder aircraftRecorder, worldTransformRecorder;

		private void Start()
		{
			GameObject worldTransformObj = GameObject.FindWithTag("WorldTransform");
			if(worldTransformObj == null) {
				Debug.LogError("No world transform found.");
				return;
			}

			aircraftRecorder = new GameObjectRecorder(aircraft);
			worldTransformRecorder = new GameObjectRecorder(worldTransformObj);
			aircraftRecorder.BindComponentsOfType<Transform>(aircraft, recursive: false);
			worldTransformRecorder.BindComponentsOfType<Transform>(worldTransformObj, recursive: false);
		}

		private void LateUpdate()
		{
			if(aircraftClip != null) {
				aircraftRecorder.TakeSnapshot(Time.deltaTime);
			}

			if(worldTransformClip != null) {
				worldTransformRecorder.TakeSnapshot(Time.deltaTime);
			}
		}

		private void OnDisable()
		{
			if(aircraftRecorder.isRecording && aircraftClip != null) {
				aircraftRecorder.SaveToClip(aircraftClip);
				aircraftRecorder.ResetRecording();
			}
			
			if(worldTransformRecorder.isRecording && worldTransformClip != null) { 
				worldTransformRecorder.SaveToClip(worldTransformClip);
				worldTransformRecorder.ResetRecording();
			}
		}
	}
}

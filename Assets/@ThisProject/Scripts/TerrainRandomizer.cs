using UnityEngine;

namespace temp.iaw
{
	public class TerrainRandomizer : MonoBehaviour
	{
		[Space(10)]
		[Tooltip("Number of copies to make of each base object prefab.")]
		public int CopyCount;
		[Tooltip("How far away from the origin in each dimension an object can be.")]
		public float MaxDistance = 500f;
		public float MaxHeight = 100f;
		public float MinScale = 0.5f;
		public float MaxScale = 10f;
		public GameObject[] BaseObjectPrefabs;

		private Transform worldTransform;

		private void Awake()
		{
			GameObject worldTransformObj = GameObject.FindWithTag("WorldTransform");
			if(worldTransformObj != null) {
				worldTransform = worldTransformObj.transform;
			}
			else {
				Debug.LogError("No world transform found.");
			}

			foreach(GameObject prefab in BaseObjectPrefabs) {
				for(int i = 0; i< CopyCount; i++ ) {
					GameObject newObj = Instantiate(prefab, worldTransform);
					newObj.transform.SetPositionAndRotation(new Vector3(Random.Range(-MaxDistance, MaxDistance), Random.Range(0f, MaxHeight), Random.Range(-MaxDistance, MaxDistance)), Random.rotationUniform);
					newObj.transform.localScale = new Vector3(Random.Range(MinScale, MaxScale), Random.Range(MinScale, MaxScale), Random.Range(MinScale, MaxScale));
				}
			}
		}
	}
}

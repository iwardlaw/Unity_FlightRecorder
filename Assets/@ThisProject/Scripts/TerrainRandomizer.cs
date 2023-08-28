using UnityEngine;

namespace temp.iaw
{
	public class TerrainRandomizer : MonoBehaviour
	{
		public Transform WorldTransform;

		[Space(10)]
		[Tooltip("Number of copies to make of each base object prefab.")]
		public int CopyCount;
		[Tooltip("How far away from the origin in each dimension an object can be.")]
		public float MaxDistance = 500f;
		public float MaxHeight = 100f;
		public float MinScale = 0.5f;
		public float MaxScale = 10f;
		public GameObject[] BaseObjectPrefabs;

		private void Awake()
		{
			foreach(GameObject prefab in BaseObjectPrefabs) {
				for(int i = 0; i< CopyCount; i++ ) {
					GameObject newObj = Instantiate(prefab, WorldTransform);
					newObj.transform.SetPositionAndRotation(new Vector3(Random.Range(-MaxDistance, MaxDistance), Random.Range(0f, MaxHeight), Random.Range(-MaxDistance, MaxDistance)), Random.rotationUniform);
					newObj.transform.localScale = new Vector3(Random.Range(MinScale, MaxScale), Random.Range(MinScale, MaxScale), Random.Range(MinScale, MaxScale));
				}
			}
		}
	}
}

using UnityEngine;

public class MeshCombiner : MonoBehaviour {

	public int GetMeshCount() {		
		MeshFilter[] filters = GetComponentsInChildren<MeshFilter>();		
		Debug.Log(name + " mesh count: " + filters.Length);
		return filters.Length;		
	}

	public void CombineMeshes() {

		MeshFilter[] filters = GetComponentsInChildren<MeshFilter>();		
		Debug.Log(name + " about to combine " + filters.Length + " meshes.");

		Quaternion oldRotation = transform.rotation;
		Vector3 oldPosition = transform.position;
		transform.rotation = Quaternion.identity;
		transform.position = Vector3.zero;

		CombineInstance[] combiners = new CombineInstance[filters.Length];

		for (int i = 0; i < filters.Length; i++) {

			if (transform == filters[i].transform) 
				continue;

			combiners[i].subMeshIndex = 0;
            combiners[i].mesh = filters[i].sharedMesh;
			combiners[i].transform = filters[i].transform.localToWorldMatrix;

			filters[i].gameObject.SetActive(false);
		}

		Mesh finalMesh = new Mesh();
		finalMesh.CombineMeshes(combiners);
		GetComponent<MeshFilter>().sharedMesh = finalMesh;

		transform.rotation = oldRotation;
		transform.position = oldPosition;

		Debug.Log(name + " combine complete.");
	}
}

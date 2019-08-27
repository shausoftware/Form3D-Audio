using UnityEditor;

public static class MeshCombinerEditor {
		
	[MenuItem("CONTEXT/MeshCombiner/Show Mesh Count")]
	public static void ShowMeshCount (MenuCommand menuCommand) {
		MeshCombiner mc = menuCommand.context as MeshCombiner;
		EditorUtility.DisplayDialog("GameObject has " + mc.GetMeshCount() + " meshes.", null, null);
	}

	[MenuItem("CONTEXT/MeshCombiner/Combine Meshes")]
	public static void CombineMeshes (MenuCommand menuCommand) {
		MeshCombiner mc = menuCommand.context as MeshCombiner;
		mc.CombineMeshes();
	}
}
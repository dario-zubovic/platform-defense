using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Tilemaps;

// utility for rotating selected tiles in scene view with right mouse click

[InitializeOnLoad]
public static class TileRotation {
	static TileRotation() {
		SceneView.onSceneGUIDelegate += Update;
	}

	private static void Update(SceneView sceneView) {
		if(EditorApplication.isPlayingOrWillChangePlaymode) {
			return;
		}

		if(!GridSelection.active || GridSelection.target == null) {
			return;
		}

		if (Event.current == null || Event.current.type != EventType.MouseUp || Event.current.button != 1) {
			return;
		}

		Tilemap tilemap = GridSelection.target.GetComponent<Tilemap>();
		if(tilemap == null) {
			return;
		}

		foreach(Vector3Int pos in GridSelection.position.allPositionsWithin) {
			TileBase tile = tilemap.GetTile(pos);
			if(tile == null) {
				continue;
			}

			Matrix4x4 matrix = tilemap.GetTransformMatrix(pos);
			matrix = matrix * Matrix4x4.Rotate(Quaternion.Euler(0, 0, 90));
			tilemap.SetTransformMatrix(pos, matrix);
		}
	}
}

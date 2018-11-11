using UnityEngine;
using UnityEditor;
using UnityEngine.Tilemaps;

public class TileImporter : EditorWindow {
    public Tile[] tiles;
    
    private Tilemap tilemap;
    private int size;

    [MenuItem("Custom/Tile importer")]
    public static void Open() {
        TileImporter window = EditorWindow.GetWindow<TileImporter>();
        window.Show();
    }

    public void OnGUI() {
        this.size = EditorGUILayout.IntField("Size", this.size);

        this.tilemap = (Tilemap) EditorGUILayout.ObjectField("Tilemap", this.tilemap, typeof(Tilemap), false);
        
        SerializedObject obj = new SerializedObject((ScriptableObject ) this);
        EditorGUILayout.PropertyField(obj.FindProperty("tiles"), new GUIContent("Tiles"), true);
        obj.ApplyModifiedProperties();

        if(GUI.Button(EditorGUILayout.GetControlRect(), "Sort")) {


            for(int i = 0; i < this.tiles.Length; i++) {
                Vector3Int pos = new Vector3Int(i % this.size, this.size - i / this.size, 0);
                this.tilemap.SetTile(pos, this.tiles[i]);
            }

            this.tilemap.RefreshAllTiles();
            this.tilemap.ResizeBounds();

        }
    }
}
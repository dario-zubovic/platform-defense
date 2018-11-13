using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SpriteAnimation))]
public class SpriteAnimationPreview : Editor  {
    private int frameNum;
    private float lastFrameTime;

    public override bool HasPreviewGUI() {
        return true;
    }

    public override void OnPreviewGUI(Rect r, GUIStyle background) {
        SpriteAnimation target = (SpriteAnimation)this.target;

        if(target == null || target.frames == null || target.frames.Count == 0) {
            return;
        }

        if(this.frameNum >= target.frames.Count) {
            this.frameNum = 0;
        }

        if(Time.time - this.lastFrameTime > target.framerate) {
            this.frameNum = (this.frameNum + 1) % target.frames.Count;
            this.lastFrameTime = Time.time;
        }

        Sprite frame = target.frames[this.frameNum];
        if(frame == null) {
            return;
        }

        Vector2 fullSize = new Vector2(frame.texture.width, frame.texture.height);

        Rect coords = frame.textureRect;
        coords.x /= fullSize.x;
        coords.width /= fullSize.x;
        coords.y /= fullSize.y;
        coords.height /= fullSize.y;

        GUI.DrawTextureWithTexCoords(r, frame.texture, coords);
        
        if(Event.current.type == EventType.Repaint) {
            EditorUtility.SetDirty(this.target);
        }
    }
}
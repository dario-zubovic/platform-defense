using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewSpriteAnimation", menuName = "SpriteAnimation", order = 1)]
public class SpriteAnimation : ScriptableObject {
    public float framerate;
    public List<Sprite> frames;
}
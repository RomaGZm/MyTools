using UnityEngine;

[CreateAssetMenu(fileName = "SpriteSheetConfig", menuName = "Tools/Sprite Sheet Config")]
public class SpriteSheetConfig : ScriptableObject
{
    public Sprite[] sprites;

    [Header("Grid")]
    public int columns = 5;
    public int rows = 1;

    [Header("Spacing")]
    public int padding = 2;

    [Header("Offset")]
    public int offsetLeft = 0;
    public int offsetRight = 0;
    public int offsetTop = 0;
    public int offsetBottom = 0;

    [Header("Background")]
    public bool transparentBackground = true;
    public Color backgroundColor = new Color(0, 0, 0, 0);

    [Header("Output")]
    public string pngPath = "Assets/generated_spritesheet.png";
}
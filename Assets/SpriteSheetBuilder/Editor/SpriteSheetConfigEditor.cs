#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.IO;

[CustomEditor(typeof(SpriteSheetConfig))]
public class SpriteSheetConfigEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        SpriteSheetConfig config = (SpriteSheetConfig)target;

        GUILayout.Space(10);

        if (GUILayout.Button("PACK SPRITES"))
        {
            Pack(config);
        }
    }
    void EnsureReadable(Texture2D texture)
    {
        string path = AssetDatabase.GetAssetPath(texture);
        TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;

        if (importer != null && !importer.isReadable)
        {
            importer.isReadable = true;
            importer.SaveAndReimport();
            Debug.Log("Enabled Read/Write for: " + texture.name);
        }
    }
    void Pack(SpriteSheetConfig config)
    {
        if (config.sprites == null || config.sprites.Length == 0)
        {
            Debug.LogError("No sprites assigned!");
            return;
        }

        int columns = Mathf.Max(1, config.columns);
        int spriteCount = config.sprites.Length;

        // ---------- CALCULATE SIZE ----------
        int totalWidth = 0;
        int totalHeight = config.offsetTop + config.offsetBottom;

        int currentRowWidth = 0;
        int currentRowHeight = 0;
        int maxWidth = 0;

        for (int i = 0; i < spriteCount; i++)
        {
            Sprite sprite = config.sprites[i];
            int w = (int)sprite.rect.width;
            int h = (int)sprite.rect.height;

            currentRowWidth += w;
            if (i % columns != 0)
                currentRowWidth += config.padding;

            currentRowHeight = Mathf.Max(currentRowHeight, h);

            bool endOfRow = ((i + 1) % columns == 0) || i == spriteCount - 1;

            if (endOfRow)
            {
                maxWidth = Mathf.Max(maxWidth, currentRowWidth);
                totalHeight += currentRowHeight;

                if (i < spriteCount - 1)
                    totalHeight += config.padding;

                currentRowWidth = 0;
                currentRowHeight = 0;
            }
        }

        totalWidth = maxWidth + config.offsetLeft + config.offsetRight;

        Texture2D sheet = new Texture2D(totalWidth, totalHeight, TextureFormat.RGBA32, false);

        // ---------- BACKGROUND ----------
        Color fill = config.transparentBackground ? new Color(0, 0, 0, 0) : config.backgroundColor;
        Color[] bg = new Color[totalWidth * totalHeight];
        for (int i = 0; i < bg.Length; i++)
            bg[i] = fill;

        sheet.SetPixels(bg);

        // ---------- DRAW ----------
        int xPos = config.offsetLeft;
        int yPos = totalHeight - config.offsetTop;

        currentRowHeight = 0;

        for (int i = 0; i < spriteCount; i++)
        {
            Sprite sprite = config.sprites[i];
            Texture2D sourceTex = sprite.texture;
            Rect r = sprite.textureRect;

            int w = (int)r.width;
            int h = (int)r.height;

            if (i % columns == 0)
            {
                if (i != 0)
                    yPos -= currentRowHeight + config.padding;

                xPos = config.offsetLeft;
                currentRowHeight = 0;
            }

            EnsureReadable(sourceTex);
            Color[] pixels = sourceTex.GetPixels(
                (int)r.x,
                (int)r.y,
                w,
                h
            );

            sheet.SetPixels(xPos, yPos - h, w, h, pixels);

            xPos += w + config.padding;
            currentRowHeight = Mathf.Max(currentRowHeight, h);
        }

        sheet.Apply();

        byte[] png = sheet.EncodeToPNG();
        File.WriteAllBytes(config.pngPath, png);

        AssetDatabase.Refresh();

        Debug.Log("Sprite sheet saved: " + config.pngPath);
    }
}
#endif
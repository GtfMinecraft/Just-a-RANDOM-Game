using UnityEngine;
using System.Collections.Generic;
using System.IO;

#if UNITY_EDITOR
using UnityEditor;
#endif

#if UNITY_EDITOR
public class TextureAutoApplier : AssetPostprocessor
{
    public class ApplyTextures
    {
        public string s;
        public Material m;
    }

    public static List<ApplyTextures> textures = new List<ApplyTextures>();

    string TruncatePath(string s)
    {
        if (s.Contains("_norm"))
        {
            s = s.Substring(0, s.IndexOf("_norm"));
        }
        else if (s.Contains("_diff"))
        {
            s = s.Substring(0, s.IndexOf("_diff"));
        }
        else if (s.Contains("_disp"))
        {
            s = s.Substring(0, s.IndexOf("_disp"));
        }
        else if (s.Contains("_occl"))
        {
            s = s.Substring(0, s.IndexOf("_occl"));
        }
        else if (s.Contains("_rough"))
        {
            s = s.Substring(0, s.IndexOf("_rough"));
        }
        else if (s.Contains("_emis"))
        {
            s = s.Substring(0, s.IndexOf("_emis"));
        }

        return s;
    }

    void OnPreprocessTexture()
    {
        if (assetPath.Contains("_norm"))
        {
            TextureImporter textureImporter = (TextureImporter)assetImporter;
            textureImporter.textureType = TextureImporterType.NormalMap;
            textureImporter.convertToNormalmap = false;
        }
    }

    // This method is called before the texture is imported
    void OnPostprocessTexture(Texture2D texture)
    {
        ApplyTextures temp = new ApplyTextures();
        temp.s = assetPath;

        string[] folder = new string[] { Path.GetFullPath(Path.Combine(assetPath, "../../")) };

        if (folder[0].StartsWith(Application.dataPath.Substring(0, 2)))
        {
            folder[0] = "Assets" + folder[0].Substring(Application.dataPath.Length) + "Materials";
        }

        if (folder[0] == "Assets\\Materials" || !Directory.Exists(folder[0]))
        {
            textures.Clear();
            return;
        }

        var guids = AssetDatabase.FindAssets("t:Material", folder);

        foreach (var g in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(g);
            var m = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (m.name.Equals(TruncatePath(texture.name)))
            {
                temp.m = m;
                textures.Add(temp);
                return;
            }
        }
    }

    private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    {
        foreach (var i in textures)
        {
            var t = AssetDatabase.LoadAssetAtPath<Texture>(i.s);
            if (i.s.Contains("_norm"))
            {
                i.m.SetTexture("_BumpMap", t);
            }
            else if (i.s.Contains("_diff"))
            {
                i.m.SetTexture("_BaseMap", t);
            }
            else if (i.s.Contains("_disp"))
            {
                i.m.SetTexture("_ParallaxMap", t);
            }
            else if (i.s.Contains("_occl"))
            {
                i.m.SetTexture("_OcclusionMap", t);
            }
            else if (i.s.Contains("_rough"))
            {
                i.m.SetTexture("_MetallicGlossMap", t);
            }
            else if (i.s.Contains("_emis"))
            {
                i.m.SetTexture("_EmissionMap", t);
            }
        }
        AssetDatabase.SaveAssets();
        textures.Clear();
    }
}
#endif
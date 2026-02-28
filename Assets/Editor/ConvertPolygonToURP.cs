using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

public class ConvertPolygonToURP
{
    [MenuItem("Tools/一键转换 Polygon 材质到 URP/Lit")]
    public static void ConvertAll()
    {
        string[] guids = AssetDatabase.FindAssets("t:Material", new[] { "Assets/PolygonFantasyHeroCharacters" });
        
        int count = 0;
        Shader urpLit = Shader.Find("Universal Render Pipeline/Lit");
        
        if (urpLit == null)
        {
            EditorUtility.DisplayDialog("错误", "找不到 URP/Lit Shader，请确保 URP 已安装", "确定");
            return;
        }
        
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            Material mat = AssetDatabase.LoadAssetAtPath<Material>(path);
            
            if (mat != null && mat.shader != null)
            {
                // 保存原始参数
                Color baseColor = Color.white;
                if (mat.HasProperty("_Color_Primary"))
                    baseColor = mat.GetColor("_Color_Primary");
                else if (mat.HasProperty("_Color"))
                    baseColor = mat.GetColor("_Color");
                
                Texture baseMap = null;
                if (mat.HasProperty("_Texture"))
                    baseMap = mat.GetTexture("_Texture");
                
                float metallic = 0;
                if (mat.HasProperty("_Metallic"))
                    metallic = mat.GetFloat("_Metallic");
                
                float smoothness = 0.5f;
                if (mat.HasProperty("_Smoothness"))
                    smoothness = mat.GetFloat("_Smoothness");
                
                // 改 Shader
                mat.shader = urpLit;
                
                // 恢复参数
                mat.SetColor("_BaseColor", baseColor);
                if (baseMap != null)
                    mat.SetTexture("_BaseMap", baseMap);
                mat.SetFloat("_Metallic", metallic);
                mat.SetFloat("_Smoothness", smoothness);
                
                EditorUtility.SetDirty(mat);
                count++;
                Debug.Log($"✓ 已转换: {path}");
            }
        }
        
        AssetDatabase.SaveAssets();
        EditorUtility.DisplayDialog("成功！", $"已转换 {count} 个材质到 URP/Lit\n\n提示: 如果材质显示不对，请手动调整颜色参数", "确定");
    }
}

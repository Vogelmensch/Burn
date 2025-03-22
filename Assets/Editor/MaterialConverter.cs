using UnityEditor;
using UnityEngine;

public class MaterialConverter : EditorWindow
{
    [MenuItem("Tools/Convert Materials to URP")]
    static void ConvertMaterials()
    {
        string[] materialGUIDs = AssetDatabase.FindAssets("t:Material");
        foreach (string guid in materialGUIDs)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            Material material = AssetDatabase.LoadAssetAtPath<Material>(path);
            
            // Überprüfe, ob es ein Built-in Shader ist
            if (material.shader.name.StartsWith("Legacy Shaders/") || 
                material.shader.name.StartsWith("Standard") ||
                material.shader.name.StartsWith("Particles/"))
            {
                // Konvertiere zu URP-Shader basierend auf dem Typ
                if (material.shader.name.Contains("Particle"))
                {
                    material.shader = Shader.Find("Universal Render Pipeline/Particles/Unlit");
                }
                else
                {
                    material.shader = Shader.Find("Universal Render Pipeline/Lit");
                }
                
                Debug.Log("Converted: " + material.name);
                EditorUtility.SetDirty(material);
            }
        }
        AssetDatabase.SaveAssets();
        Debug.Log("Conversion completed!");
    }
}
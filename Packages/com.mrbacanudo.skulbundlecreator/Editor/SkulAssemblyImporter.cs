using System.IO;
using Mono.Cecil;
using UnityEditor;
using UnityEngine;

public class SkulAssemblyImporter
{
    // List of all
    // They have a specific order that allows
    static readonly string[] Assemblies = {
        "Plugins.Common.dll",
        "Assembly-CSharp-firstpass.dll",
        "BehaviorDesigner.Runtime.dll",
        "InControl.dll",
        "Microsoft.CSharp.dll",
        "Mono.Security.dll",
        "Patchwork.Attributes.dll",
        "Platforms.Common.dll",
        "Platforms.dll",
        "Platforms.Steam.dll",
        "Plugins.Animator.dll",
        "Plugins.Data.dll",
        "Plugins.ObjectPool.dll",
        "Plugins.PhysicsUtils.dll",
        "Plugins.Singletons.dll",
        "spine-unity.dll",
        "System.Configuration.dll",
        "System.Core.dll",
        "System.dll",
        "System.Xml.dll",
        "Tilemap.dll",
        "Unity.2D.PixelPerfect.dll",
        "Unity.Addressables.dll",
        "Unity.Burst.dll",
        "Unity.Mathematics.dll",
        "Unity.RenderPipelines.Core.Runtime.dll",
        "Unity.RenderPipeline.Universal.ShaderLibrary.dll",
        "Unity.RenderPipelines.Universal.Runtime.dll",
        "Unity.ResourceManager.dll",
        "Unity.TextMeshPro.dll",
        "UnityEngine.UI.dll",
        "UserInput.dll",
    };

    [MenuItem("Assets/Import Skul Assemblies")]
    static void ImportSkulAssemblies()
    {
        var bundleDir = "Assets/Assemblies/";
        Directory.CreateDirectory(bundleDir);

        string path = EditorUtility.OpenFilePanel("Select Skul Assembly", @"C:\Program Files (x86)\Steam\steamapps\common\Skul\Skul_Data\Managed", "dll");

        if (path.Length == 0)
        {
            return;
        }

        var dir = Path.GetDirectoryName(path) + "/";

        // Import dependencies for the game's Assembly.
        // We disable them for editor usage, so they don't have conflicts with Unity's packages.
        foreach (var assemblyPath in Assemblies)
        {
            var source = dir + assemblyPath;
            var dest = bundleDir + assemblyPath;

            if (!File.Exists(dest))
            {
                File.Copy(source, dest);
            }

            AssetDatabase.ImportAsset(dest);

            var importer = AssetImporter.GetAtPath(dest);

            if (importer is null)
            {
                Debug.LogWarning("Could not import " + dest);
                continue;
            }

            var plugin = (PluginImporter)importer;
            plugin.SetExcludeEditorFromAnyPlatform(true);
        }

        // Import main assembly, but renaming it so it doesn't conflict with Unity's expectations of where that name is generated.
        // This is undocumented behavior, and couldn't find another workaround.
        // I hate Unity.
        {
            var assemblyPath = "Assembly-CSharp.dll";
            var newName = "Assembly-CSharp2";

            var source = dir + assemblyPath;

            var resolver = new DefaultAssemblyResolver();
            resolver.AddSearchDirectory(dir);

            var readerParameters = new ReaderParameters
            {
                AssemblyResolver = resolver
            };

            var assembly = AssemblyDefinition.ReadAssembly(source, readerParameters);

            assembly.Name = new AssemblyNameDefinition(newName, System.Version.Parse("0.0.0.0"));

            assembly.MainModule.Name = newName;

            var temp = dir + newName + ".dll";
            assembly.Write(temp);

            var dest = bundleDir + newName + ".dll";
            File.Delete(dest);
            File.Move(temp, dest);
            AssetDatabase.ImportAsset(dest);
        }
    }
}

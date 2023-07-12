using System.IO;
using UnityEditor;
using UnityEngine;

public class ModdedAssetBundleCreator
{
    [MenuItem("Assets/Build Modded Asset Bundles")]
    static void BuildModdedAssetBundles()
    {
        var bundleDir = "Assets/AssetBundles";
        var exportDir = bundleDir + "/Exported";
        Directory.CreateDirectory(exportDir);

        var manifest = BuildPipeline.BuildAssetBundles(bundleDir, BuildAssetBundleOptions.UncompressedAssetBundle, BuildTarget.StandaloneWindows64);

        // Assembly-CSharp2.dll with length in a byte array
        byte[] original = { 0x14, 0x00, 0x00, 0x00, 0x41, 0x73, 0x73, 0x65, 0x6d, 0x62, 0x6c, 0x79, 0x2d, 0x43, 0x53, 0x68, 0x61, 0x72, 0x70, 0x32, 0x2e, 0x64, 0x6c, 0x6c };

        // Assembly-CSharp.dll replacement with corrected length and appending a 0x00 to maintain it
        byte[] replacer = { 0x13, 0x00, 0x00, 0x00, 0x41, 0x73, 0x73, 0x65, 0x6d, 0x62, 0x6c, 0x79, 0x2d, 0x43, 0x53, 0x68, 0x61, 0x72, 0x70, 0x2e, 0x64, 0x6c, 0x6c, 0x00 };

        foreach (var bundleName in manifest.GetAllAssetBundles())
        {
            byte[] data = File.ReadAllBytes(bundleDir + "/" + bundleName);
            byte[] newData = AssemblyNameEditor.ReplaceAllOccurrences(data, original, replacer);

            File.WriteAllBytes(exportDir + "/" + bundleName, newData);
        }
    }
}

# Unity Modding Project for Skul: the Hero Slayer

This is an empty project template to enable development of mod Asset Bundles

## How to use it

1. Open this project on Unity
2. Go to `Assets -> Import Skul Assemblies` and find Skul's `Assembly-CSharp.dll`
3. Use the game's components on your prefabs
4. Add your prefab to an AssetBundle
5. Run `Assets -> Build Modded Asset Bundles`
6. Use the asset bundles generated on the `Assets/AssetBundles/Exported` folder

## How it works

Instead of decompiling and recompiling the game's code, we instead import the assemblies directly into Unity, as plugins.

However, when we try to import the game's assembly, it all seems OK, until we try to import a component from it: 
we get "Can't add script ... because the script class cannot be found". 
This happens because Unity's default export is called `Assembly-CSharp`, and — what I assume — an undocumented behavior
of the engine is that if you try to import something from `Assembly-CSharp`, Unity will try to find it in your scripts instead.

So we rename the file to `Assembly-CSharp2`. But renaming it by itself is not enough: we need to rename the assembly inside its metadata!
Then we use the Mono.Cecil library to rename it and import the new one.

Due to compatibility issues, we create an empty project, import all of the game's dependencies and disable editor usage for them.
If you find a very cryptic error from a Unity package, it's this kind of collision and Unity being bad at detecting them.

To export the asset bundles, we now have a problem: all `MonoScript` references now try to point to `Assembly-CSharp2.dll`!
That's where our second script comes in: we first export an uncompressed AssetBundle. Then, we find all occurrences of `Assembly-CSharp2.dll`
(plus the string length) and replace them back with `Assembly-CSharp.dll`. Simple like that!

Note the exported bundle won't be compressed, so you might want to post-process it to compress it.
We might use a tool like [Unity3D Compressor](https://github.com/IllusionMods/Unity3DCompressor) or use its dependency, 
`AssetTools.NET`, to just load and re-export our modified bundle in a two-line addition to the `ModdedAssetBundleCreator` script.

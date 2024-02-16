using System;
using System.IO;
using System.Linq;
using System.Reflection;
using BepInEx;
using UnityEngine;
namespace Pineapler.Utils;

public static class LibraryLoader {
    public static bool LoadEarlyRuntimeDependencies() {
        try {
            string current = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;
            string deps = Path.Combine(current, "RuntimeDeps");

            // Early load all DLLs in RuntimeDeps
            foreach (string file in Directory.GetFiles(deps, "*.dll")) {
                string filename = Path.GetFileName(file);

                Log.Info($"  Early loading {filename}");

                try {
                    Assembly.LoadFile(file);
                }
                catch (Exception ex) {
                    Log.Error($"Failed to early load {filename}: {ex.Message}");
                }
            }
        }
        catch (Exception ex) {
            Log.Error($"Unexpected error while loading early runtime dependencies: {ex.Message}");
            return false;
        }

        return true;
    }

    public static bool SetupSubsystems() {
        bool ok = true;
        
        string root = Path.Combine(Paths.GameRootPath, Application.productName + "_Data");
        string subsys = Path.Combine(root, "UnitySubsystems");
        if (!Directory.Exists(subsys)) {
            Directory.CreateDirectory(subsys);
        }
       
        // OpenVR
        string openVr = Path.Combine(subsys, "XRSDKOpenVR");
        if (!Directory.Exists(openVr)) {
            Directory.CreateDirectory(openVr);
        }
        string manifestOpenVr = Path.Combine(openVr, "UnitySubsystemsManifest.json");
        if (!File.Exists(manifestOpenVr)) {
            File.WriteAllText(manifestOpenVr, Properties.Resources.OpenVR_UnitySubsystemsManifest);
            ok = false;
        }
        
        // OpenXR
        string openXr = Path.Combine(subsys, "UnityOpenXR");
        if (!Directory.Exists(openXr)) {
            Directory.CreateDirectory(openXr);
        }
        string manifestOpenXr = Path.Combine(openXr, "UnitySubsystemsManifest.json");
        if (!File.Exists(manifestOpenXr)) {
            File.WriteAllText(manifestOpenXr, Properties.Resources.OpenXR_UnitySubsystemsManifest);
            ok = false;
        }
        return ok;
    }
    
    /// Set up runtime assets so that the game can launch with VR support.
    public static bool SetupPlugins() {
        bool ok = true;
        string current = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;
        string root = Path.Combine(Paths.GameRootPath, Application.productName + "_Data");
        string src = Path.Combine(current, "RuntimeDeps/Plugins");
        string dst = Path.Combine(root, "Plugins/x86_64");
            
        try {
            CopyDirectory(src, dst, true, false);
        }
        catch (Exception e) {
            Log.Error("Could not copy plugin DLL: " + e.Message);
            ok = false;
        }

        return ok;
    }


    public static bool SetupStreamingAssets() {
        bool ok = true;
        string current = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;
        string root = Path.Combine(Paths.GameRootPath, Application.productName + "_Data");
        string src = Path.Combine(current, "RuntimeDeps/StreamingAssets");
        string dst = Path.Combine(root, "StreamingAssets");
        try {
            CopyDirectory(src, dst, true, false);
        }
        catch (Exception e) {
            Log.Error("Error copying StreamingAssets: " + e.Message);
            ok = false;
        }

        return ok;
    }

    // Mostly from MS docs (why isn't this in the standard library), last write check mine.
    private static void CopyDirectory(string sourceDir, string destinationDir, bool recursive, bool overwriteNewer) {
        var dir = new DirectoryInfo(sourceDir);

        if (!dir.Exists)
            throw new DirectoryNotFoundException($"Source directory not found: {dir.FullName}");

        DirectoryInfo[] dirs = dir.GetDirectories();

        Directory.CreateDirectory(destinationDir);

        foreach (FileInfo file in dir.GetFiles()) {
            string targetFilePath = Path.Combine(destinationDir, file.Name);
            if (!overwriteNewer) {
                FileInfo dstFile = new FileInfo(targetFilePath);
                if (dstFile.Exists && dstFile.LastWriteTime >= file.LastWriteTime) {
                    continue;
                }
            }
            file.CopyTo(targetFilePath);
        }

        if (recursive) {
            foreach (DirectoryInfo subDir in dirs)
            {
                string newDestinationDir = Path.Combine(destinationDir, subDir.Name);
                CopyDirectory(subDir.FullName, newDestinationDir, true, overwriteNewer);
            }
        }
    }
}
$package = "bin/package/VirtualOrc";
$mod_dir = "$env:ORC_MASSAGE_DIR/BepInEx/plugins";
Copy-Item $package $mod_dir -Recurse -Force;
Start-Process "$env:ORC_MASSAGE_DIR/OrcMassage.exe";
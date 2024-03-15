$build_dll = "bin/Debug/netstandard2.1/VirtualOrc.dll";
$package = "bin/package/VirtualOrc";

if (!(Test-Path -path $package)) {New-Item $package -Type Directory};
Copy-Item "Libs/RuntimeDeps" -Destination $package -Recurse -Force;
Copy-Item "StreamingAssets" -Destination "$package/RuntimeDeps" -Recurse -Force;
Copy-Item $build_dll -Destination $package -Force;
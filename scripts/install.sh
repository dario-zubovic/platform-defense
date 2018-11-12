#! /bin/sh

# butler
echo "Downloading butler..."
wget https://broth.itch.ovh/butler/linux-amd64/LATEST/archive/default -O butler.zip
unzip butler.zip -d ./
sudo chmod +x ./butler

# unity
echo "Downloading Unity..."
curl https://netstorage.unity3d.com/unity/65e0713a5949/MacEditorInstaller/Unity-2018.2.15f1.pkg -o unity.pkg
curl https://netstorage.unity3d.com/unity/65e0713a5949/MacEditorTargetInstaller/UnitySetup-WebGL-Support-for-Editor-2018.2.15f1.pkg -o webglsupport.pkg
sudo installer -dumplog -package unity.pkg -target /
sudo installer -dumplog -package webglsupport.pkg -target /

# misc
mkdir ~/PD
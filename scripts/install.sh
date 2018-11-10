#! /bin/sh

# butler
echo "Downloading butler..."
curl -o butler.zip https://broth.itch.ovh/butler/linux-amd64/LATEST/archive/default
echo `md5sum butler.zip`
unzip butler.zip -d ./
chmod +x ./butler

# unity
echo "Downloading Unity..."
curl https://beta.unity3d.com/download/dad990bf2728/UnitySetup-2018.2.7f1
sudo chmod +x ./UnitySetup-2018.2.7f1
echo y | ./UnitySetup-2018.2.7f1 --unattended --install-location=/opt/Unity/Editor --verbose --download-location=/tmp/unity --components=Unity,WebGL
mkdir -p $HOME/.local/share/unity3d/Certificates/

# misc
mkdir ~/PD
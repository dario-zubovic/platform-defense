#! /bin/sh

# butler
echo "Downloading butler..."
wget https://broth.itch.ovh/butler/linux-amd64/LATEST/archive/default -O butler.zip
echo `md5sum butler.zip`
unzip butler.zip -d ./
sudo chmod +x ./butler

# unity
echo "Downloading Unity..."
curl https://beta.unity3d.com/download/dad990bf2728/UnitySetup-2018.2.7f1
sudo chmod +x ./UnitySetup-2018.2.7f1
echo y | ./UnitySetup-2018.2.7f1 --unattended --install-location=./Editor --verbose --download-location=/tmp/unity --components=Unity,WebGL
mkdir -p $HOME/.local/share/unity3d/Certificates/

# misc
mkdir ~/PD
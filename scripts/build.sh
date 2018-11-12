#! /bin/sh

echo "Performing build..."

/Applications/Unity/Unity.app/Contents/MacOS/Unity  -batchmode -nographics -buildTarget WebGL -logFile $(pwd)/unity.log -projectPath $(pwd) -executeMethod Build.PerformBuild -quit

echo "Log file from the build:"
cat $(pwd)/unity.log


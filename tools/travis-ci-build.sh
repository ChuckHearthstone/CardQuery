#!/bin/sh
echo "Changing to /src directory..."
cd CardQuery
echo "Executing MSBuild.exe begin command..."
mono ../tools/sonar/SonarScanner.MSBuild.exe begin /o:"chuckhearthstone" /k:"ChuckHearthstone_CardQuery" /d:sonar.host.url="https://sonarcloud.io" /d:sonar.verbose=true /d:sonar.login=${SONAR_TOKEN}
echo "Running build..."
msbuild /p:Configuration=Release /t:Rebuild ../CardQuery.sln
echo "Executing MSBuild.exe end command..."
mono ../tools/sonar/SonarScanner.MSBuild.exe end /d:sonar.login=${SONAR_TOKEN}
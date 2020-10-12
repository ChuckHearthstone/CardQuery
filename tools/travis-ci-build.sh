#!/bin/sh
echo "Changing to /src directory..."
cd SonarqubeTest
echo "Executing MSBuild.exe begin command..."
mono ../tools/sonar/SonarScanner.MSBuild.exe begin /o:"chucktest" /k:"ChuckTest_SonarqubeTest" /d:sonar.host.url="https://sonarcloud.io" /d:sonar.verbose=true /d:sonar.login=${SONAR_TOKEN}
echo "Running build..."
msbuild /p:Configuration=Release /t:Rebuild ../SonarqubeTest.sln
echo "Executing MSBuild.exe end command..."
mono ../tools/sonar/SonarScanner.MSBuild.exe end /d:sonar.login=${SONAR_TOKEN}
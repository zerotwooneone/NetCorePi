This is just a project for trying out some dotnet core code on a Raspberry Pi 3.

# To publish
chmod 755 ./MyWebApp
dotnet publish -r linux-arm

# To Set up Raspberry Pi
sudo apt-get install curl libunwind8 gettext apt-transport-https

curl -o- https://raw.githubusercontent.com/creationix/nvm/v0.31.0/install.sh | bash

nvm install v6.9.0
nvm alias default v6.9.0



sudo pacman -S syncthing
sudo systemctl enable syncthing@${USER}.service
sudo systemctl start syncthing@${USER}.service

sudo pacman -S syncthing
sudo systemctl enable syncthing@${USER}.service
sudo systemctl start syncthing@${USER}.service

# symlink as below according to: https://forum.syncthing.net/t/discovery-failures-connection-refused-for-v4-and-v6/15553/3
sudo ln -sf /run/systemd/resolve/stub-resolv.conf /etc/resolv.conf
# and restart service will perhaps get things working ....
sudo systemctl restart systemd-networkd
sudo systemctl restart systemd-resolved
sudo systemctl restart syncthing@${USER}.service


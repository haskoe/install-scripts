# Using archinstall and you don't have an Ethernet connection: Remember iwd when selecting additional packages
 sudo systemctl start iwd
 sudo systemctl enable iwd

 iwctl device list
 iwctl station <device> scan
 iwctl station <device> get-networks
 iwctl station <device> get-networks
 iwctl station <device> connect <SSID>
 iwctl station <device> connect <SSID> --passphrase passphrase 

 sudo systemctl restart systemd-networkd

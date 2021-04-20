sudo pacman -S tailscale
sudo systemctl enable --now tailscaled
sudo tailscale up
# copy/paste URL in browser and login
ip addr show tailscale0

# now you can ping other hosts on the 'local' tailscale network
ping .....

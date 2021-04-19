#git@github.com:haskoe/install-scripts.git

sudo pacman -Sy --needed python-pip zsh-completions openssh inetutils keychain bash-completion mc cdrkit qemu fd zola ripgrep xorg-server xorg-apps lightdm-gtk-greeter i3-gaps firefox ranger i3status terminator base-devel thunar chromium xorg-xinit nodejs autorandr sshfs samba alsa-utils alsa-plugins pulseaudio pulseaudio-alsa pulseaudio-bluetooth pulseaudio-equalizer imagemagick pavucontrol
sudo systemctl enable lightdm
sudo systemctl start lightdm
# select i3 and login

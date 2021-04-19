#git@github.com:haskoe/install-scripts.git
# edit /etc/locale.gen manually

sudo pacman -Sy --needed python-pip zsh-completions openssh inetutils keychain bash-completion mc cdrkit qemu fd zola ripgrep xorg-server xorg-apps lightdm-gtk-greeter i3-gaps firefox ranger i3status terminator base-devel thunar chromium xorg-xinit nodejs autorandr sshfs samba alsa-utils alsa-plugins pulseaudio pulseaudio-alsa pulseaudio-bluetooth pulseaudio-equalizer imagemagick pavucontrol rxvt-unicode i3lock xautolock tk

sudo locale-gen
sudo localectl set-locale LANG=en_DK.UTF-8

unset LANG
source /etc/profile.d/locale.sh

sudo systemctl enable lightdm
sudo systemctl start lightdm
# select i3 and login

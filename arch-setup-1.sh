#!/bin/bash -e

# After installation from ARCH iso:
# Either clone, copy to USB drive and copy to ~/install-scripts
# or clone: git clone https://github.com/haskoe/install-scripts.git
# and then
# 1) edit ~/install-scripts/install-vars.sh
# 2) sh ~/install-scripts/arch-setup-1.sh
# 3) From login screen change to i3 (optional) and login 
# 4) sh ~/install-scripts/arch-setup-2.sh
# 5) Logout and Login (due to added group memberships)
# 6) sh ~/install-scripts/arch-setup-3.sh

. `dirname "$0"`/setup-vars.sh

[[ -z "$PREFERRED_LOCALE" ]] && echo "PREFERRED_LOCALE must be set in setup-vars.sh" && exit 1
[[ -z "$PREFERRED_KEYMAP" ]] && echo "PREFERRED_KEYMAP must be set in setup-vars.sh" && exit 1

sudo localectl --no-convert set-x11-keymap $PREFERRED_KEYMAP

# a temp subst variable is needed
SUBST="s/#${PREFERRED_LOCALE}/${PREFERRED_LOCALE}/g"
sudo perl -pibak -e $SUBST /etc/locale.gen
sudo locale-gen
sudo localectl set-locale LANG=${PREFERRED_LOCALE}
unset LANG
source /etc/profile.d/locale.sh

echo "new locale:$LANG"
[[ ! "$LANG"=="${PREFERRED_LOCALE}" ]] && echo "locale was not set correctly. exiting" && exit 1

sudo timedatectl set-ntp true

sudo pacman -Sy --needed python-pip zsh-completions openssh inetutils keychain bash-completion mc cdrkit qemu fd zola ripgrep xorg-server xorg-apps lightdm-gtk-greeter i3-gaps firefox ranger i3status terminator base-devel thunar chromium xorg-xinit nodejs autorandr sshfs samba alsa-utils alsa-plugins pulseaudio pulseaudio-alsa pulseaudio-bluetooth pulseaudio-equalizer imagemagick pavucontrol rxvt-unicode i3lock xautolock tk thunderbird gvfs-smb keepass tldr iwd

#eval keychain does not work with vscode and github auth
# but couldnt get gnome-keyring and libsecret to work either
#sudo pacman -Sy --needed gnome-keyring libsecret

sudo systemctl enable lightdm
sudo systemctl start lightdm


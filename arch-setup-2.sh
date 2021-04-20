#!/bin/bash

. `dirname "$0"`/setup-vars.sh

# abort if GH_USER is not set
[[ -z "$GH_USER" ]] && echo "GH_USER must be set" && exit 1

# abort if GH_EMAIL is not set
[[ -z "$GH_EMAIL" ]] && echo "GH_EMAIL must be set" && exit 1

# abort if i3 is not active display env
[[ ! "$XDG_CURRENT_DESKTOP"=="i3" ]] && echo "i3 is not running" && exit 1

grep -xq '^\..*\.bash\/\.bashrc' ~/.bashrc
RETVAL=$?
[[ $RETVAL -ne 0 ]] && echo ". $SCRIPTPATH/.bash/.bashrc $SCRIPTPATH" >>~/.bashrc
source ~/.bashrc

BAK_DIR=~/bak/i3
I3_CONFIG=~/.config/i3/config
[[ ! -d $BAK_DIR ]] && mkdir -p $BAK_DIR
[[ ! -f ${BAK_DIR}/config ]] && cp $I3_CONFIG $BAK_DIR
cp $SCRIPTPATH/i3/i3-config $I3_CONFIG

tee -a $I3_CONFIG <<-EOF

bindsym \$mod+Ctrl+l exec --no-startup-id $SCRIPTPATH/i3/i3-lock.sh
exec --no-startup-id $SCRIPTPATH/i3/i3-xautolock.sh
EOF

tee $SCRIPTPATH/i3/i3-xautolock.sh <<-EOF
#!/bin/sh -e

pkill xautolock
xautolock -time 5 -locker '$SCRIPTPATH/i3/i3-lock.sh' &
EOF

# autorandr
# xrandr --output HDMI1 --auto --output eDP1 --off
# autorandr --save docked

git config --global user.name $GH_USER
git config --global user.email $GH_EMAIL

sudo systemctl enable sshd
sudo systemctl start sshd

# node
mkdir -p $HOME/.npm-packages/bin
mkdir -p $HOME/.npm-packages/etc
mkdir -p $HOME/.npm-packages/lib/node_modules
mkdir -p $HOME/.npm-packages/share
# .npmrc
echo prefix=${HOME}/.npm-packages >~/.npmrc

# yay
git clone https://aur.archlinux.org/yay.git
cd yay
makepkg -si

systemctl --user start pulseaudio.service
systemctl --user start pulseaudio.socket
# pavucontrol

# vscode
yay -Sy visual-studio-code-bin
# extensions
code --install-extension eamodio.gitlens 
code --install-extension ritwickdey.LiveServer 
code --install-extension ckolkman.vscode-postgres 
code --install-extension geddski.macros 
code --install-extension ms-azuretools.vscode-docker 
code --install-extension ms-vscode-remote.vscode-remote-extensionpack 
code --install-extension ms-vscode-remote.remote-ssh 
code --install-extension ms-vscode-remote.remote-containers
code --install-extension vadimcn.vscode-lldb

# docker
sudo pacman -Sy --needed docker
sudo systemctl start docker
sudo systemctl enable docker
sudo usermod -a -G docker $USER
# doesnt work
#sudo su $USER

# KVM
# checks
LC_ALL=C lscpu | grep Virtualization
zgrep CONFIG_KVM /proc/config.gz
#
sudo pacman -Sy --needed virt-manager qemu vde2 ebtables dnsmasq bridge-utils openbsd-netcat libgovirt  dhclient  dmidecode  virt-viewer
# check
sudo modprobe kvm_intel
lsmod | grep kvm
#
sudo usermod -a -G libvirt $USER
sudo systemctl enable libvirtd.service
sudo perl -pibak -e 's/^#unix_sock_group/unix_sock_group/' /etc/libvirt/libvirtd.conf
sudo perl -pibak -e 's/^#unix_sock_rw_perms/unix_sock_rw_perms/' /etc/libvirt/libvirtd.conf
sudo systemctl start libvirtd.service
#sudo usermod --shell /bin/bash $USER

# rust
curl --proto '=https' --tlsv1.2 -sSf https://sh.rustup.rs | sh
source ~/.cargo/env

# zsh, sheldon, nerd-fonts
#yay nerd-fonts-complete
yay -Sy nerd-fonts-hermit
fc-cache -fv

#yay -Sy sheldon
cargo install sheldon
sheldon init --shell zsh
sheldon add base16 --github chriskempson/base16-shell
sheldon add zsh-autosuggestions --github zsh-users/zsh-autosuggestions --use '{{ name }}.zsh'
sheldon add autojump --github wting/autojump --dir bin --apply PATH source
sheldon add zsh-syntax-highlighting --github zsh-users/zsh-syntax-highlighting
#sheldon add z.lua --github skywind3000/z.lua
sheldon add enhancd --github b4b4r07/enhancd
sheldon add powerlevel10k --github romkatv/powerlevel10k
sheldon add oh-my-zsh --github "ohmyzsh/ohmyzsh"
sheldon remove enhancd
sheldon add autols --github desyncr/auto-ls

export ZDOTDIR=~/.config/zsh
[[ ! -d $ZDOTDIR ]] && mkdir $ZDOTDIR
echo "ZDOTDIR=${ZDOTDIR}" >~/.zshenv
cp $SCRIPTPATH/.zshrc $ZDOTDIR
cp $SCRIPTPATH/.p10k.zsh $ZDOTDIR
#test
#zsh

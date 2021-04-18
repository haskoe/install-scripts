#git@github.com:haskoe/install-scripts.git

SSH_HOSTS=()
PREFERRED_LOCALE_PREFIX=da_
PREFERRED_LOCALE=da_DK
GH_EMAIL= #$USER
GH_EMAIL= #henrik@haskoe.dk

# abort if GH_USER is not set
[[ -z "$GH_EMAIL" ]] && echo "GH_EMAIL must be sert" && exit 1

# abort if GH_EMAIL is not set
[[ -z "$GH_EMAIL" ]] && echo "GH_EMAIL must be sert" && exit 1

# abort if i3 is not active display env
[[ ! "$XDG_CURRENT_DESKTOP"=="i3" ]] && echo "i3 is not running" && exit 1

BAK_DIR=~/bak/i3
[[ ! -d $BAK_DIR ]] && mkdir -p $BAK_DIR
cp ~/.config/i3/config $BAK_DIR
cp ~/arch-setup/i3/i3-config ~/.config/i3/config

grep -q arch-setup ~/.bashrc
[[ ! "$?"=="0" ]] && echo ". ~/arch-setup/.bash/.bashrc" >>~/.bashrc
source ~/.bashrc

# autorandr
# xrandr --output HDMI1 --auto --output eDP1 --off
# autorandr --save docked

# locale
sudo perl -pibak -e 's/^#${PREFERRED_LOCALE_PREFIX}/${PREFERRED_LOCALE_PREFIX}/g' /etc/locale.conf
sudo locale-gen
sudo localectl set-locale LANG=${PREFERRED_LOCALE}.UTF-8

git config --global user.name $USER
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


#cp /etc/X11/xinit/xinitrc ~/.xinitrc
#perl -pibak -e 's/^exec /#exec/g' ~/.xinitrc
#tee -a ~/.xinitrc <<-EOF
#numlockx &
#exec i3
#EOF

# logout
# login

# X11
sudo localectl --no-convert set-x11-keymap dk

# yay
git clone https://aur.archlinux.org/yay.git
cd yay
makepkg -si

# vscode
yay visual-studio-code-bin
# extensions
code --install-extension eamodio.gitlens 
code --install-extension ritwickdey.LiveServer 
code --install-extension ckolkman.vscode-postgres 
code --install-extension geddski.macros 
code --install-extension ms-azuretools.vscode-docker 
code --install-extension ms-vscode-remote.vscode-remote-extensionpack 
code --install-extension ms-vscode-remote.remote-ssh 
code --install-extension ms-vscode-remote.remote-containers


# remote-ssh ssh config example
tee -a ~/.ssh/config <<-EOF

Host 10.0.0.3
 IdentityFile ~/.ssh/id_mx500
 HostName 10.0.0.3
 User $USER
EOF

# postgresql
sudo pacman -Sy --needed postgresql postgis
sudo -u postgres bash -c "initdb --locale $LANG -E UTF8 -D '/var/lib/postgres/data'"
sudo perl -pibak -e 's/ident/trust/' /var/lib/postgres/data/pg_hba.conf

sudo tee -a /var/lib/postgres/data/postgresql.conf <<-EOF
max_connections = 32
shared_buffers = 2GB
effective_cache_size = 6GB
work_mem = 128MB
maintenance_work_mem = 512MB
checkpoint_completion_target = 0.7
wal_buffers = 16MB
default_statistics_target = 100
EOF

sudo systemctl start postgresql
#systemctl enable postgresql
# giver fejl, men bruger oprettes
sudo -u postgres createuser -s -d $USER
# i stedet
# sudo -iu postgres
# createuser -s -d 
sudo systemctl stop postgresql
yay postgrest

# octave
sudo pacman -Sy --needed octave gcc-fortran
octave --eval "pkg install -forge control"
octave --eval "pkg install -forge signal"
# clone haskoe repo
# cd ....
# octave patient_plot_full_csi.m

# ssh
hostname=`hostname`
ssh_fname=id_$hostname
ssh-keygen -f ~/.ssh/${ssh_fname}
# todo: copy to all hosts in SSH_HOSTS
ssh-copy-id -i ~/.ssh/${ssh_fname}.pub $USER@......


# azure-repos
tee -a ~/.ssh/config <<-EOF

Host ssh.dev.azure.com
 IdentityFile ~/.ssh/id_hg
 IdentitiesOnly yes
EOF

mkdir -p ~/dev/azure-repos/misc
cd ~/dev/azure-repos/misc
git clone git@ssh.dev.azure.com:v3/heas0404/MISC/MISC misc
eval `keychain --agents ssh --eval ~/.ssh/id_hg`
bash ~/dev/azure-repos/misc/misc/git/status-all-branches.sh

# mode 2560 on older intel CPUs
HDMI=HDMI1
tee -a ~/mode-2560.sh <<-EOF
# 2560x1440 on older intel GPUs
xrandr --newmode "2560x1440_40.00"  201.00  2560 2720 2984 3408  1440 1443 1448 1476 +hsync +vsync
xrandr --addmode $HDMI 2560x1440_40.00
xrandr --output $HDMI --mode 2560x1440_40.00 --verbose
EOF

# docker
sudo pacman -Sy --needed docker
sudo systemctl start docker
sudo systemctl enable docker
sudo usermod -a -G docker $USER
sudo su $USER
docker run -it --rm archlinux bash -c "echo hello world"

# github repos
mkdir -p ~/dev/haskoe
cd ~/dev/haskoe
git clone https://github.com/haskoe/ecg_epilepsy.git

# node
mkdir -p $HOME/.npm-packages/bin
mkdir -p $HOME/.npm-packages/etc
mkdir -p $HOME/.npm-packages/lib/node_modules
mkdir -p $HOME/.npm-packages/share
# .npmrc
echo prefix=${HOME}/.npm-packages >~/.npmrc

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

virt-install  \
  --name arch-linux_testing \
  --memory 1024             \
  --vcpus=2,maxvcpus=4      \
  --cpu host                \
  --cdrom $HOME/Downloads/archlinux-2021.04.01-x86_64.iso \
  --disk size=20,format=qcow2  \
  --network user            \
  --virt-type kvm



# terraform prov.
sudo pacman -Sy --needed terraform
cd ~
#terraform init
mkdir -p ~/.local/share/terraform/plugins
mkdir -p ~/.local/share/terraform/plugins/registry.terraform.io/dmacvicar/libvirt/0.6.3/linux_amd64
yay terraform-provider-libvirt 
cp /usr/bin/terraform-provider-libvirt ~/.local/share/terraform/plugins/registry.terraform.io/dmacvicar/libvirt/0.6.3/linux_amd64
mkdir -p ~/projects/terraform
cd ~/projects/terraform
# make libvirt.tf in dir
terraform init

sudo usermod --shell /bin/bash $USER

# rust
curl --proto '=https' --tlsv1.2 -sSf https://sh.rustup.rs | sh
source ~/.cargo/env

# zsh/sheldon
yay nerd-fonts-complete
cargo install sheldon
sheldon init --shell zsh
sheldon add base16 --github chriskempson/base16-shell
sheldon add zsh-autosuggestions --github zsh-users/zsh-autosuggestions --use '{{ name }}.zsh'
sheldon add autojump --github wting/autojump --dir bin --apply PATH source
sheldon add zsh-syntax-highlighting --github zsh-users/zsh-syntax-highlighting
#sheldon add z.lua --github skywind3000/z.lua
sheldon add enhancd --github b4b4r07/enhancd
sheldon add powerlevel10k --github romkatv/powerlevel10k


export ZDOTDIR=~/.config/zsh/sheldon
[[ ! -d $ZDOTDIR ]] && mkdir $ZDOTDIR
echo 'eval "$(sheldon source)"' >$ZDOTDIR/.zshrc
echo "ZDOTDIR=${ZDOTDIR}" >~/.zshenv

....
# https://github.com/mattmc3/zdotdir.git
export ZDOTDIR=~/.config/zsh/mattmc3
git clone --recursive https://github.com/mattmc3/zdotdir.git $ZDOTDIR
perl -pibak -e 's/^export/#export/g' $ZDOTDIR/.zshrc
tee -a $ZDOTDIR/.zshrc <<-EOF
export TZ="Europe/Copenhagen"
export LANG="da_DK.UTF-8"
export LANGUAGE="en"
export LC_ALL="da_DK.UTF-8"
export VISUAL="code"
EOF

echo "source ${ZDOTDIR}/.zshenv" >| ~/.zshenv
zsh



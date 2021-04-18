#git@github.com:haskoe/install-scripts.git

SSH_HOSTS=()
PREFERRED_LOCALE_PREFIX=da_
PREFERRED_LOCALE=da_DK
GH_EMAIL=

sudo pacman -Syu
sudo pacman -Sy git

# git clone git@github.com:haskoe/install-scripts.git ~/arch-setup

tee -a ~/.bashrc <<-EOF
. ~/arch-setup/.bash/.bashrc
EOF
source ~/.bashrc

sudo pacman -Sy --needed python-pip zsh-completions openssh inetutils keychain bash-completion mc cdrkit qemu fd zola rg xorg-server xorg-apps lightdm-gtk-greeter i3-gaps firefox ranger i3status terminator base-devel

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

systemctl enable lightdm

cp /etc/X11/xinit/xinitrc ~/.xinitrc
perl -pibak -e 's/^exec /#exec/g' ~/.xinitrc
tee -a ~/.xinitrc <<-EOF
numlockx &
exec i3
EOF

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
code --install-extension eamodio.gitlens ritwickdey.LiveServer ckolkman.vscode-postgres geddski.macros ms-azuretools.vscode-docker ms-vscode-remote.vscode-remote-extensionpack ms-vscode-remote.remote-ssh ms-vscode-remote.remote-containers


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
ssh-keygen -f ~/.ssh/$ssh_fname
# todo: copy to all hosts in SSH_HOSTS
ssh-copy-id -i ~/.ssh/$ssh_fname.pub $USER@......


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
tee -a ~/mode-2560.sh <<-EOF
# 2560x1440 on older intel GPUs
xrandr --newmode "2560x1440_40.00"  201.00  2560 2720 2984 3408  1440 1443 1448 1476 +hsync +vsync
xrandr --addmode HDMI2 2560x1440_40.00
xrandr --output HDMI2 --mode 2560x1440_40.00 --verbose
EOF

# docker
sudo pacman -Sy --needed docker
sudo systemctl start docker
sudo systemctl enable docker
sudo usermod -a -G docker $USER
sudo su -l $USER
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
sudo pacman -Sy --needed virt-manager qemu vde2 ebtables dnsmasq bridge-utils openbsd-netcat libgovirt-0.3.8-1  dhclient-4.4.2-2  dmidecode-3.3-1  virt-viewer-9.0-1
# check
sudo modprobe kvm_intel
lsmod | grep kvm
#
usermod --append --groups libvirt `whoami`
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
curl --proto '=https' --tlsv1.2 -sSf https://sh.rustup.rs -o rust.sh
source ~/.cargo/env

# zsh/sheldon
yay sheldon-bin
yay nerd-fonts-complete
....
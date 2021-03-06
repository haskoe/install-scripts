# test docker
#docker run -it --rm archlinux bash -c "echo hello world"

# ssh
hostname=`hostname`
ssh_fname=id_$hostname
ssh-keygen -f ~/.ssh/${ssh_fname}
# todo: copy to all hosts in SSH_HOSTS
#ssh-copy-id -i ~/.ssh/${ssh_fname}.pub $USER@......

#eval `keychain --agents ssh --eval ~/.ssh/id_hg`
SSH_ID=id_hg
# abort if id_hg is not in keyring
# azure-repos
tee -a ~/.ssh/config <<-EOF

Host ssh.dev.azure.com
 IdentityFile ~/.ssh/${SSH_ID}
 IdentitiesOnly yes
EOF

chmod 600 ~/.ssh/${SSH_ID}
chmod 644 ~/.ssh/${SSH_ID}.pub
mkdir -p ~/dev/azure-repos/misc
cd ~/dev/azure-repos/misc
git clone git@ssh.dev.azure.com:v3/heas0404/MISC/MISC misc
bash ~/dev/azure-repos/misc/misc/git/status-all-branches.sh

# github repos
mkdir -p ~/dev/haskoe
cd ~/dev/haskoe
git clone https://github.com/haskoe/ecg_epilepsy.git

# icc profile
yay -S xiccd
sudo systemctl enable --now colord
xiccd &
colormgr get-devices
colormgr get-profiles 
colormgr device-add-profile "xrandr-BenQ GW2765-5AE00001019" icc-666e1c8857493a779957c377b08c44af
colormgr device-make-profile-default  "xrandr-BenQ GW2765-5AE00001019" icc-666e1c8857493a779957c377b08c44af
# reboot

# terminator theme
pip install requests
[[ ! -d $HOME/.config/terminator/plugins ]] && mkdir -p $HOME/.config/terminator/plugins
wget https://git.io/v5Zww -O $HOME"/.config/terminator/plugins/terminator-themes.py"

# brightness
# amdgpu
sudo usermod -aG video $USER
yay -R xorg-xbacklight 
yay -S acpilight
sudo tee -a /etc/udev/rules.d/90-backlight.rules <<-EOF
SUBSYSTEM=="backlight", ACTION=="add", \
  RUN+="/usr/bin/chgrp video /sys/class/backlight/amdgpu_bl0/brightness", \
  RUN+="/usr/bin/chmod g+w /sys/class/backlight/amdgpu_bl0/brightness"
EOF

# decrease by 30%
brightnessctl --min-val=2 -q set 30%-
# increase by 30%
brightnessctl -q set 30%+

# lightdm startup issue
sudo perl -pibak -e 's/.logind-check-graphical=.*$/logind-check-graphical=true/' /etc/lightdm/lightdm.conf

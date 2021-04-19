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

#!/bin/bash

apt -y install emacs-nox curl language-pack-da git build-essential ripgrep bash-completion ca-certificates gnupg lsb-release apt-transport-https ca-certificates software-properties-common tmux tmuxinator

locale-gen da_DK.UTF-8
locale-gen da_DK
update-locale 
update-locale LANG=da_DK.UTF-8

[[ -z "$NEW_HOSTNAME" ]] && echo "NEW_HOSTNAME must be set" && exit 1

# abort if USERNAME is not set
[[ -z "$USERNAME" ]] && echo "USERNAME must be set" && exit 1

# change hostname
hostnamectl set-hostname $NEW_HOSTNAME

# docker
curl -fsSL https://download.docker.com/linux/ubuntu/gpg | sudo gpg --dearmor -o /usr/share/keyrings/docker-archive-keyring.gpg
echo "deb [arch=$(dpkg --print-architecture) signed-by=/usr/share/keyrings/docker-archive-keyring.gpg] https://download.docker.com/linux/ubuntu $(lsb_release -cs) stable" | sudo tee /etc/apt/sources.list.d/docker.list > /dev/null
apt -y update
apt -y install docker-ce docker-ce-cli containerd.io
usermod -aG docker $USERNAME

# docker compose
DEST=/usr/local/bin/docker-compose
curl -L "https://github.com/docker/compose/releases/download/1.27.4/docker-compose-$(uname -s)-$(uname -m)" -o $DEST
chmod +x $DEST

# tailscale
curl -fsSL https://pkgs.tailscale.com/stable/ubuntu/focal.gpg | sudo apt-key add -
curl -fsSL https://pkgs.tailscale.com/stable/ubuntu/focal.list | sudo tee /etc/apt/sources.list.d/tailscale.list
apt-get -y update
apt-get -y install tailscale
tailscale up

# create user and add to group sudo
adduser $USERNAME
usermod -a -G sudo $USERNAME

# change password
passwd $USERNAME

# sudoers nopasswd
# %USERNAME% ALL=(ALL) NOPASSWD: ALL 

# passwordless login
#su $USERNAME
mkdir /home/${USERNAME}/.ssh
chmod 700 /home/${USERNAME}/.ssh 
chown heas /home/${USERNAME}/.ssh 

# on other host
ssh-copy-id -i ~/.ssh/id_<key name>.pub user@host

# login as new user
sudo usermod -aG docker $USER
docker run hello-world

# postgres
sudo sh -c 'echo "deb http://apt.postgresql.org/pub/repos/apt $(lsb_release -cs)-pgdg main" > /etc/apt/sources.list.d/pgdg.list'
wget --quiet -O - https://www.postgresql.org/media/keys/ACCC4CF8.asc | sudo apt-key add -
sudo apt -y update
sudo apt install -y postgresql-13 postgresql-client-13 libpq-dev postgresql-13-postgis-3 postgresql-13-postgis-3-scripts 
sudo apt install -y postgresql-server-dev-13 postgresql-plpython3-13 postgresql-13-pgtap
sudo systemctl enable postgresql
sudo systemctl start postgresql
sudo -u postgres psql -c "ALTER USER postgres with encrypted password '<....>'" postgres
sudo perl -pibak -e 's/local.*all.*postgres.*peer/local all postgres md5/' /etc/postgresql/13/main/pg_hba.conf
sudo perl -pibak -e 's/en_US/da_DK/g' /etc/postgresql/13/main/postgresql.conf 
sudo systemctl restart postgresql.service
sudo -u postgres createuser -s -d heas
createdb heas
psql
# tune parameters ....
#sudo emacs /etc/postgresql/12/main/postgresql.conf
#sudo systemctl restart postgresql.service

# passwordless ssh and block root
CONF_FILE=/etc/ssh/sshd_config.d/hardened.conf
echo PermitRootLogin no | sudo tee $CONF_FILE
echo PubkeyAuthentication yes | sudo tee -a $CONF_FILE
echo ChallengeResponseAuthentication no | sudo tee -a $CONF_FILE
sudo perl -pibak -e 's/^PermitRootLogin/#PermitRootLogin/g' /etc/ssh/sshd_config
sudo systemctl restart ssh

# node
curl -sL https://deb.nodesource.com/setup_16.x | sudo bash
sudo apt-get install -y nodejs


# firewall
sudo ufw default deny incoming
sudo ufw allow OpenSSH
sudo ufw allow 80
sudo ufw allow 443

# docker nginx auto ...
# registrerer nyt dyndns domæne som skal pege på host IP
docker run -d -p 80:80 -v /var/run/docker.sock:/tmp/docker.sock:ro jwilder/nginx-proxy
docker run --expose 80 -e VIRTUAL_HOST=dev.humanassist.dyndns.dk --name aspnetcore_sample --rm -it mcr.microsoft.com/dotnet/samples:aspnetapp

# Clone, build and run aspnetapp
mkdir -p ~/dev/ms
cd ~/dev/ms
git clone https://github.com/dotnet/dotnet-docker.git
cd dotnet-docker/samples/aspnetapp/
docker build -t aspnetapp .



# upload server
docker run --expose 80 -e VIRTUAL_HOST=upload.humanassist.dyndns.dk --name upload -v $HOME/tmp:/var/root mayth/simple-upload-server -token f9403fc5f537b4ab332d -port 80 /var/root



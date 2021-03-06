# change shell
sudo usermod --shell /bin/bash $USER

# KVM/QEMU install
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

# remote-ssh ssh config example
tee -a ~/.ssh/config <<-EOF

Host 10.0.0.3
 IdentityFile ~/.ssh/id_mx500
 HostName 10.0.0.3
 User $USER
EOF

# octave
sudo pacman -Sy --needed octave gcc-fortran
octave --eval "pkg install -forge control"
octave --eval "pkg install -forge signal"
# clone haskoe repo
# cd ....
# gco master
# octave patient_plot_full_csi.m

# dotnet
DI_DIR=~/proj
[[ ! -d $DI_DIR ]] && mkdir $DI_DIR
wget https://dot.net/v1/dotnet-install.sh
mv dotnet-install.sh $DI_DIR
~/proj/dotnet-install.sh --install-dir /usr/share/dotnet -channel Current -version latest

# jupyter simpy notebook
docker run -p 8888:8888 jupyter/scipy-notebook
# paste below in 
from sympy import *
x, y, z, t = symbols('x y z t')
integrate(exp(x)*sin(x) + exp(x)*cos(x), x)
eq = tan(sympy.log(x**2 + 1))
eq.diff(x)

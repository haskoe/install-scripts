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
# octave patient_plot_full_csi.m


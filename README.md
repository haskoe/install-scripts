# ARCH install scripts

My Arch Linux install scripts for quickly setting up a development environment with i3, ZSH, Visual Studio Code, Docker, KVM and postgres.

sheldon is used to configure ZSH.

Usage:
1. Clone repo and copy to USB
2. Install from Arch Linux ISO, I'm using archinstall that has been added in the april 2021 ISO (really good)
3. After reboot: Login, startx, mount USB, copy installer files to ~/whatever
4. Edit setup-vars.sh
5. Run arch-setup-1.sh, logout, select i3 and login
6. Run arch-setup-2.sh, logout and login

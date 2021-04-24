#!/bin/bash -e

INTF=$1
[[ -z $INTF ]] && echo "please specify network interface to configure" && exit 1

INTF_FNAME="/etc/systemd/network/10-${INTF}.network"
[[ -f $INTF_FNAME ]] && echo "network interface file already exists" && exit 1

ip addr | grep -q "[[:space:]]${INTF}\:"
RETVAL=$?
[[ $RETVAL -ne 0 ]] && echo "network interface not found" && exit 1

sudo tee -a $INTF_FNAME <<-EOF
[Match]
Name=$INTF

[Network]
DHCP=yes
EOF

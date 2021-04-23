#!/bin/bash -e

SCRIPTPATH="$( cd "$( dirname "${BASH_SOURCE[0]}" )" &> /dev/null && pwd )"

. $SCRIPTPATH/kdbx.sh $KP_VPN_PATH

if ! [[ -z $KP_PWD ]];
then
    xterm -T "a" -e "echo $KP_PWD | sudo openconnect -u $VPN_USER --passwd-on-stdin $VPN_HUB" &
fi

#!/bin/bash -x

[[ -z $KP_MASTER_PWD ]] && echo missing master pwd && (exit 1) && true

SCRIPTPATH="$( cd "$( dirname "${BASH_SOURCE[0]}" )" &> /dev/null && pwd )"

RD_ARGS=`python3 $SCRIPTPATH/kp1.py $KP_DB $KP_MASTER_PWD askingculture/hetzner-jumphost`
xfreerdp  /kbd:0x00000406 /f $RD_ARGS


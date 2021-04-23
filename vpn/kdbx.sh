#!/bin/bash -e

SCRIPTPATH="$( cd "$( dirname "${BASH_SOURCE[0]}" )" &> /dev/null && pwd )"

KP_PWD=""
[[ ! -f $KP_DB ]] && echo "keepass database missing" && return 1


KP_PWD=`python3 $SCRIPTPATH/kp.py $KP_DB $KP_MASTER_PWD $1`

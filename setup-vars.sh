#!/bin/bash

SSH_HOSTS=()
PREFERRED_LOCALE= #en_DK.UTF-8
PREFERRED_KEYMAP= #dk
GH_USER= #haskoe
GH_EMAIL= #henrik@haskoe.dk

SCRIPTPATH="$( cd "$( dirname "${BASH_SOURCE[0]}" )" &> /dev/null && pwd )"
[[ ! -f $SCRIPTPATH/setup-vars.sh ]] && echo "SCRIPTPATH not set correctly" && exit 1

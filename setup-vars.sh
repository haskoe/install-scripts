#!/bin/bash

SSH_HOSTS=()
PREFERRED_LOCALE= #en_DK.UTF-8
PREFERRED_KEYMAP= #dk
GH_USER= #haskoe
GH_EMAIL= #henrik@haskoe.dk

SCRIPTPATH="$( cd -- "$(dirname "$0")" >/dev/null 2>&1 ; pwd -P )"

[[ ! -f $SCRIPTPATH/setup-vars.sh ]] && echo "SCRIPTPATH not set correctly" && exit 1

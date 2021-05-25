#!/bin/bash

SCRIPTPATH="$( cd "$( dirname "${BASH_SOURCE[0]}" )" &> /dev/null && pwd )"

I3_CONFIG_PATH=~/.config/i3
I3_CONFIG=${I3_CONFIG_PATH}/config

[[ ! -f $I3_CONFIG ]] && echo "missing i3 config dir and file" && exit 1

cp $SCRIPTPATH/i3/i3-config $I3_CONFIG
cp $SCRIPTPATH/i3/i3-lock.sh $I3_CONFIG_PATH
#cp $SCRIPTPATH/i3/i3-xautolock.sh $I3_CONFIG_PATH

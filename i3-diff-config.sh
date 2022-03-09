#!/bin/bash

SCRIPTPATH="$( cd "$( dirname "${BASH_SOURCE[0]}" )" &> /dev/null && pwd )"

[[ ! -f $SCRIPTPATH/i3/i3-config ]] && echo "SCRIPTPATH not set correctly" && exit 1

I3_CONFIG_PATH=~/.config/i3
I3_CONFIG=${I3_CONFIG_PATH}/config

[[ ! -f $I3_CONFIG ]] && echo "missing i3 config dir and file" && exit 1

diff $SCRIPTPATH/i3/i3-config $I3_CONFIG
diff  $SCRIPTPATH/i3/i3-lock.sh $I3_CONFIG_PATH


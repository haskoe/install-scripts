#!/bin/bash -x

userid=$1
pc=$2
SCRIPTPATH="$( cd "$( dirname "${BASH_SOURCE[0]}" )" &> /dev/null && pwd )"

. $SCRIPTPATH/kdbx.sh regionh/$userid

[[ -z $KP_PWD ]] && echo "no entry for $userid in pwd database"

xfreerdp /kbd:0x00000406 /v:$pc /u:regionh\\${userid} /p:$KP_PWD /f

#!/bin/bash -x

domain=$1
userid=$2
pc=$3
SCRIPTPATH="$( cd "$( dirname "${BASH_SOURCE[0]}" )" &> /dev/null && pwd )"

. $SCRIPTPATH/kdbx.sh ${domain}/${userid}

[[ -z $KP_PWD ]] && echo "no entry for $userid in pwd database"

xfreerdp  /kbd:0x00000406 /v:$pc /u:${domain}\\${userid} /p:$KP_PWD /f

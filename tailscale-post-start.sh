#!/bin/bash -e
#
# update tailscale ip in syncthing hosts file
HOSTS_DIR=/home/heas/sync/heas-private
HOSTS_FILE=${HOSTS_DIR}/hosts
TEMP_FILE=${HOSTS_DIR}/hosts.stripped
TS_HOSTNAME=t`hostname`

# remove current tailscale IP
[[ ! -f $HOSTS_FILE ]] && touch $HOSTS_FILE
grep -v "^${TS_HOSTNAME}" $HOSTS_FILE >$TEMP_FILE
ENTRY="$TS_HOSTNAME `ip addr show tailscale0 | grep "inet\b" | awk '{print $2}' | cut -d/ -f1`"
tee -a $TEMP_FILE <<-EOF
$ENTRY
EOF

cp $TEMP_FILE $HOSTS_FILE
rm $TEMP_FILE

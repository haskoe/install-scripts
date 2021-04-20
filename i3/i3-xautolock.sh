#!/bin/sh -e

#pkill xautolock
# todo fix
xautolock -time 5 -locker '~/dev/azure-repos/linux/ubuntu/i3-lock.sh' &

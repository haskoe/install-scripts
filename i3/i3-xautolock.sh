#!/bin/sh -e

#pkill xautolock
xautolock -time 5 -locker '~/dev/azure-repos/linux/ubuntu/i3-lock.sh' &

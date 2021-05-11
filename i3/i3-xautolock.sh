#!/bin/sh -e

#pkill xautolock
xautolock -time 1 -locker '~/.config/i3/i3-lock.sh' &

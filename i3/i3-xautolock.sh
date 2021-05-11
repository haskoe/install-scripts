#!/bin/sh -e
#
echo i3-xautolock >>~/.config/i3/log
pkill xautolock
echo i3-xautolock >>~/.config/i3/log
xautolock -time 1 -locker '~/.config/i3/i3-lock.sh' &

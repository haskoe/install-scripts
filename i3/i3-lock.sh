#!/bin/sh -e

echo "Time: $(date -Iseconds)" >>~/.config/i3/log

# Take a screenshot
scrot /tmp/screen_locked.png

# Pixellate it 10x
mogrify -scale 10% -scale 1000% /tmp/screen_locked.png

# Lock screen displaying this image.
i3lock -i /tmp/screen_locked.png

# Turn the screen off after a delay.
sleep 300; pgrep i3lock && xset dpms force off

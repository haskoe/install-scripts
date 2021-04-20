# mode 2560 on older intel CPUs
HDMI=`xrandr | grep ^HDMI | sed -e 's/\s.*$//'`
[[ "${HDMI}" = "" ]] && echo "No HDMI" && exit 1

# two HDMI from xrandr .....
HDMI=HDMI2
SCRPT=~/mode-2560.sh
tee -a $SCRPT <<-EOF
# 2560x1440 on older intel GPUs
xrandr --newmode "2560x1440_40.00"  201.00  2560 2720 2984 3408  1440 1443 1448 1476 +hsync +vsync
xrandr --addmode $HDMI 2560x1440_40.00
xrandr --output $HDMI --mode 2560x1440_40.00 --verbose
EOF

echo "$SCRPT created"
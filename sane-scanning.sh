sudo pacman -S --needed sane-airscan sane-frontends

# identify scanner
scanimage -L

SCAN_DEVICE="airscan:w0:CANON INC. TS7400 series"

# scan from bed
scanimage --format=png --output-file test.png --progress --device "${SCAN_DEVICE}"

# scan from sheet feeder
scanimage --format=png --progress --device "${SCAN_DEVICE}" --source ADF --batch

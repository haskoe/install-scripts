#!/bin/bash -e
#
# i3-wrapper associates strings supplied by i3-input with programs.
#
# It is inspired by the configurable keyboard shortcuts for bookmarks
# in web browsers such as Firefox and Google Chrome.
#
# Type a string and the corresponding program is brought into focus;
# if it is already running focus moves to its associate workspace,
# otherwise it is launched in the current workspace.
#
# Take care to match long strings before short strings beginning with
# identical substrings.
#
# The easiest match to use is 'instance' because the instance sring
# usually matches the program name. Using 'title' is complicated by
# the need to enclose the matched substring in quotes but it can be a
# good choice in some cases.
#
# Contains at least one bashism: (link deleted so I can post)
#

focus ()
{
    case $1 in
        (instance)
        W=$(xdotool search --classname "$2" | head -1)
        ;;
        (class)
        W=$(xdotool search --class "$2" | head -1)
        ;;
        (title)
        W=$(xdotool search --name "$2" | head -1)
        ;;
        (*)
        W=''
        ;;
    esac
    if [ -z "$W" ]; then
        # Launch the program. 
        eval ${@:3} &  # bash
    else
        # Focus the window.
        # Used 'sed' to escape any whitepace to suit 'i3-msg'.
        i3-msg "[$1=$(echo $2 | sed s/\\x20/\\\\x20/g)] focus"
    fi
}

start ()
{
    case "$@" in
        (apsacc)
        focus instance google-chrome google-chrome
        ;;
        (sy)
        focus instance synaptic gksu synaptic
        ;;
        (th)
        focus instance thunar thunar
        ;;
        (dc|py)
        focus title Calculator python-interactive 
        ;;
        (wr)
        focus title 'LibreOffice Writer' libreoffice --writer
        ;;
        (pp)
        focus title 'LibreOffice Impress' libreoffice --impress
        ;;
        (ss)
        focus title 'LibreOffice Calc' libreoffice --calc
        ;;
        (pdf)
        focus instance okular okular
        ;;
        (am)
        focus instance clementine clementine
        ;;
        (vc|vlc)
        focus instance vlc vlc
        ;;
        (*)
        ;;
    esac
}

start "$@"

#
# Done.
#
import os
import sys
from os import path
from os.path import expanduser
from pykeepass import PyKeePass

kdbx_file=sys.argv[1]
pwd=sys.argv[2]
pth=sys.argv[3]
kp = PyKeePass( kdbx_file, password=pwd)

splt=pth.split('/')
group = kp.find_groups(name=splt[0], first=True)
entries=[e for e in group.entries if e.title==splt[1]]
if not entries:
    entries=[e for e in group.entries if e.title.endswith(splt[1])]
print(entries and entries[0].password or '')

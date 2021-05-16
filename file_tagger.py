from os import path
import sys
from pathlib import Path
import configparser

if len(sys.argv)<2:
    fn = '/home/has/dev/haskoe/install-scripts/file_tagger.py'
else:
    fn = sys.argv[1]

if not path.exists(fn):
    exit(1)

full_file_name = path.abspath(fn)
tag_file = 'tagged_files.ini'
pth = Path(full_file_name)
while True:
    if pth == pth.parent.absolute():
        exit(1)

    pth = pth.parent.absolute()
    tag_file_full = path.join(pth,tag_file)
    if path.exists(path.join(pth,tag_file_full)):
        break

config = configparser.ConfigParser()
config.read(tag_file_full)

rel_dir = path.dirname(full_file_name)[len(path.dirname(tag_file_full))+1:]
file_name = path.basename(full_file_name)
if not config.has_section(rel_dir):
    config.add_section(rel_dir)
if not config.has_option(rel_dir,file_name):
    config[rel_dir][file_name] = ''

with open(tag_file_full, 'w') as configfile:
    config.write(configfile)

with open(tag_file_full, 'r') as f:
    content = f.readlines()
rel_dir = '[%s]' % (rel_dir,)
idx = next((l for l in range(len(content)) if content[l].startswith(rel_dir)))
idx = next((l for l in range(idx+1,len(content)) if content[l].startswith(file_name)))

print(tag_file_full)
exit(idx)

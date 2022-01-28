import requests as req
import xml.etree.ElementTree as ET
from os import path
import os
import re

script_path = path.dirname(path.abspath(__file__))
root_dir = path.join(script_path, 'dr-radio')

prog_names = ('hjernekassen-pa-p1', )
prog_names = ('min-yndlingsmusik', )

wget_pattern = 'wget -c --no-check-certificate -O %s "%s"'
for prog_name in prog_names:
    overview_url = f'https://www.dr.dk/mu/feed/{prog_name}.xml?format=podcast'
    r = req.get(overview_url)
    root = ET.fromstring(r.text)

    tgt_dir = path.join(root_dir, prog_name)
    if not path.exists(tgt_dir):
        os.makedirs(tgt_dir)

    dwn = []
    for item in root.findall('channel/item'):
        title = item.find('title').text
        description = item.find('description').text
        url = item.find('enclosure').attrib['url']

        title_stripped = re.sub('\W+', '-', title)
        dwn.append(wget_pattern % (title_stripped + '.mp3', url))

        desc_file = path.join(tgt_dir, title_stripped + '.txt')
        if path.exists(desc_file):
            continue

        with open(desc_file, 'w') as f:
            f.write(description)

    with open(path.join(tgt_dir, 'download.sh'), 'w') as f:
        f.write('\n'.join(dwn))

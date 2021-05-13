import os
from os import path
import magic
import collections
import more_itertools as it

THREAD_NUM = 4


def walk_error(x):
    print('walk_error')


def walk(dir, filter_func=None):
    filter_func = filter_func and not isinstance(
        filter_func, collections.Sequence) and (filter_func, ) or filter_func
    for root, dirs, files in os.walk(dir,
                                     followlinks=False,
                                     onerror=walk_error):
        for name in files:
            ext = path.splitext(name)[-1][1:]
            full_name = path.join(root, name)
            do_yield = not filter_func
            if not do_yield:
                do_yield = all(
                    (f(root, name, ext, full_name) for f in filter_func))
            if do_yield:
                yield root, name, ext, full_name


def is_mime_type(mime_type):
    def f(root, name, ext, full_name):
        fm = magic.detect_from_filename(full_name)
        ok = fm and fm[0].split('/')[0] == mime_type or False
        return ok

    return f


check_for = 'video'
pth = '/run/media/heas/Seagate/recover'
pth = '/home/heas/recover'
#pth = '/home/heas/.cache/thunderbird/wxfo54zh.default-release/cache2/entries/'
for fname in walk(pth, is_mime_type(check_for)):
    print('sh ~/check_%s.sh %s' % (check_for,fname[-1]))

# exts = {}
# for root, name, ext, full_name in it.take(10000000, walk(pth)):  # walk(pth):
#     exts[ext] = exts.get(ext, 0) + 1
#     #print('sh ~/check_%s.sh %s' % (check_for,fname[-1]))

# counts = reversed(sorted(set(exts.values())))
# print('\n'.join([
#     '%d: %s' % (c, ','.join([k for k in exts if exts[k] == c]))
#     for c in counts
# ]))

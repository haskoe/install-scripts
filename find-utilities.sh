# delete by extension and file size less than ...
  /run/media/heas/Seagate/samsungt5/sync/photos/ -name "*.png" -type f -size -100k -print # -delete

  # delete empty direct. 
  find . -type d -empty -print #-delete

# remove duplicates from >1 targets with no prompt
fdupes -dNr /run/media/heas/Seagate/ /run/media/heas/Expansion\ Drive/


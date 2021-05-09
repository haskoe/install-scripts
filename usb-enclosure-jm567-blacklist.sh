# USB enclosures based on JMicron JM567 is currently causing trouble in UAS mode
# Solution: revert to usb_stotage mode

# 1: plugin enclosure and obtain vendorid and productid  from dmesg
sudo dmesg

# 2: create a new file in /etc/modprobe.d/ with content
# options usb-storage quirks=<vendor id>:<product id>:u
echo options usb-storage quirks=152d:0562:u | sudo tee /etc/modprobe.d/jm567.conf

# 3: add /etc/modprobe.d/jm567.conf to FILES in /etc/mkinitcpio.conf 
# FILE(.... /etc/modprobe.d/jm567.conf)

# rebuilt and reboot
sudo mkinitcpio -p linux
sudo reboot
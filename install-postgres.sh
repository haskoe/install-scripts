# postgresql
# remote
# sudo rm -rf /var/lib/postgres/data
# sudo pacman -R postgresql postgis
sudo pacman -Sy --needed postgresql postgis
yay -Sy postgrest
sudo -u postgres bash -c "initdb --locale $LANG -E UTF8 -D '/var/lib/postgres/data'"
sudo perl -pibak -e 's/ident/trust/' /var/lib/postgres/data/pg_hba.conf

sudo tee -a /var/lib/postgres/data/postgresql.conf <<-EOF
max_connections = 32
shared_buffers = 2GB
effective_cache_size = 6GB
work_mem = 128MB
maintenance_work_mem = 512MB
checkpoint_completion_target = 0.7
wal_buffers = 16MB
default_statistics_target = 100
EOF

sudo systemctl start postgresql
systemctl enable postgresql
# giver fejl, men bruger oprettes
sudo -u postgres createuser -s -d $USER

# stop and disable
sudo systemctl stop postgresql
systemctl disable postgresql

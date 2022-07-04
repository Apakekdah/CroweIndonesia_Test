# CroweIndonesia_Test
 Crowe Indonesia Test Skill

# Initial data
User : Atlas, Rudi
Password : secret

Becareful when you want to trace it

# Docker Builder
sudo docker build -t croweindo/api . -f CI.API/Dockerfile --compress --no-cache

# Docker Dump
sudo docker save croweindo/api -o ./ci_api.tar

# Docker Import/Load
sudo docker load < ci_api.tar
FROM nginx:1.19.2
WORKDIR /usr/share/nginx/html
COPY . updates
RUN rm ./updates/Dockerfile && echo "OK|$(md5sum ./updates/launcher/SkyMPLauncher.exe | awk '{ print toupper($1) }')" > ./updates/launcher/index.html
RUN cat ./updates/launcher/index.html
RUN ls -R

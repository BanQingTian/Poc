FROM harbor.nreal.work:5000/tools/nodejs:v12.16.1
# docker pull harbor.nreal.work:5000/tools/nodejs:v12.16.1
RUN mkdir -p /home/towergame
ADD netframeworkserver /home/towergame/netframeworkserver
WORKDIR /home/towergame/netframeworkserver

EXPOSE 2567 2568
ENV NODE_ENV test
ENTRYPOINT ["npm", "start"]

FROM node:lts
WORKDIR /app/project
COPY client/package*.json ./
RUN npm ci
COPY client .
EXPOSE 3939
CMD [ "node", "server.js"]
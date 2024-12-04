FROM node:lts-alpine
WORKDIR /usr/src/app
COPY ["package.json", "package-lock.json*", "npm-shrinkwrap.json*", "./"]
RUN npm install --silent && mv node_modules ../
RUN npm install -g @angular/cli
COPY . .
EXPOSE 5004
RUN chown -R node /usr/src/app
USER node
CMD ["ng", "serve", "--host", "0.0.0.0", "--port", "5004"]
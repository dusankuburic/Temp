#angualr setup
FROM node as build
WORKDIR /app
COPY package*.json .
RUN npm install
COPY . .

RUN npm run build

#nginx setup
FROM nginx:alpine

RUN rm /etc/nginx/conf.d/default.conf
COPY nginx.conf /etc/nginx/conf.d
COPY --from=build app/dist/Temp-SPA /usr/share/nginx/html

EXPOSE 80
EXPOSE 433
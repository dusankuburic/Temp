#angualr setup
FROM node as build
WORKDIR /app
COPY package*.json .
RUN npm install
COPY . .

RUN npm run build

#nginx setup
FROM nginx:alpine

# Install OpenSSL for generating self-signed certificates
RUN apk add --no-cache openssl

# Generate self-signed SSL certificate for local development
RUN mkdir -p /etc/nginx/ssl && \
    openssl req -x509 -nodes -days 365 -newkey rsa:2048 \
    -keyout /etc/nginx/ssl/nginx.key \
    -out /etc/nginx/ssl/nginx.crt \
    -subj "/C=US/ST=State/L=City/O=Organization/CN=localhost"

RUN rm /etc/nginx/conf.d/default.conf
COPY nginx.conf /etc/nginx/conf.d
COPY --from=build app/dist/Temp-SPA/browser /usr/share/nginx/html

EXPOSE 80
EXPOSE 443
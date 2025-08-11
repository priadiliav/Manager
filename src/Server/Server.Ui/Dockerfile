FROM node:22-alpine AS build
WORKDIR /app

COPY ["src/Server/Server.Ui/package.json", "src/Server/Server.Ui/package-lock.json", "./"]
RUN npm ci

COPY src/Server/Server.Ui/. .
RUN npm run build

FROM nginx:alpine
COPY --from=build /app/build /usr/share/nginx/html

COPY ["src/Server/Server.Ui/docker-entrypoint.sh", "/docker-entrypoint.sh"]

RUN chmod +x /docker-entrypoint.sh

EXPOSE 80
ENTRYPOINT ["/docker-entrypoint.sh"]
CMD ["nginx", "-g", "daemon off;"]

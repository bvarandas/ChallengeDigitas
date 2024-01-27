# stage 1
FROM node:20.11.0 as build 
WORKDIR /usr/local/app
COPY ./ /usr/local/app/
RUN npm install
RUN npm run build
#EXPOSE 4200
CMD ["npm", "start"]

FROM nginx:latest
VOLUME /var/cache/nginx
COPY --from=build /usr/local/app/dist/order-book.front /usr/share/nginx/html
COPY ./config/nginx.conf /etc/nginx/conf.d/default.conf
EXPOSE 80
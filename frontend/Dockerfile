# Frontend Dockerfile for Transaction Explorer
# Multi-stage build for development and production

# Base stage with Node.js
FROM node:20-alpine AS base
WORKDIR /app
# Install dependencies needed for native modules
RUN apk add --no-cache libc6-compat

# Dependencies stage
FROM base AS deps
COPY package.json package-lock.json ./
RUN npm ci

# Build stage
FROM base AS build
COPY --from=deps /app/node_modules ./node_modules
COPY . .
RUN npm run build

# Development stage with hot reload
FROM base AS dev
COPY --from=deps /app/node_modules ./node_modules
COPY . .
EXPOSE 3000
CMD ["npm", "run", "dev"]

# Production stage with static file serving
FROM nginx:alpine AS production
# Copy built files to nginx
COPY --from=build /app/build /usr/share/nginx/html
# Copy nginx configuration
COPY nginx.conf /etc/nginx/nginx.conf
EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]

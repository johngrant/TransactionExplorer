#!/bin/bash

# Navigate to the frontend directory
cd $(dirname "$0")

# Install dependencies
if [ ! -d "node_modules" ]; then
  echo "Installing dependencies..."
  npm install
fi

# Start the development server
echo "Starting the development server..."
npm run dev

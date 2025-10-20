#!/bin/bash

# SimplyTrack API Deployment Script
# Run this on your Ubuntu server

set -e  # Exit on any error

echo "🚀 Deploying SimplyTrack API..."

# Configuration
REPO_URL="https://github.com/alvaro242/SimplyTrack-API.git"
APP_DIR="/opt/simplytrack"
SERVICE_NAME="simplytrack-api"

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

print_status() {
    echo -e "${GREEN}✅ $1${NC}"
}

print_warning() {
    echo -e "${YELLOW}⚠️  $1${NC}"
}

print_error() {
    echo -e "${RED}❌ $1${NC}"
}

# Check if running as root or with sudo
if [ "$EUID" -ne 0 ]; then 
    print_error "Please run with sudo"
    exit 1
fi

# Update system packages
print_status "Updating system packages..."
apt update && apt upgrade -y

# Install Docker if not present
if ! command -v docker &> /dev/null; then
    print_status "Installing Docker..."
    curl -fsSL https://get.docker.com -o get-docker.sh
    sh get-docker.sh
    systemctl enable docker
    systemctl start docker
    rm get-docker.sh
else
    print_status "Docker already installed"
fi

# Install Docker Compose if not present
if ! command -v docker-compose &> /dev/null; then
    print_status "Installing Docker Compose..."
    curl -L "https://github.com/docker/compose/releases/latest/download/docker-compose-$(uname -s)-$(uname -m)" -o /usr/local/bin/docker-compose
    chmod +x /usr/local/bin/docker-compose
else
    print_status "Docker Compose already installed"
fi

# Create application directory
print_status "Setting up application directory..."
mkdir -p $APP_DIR
cd $APP_DIR

# Clone or update repository
if [ -d ".git" ]; then
    print_status "Updating existing repository..."
    git pull origin main
else
    print_status "Cloning repository..."
    git clone $REPO_URL .
fi

# Check for environment file
if [ ! -f ".env" ]; then
    print_warning "Creating .env file from template..."
    cp .env.docker .env
    print_warning "⚠️  IMPORTANT: Edit .env file with your production secrets!"
    print_warning "nano .env"
    exit 1
fi

# Build and start services
print_status "Building and starting services..."
docker-compose down --remove-orphans
docker-compose build --no-cache
docker-compose up -d

# Wait for services to be ready
print_status "Waiting for services to start..."
sleep 30

# Check service health
if docker-compose ps | grep -q "Up"; then
    print_status "✅ SimplyTrack API deployed successfully!"
    print_status "📱 API URL: http://$(hostname -I | awk '{print $1}'):8080"
    print_status "🗃️  Database Admin: http://$(hostname -I | awk '{print $1}'):8081"
    print_status "📊 Check logs: docker-compose logs -f"
else
    print_error "❌ Deployment failed. Check logs: docker-compose logs"
    exit 1
fi

# Optional: Set up reverse proxy reminder
print_warning "💡 Next steps:"
echo "1. Configure firewall: ufw allow 8080"
echo "2. Set up reverse proxy (nginx/caddy) for production"
echo "3. Configure SSL certificates"
echo "4. Set up monitoring and backups"

print_status "🎉 Deployment complete!"
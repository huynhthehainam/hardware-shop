#!/bin/bash
set -e

echo "======================================"
echo " STEP 1: Dropping database"
echo "======================================"
dotnet ef database drop --force \
  --project HardwareShop.Infrastructure \
  --startup-project HardwareShop.WebApi

echo ""
echo "======================================"
echo " STEP 2: Removing existing migrations"
echo "======================================"
rm -rf "./HardwareShop.Infrastructure/Data/Migrations"

echo ""
echo "======================================"
echo " STEP 3: Adding initial migration"
echo "======================================"
dotnet ef migrations add Initial \
  --project HardwareShop.Infrastructure \
  --startup-project HardwareShop.WebApi \
  --output-dir Data/Migrations

echo ""
echo "======================================"
echo " STEP 4: Cleaning up build artifacts"
echo "======================================"
rm -rf HardwareShop.Infrastructure/bin\\Debug
rm -rf HardwareShop.WebApi/bin\\Debug

echo ""
echo "======================================"
echo " ✅ All steps completed successfully"
echo "======================================"

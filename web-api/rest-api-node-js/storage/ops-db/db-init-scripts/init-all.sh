#!/bin/bash
set -e

echo "Starting database initialization..."

# Create demo database
echo "Creating demo database..."
psql -v ON_ERROR_STOP=1 --username "$POSTGRES_USER" --dbname "$POSTGRES_DB" <<-EOSQL
    CREATE DATABASE demo;
EOSQL

# System DB Objects (sorted by filename) - run on demo database
echo "Executing System DB Objects..."
for file in $(find /scripts/schema-scripts/000-sys-db-objects/ -name "*.sql" | sort); do
    echo "Running: $file"
    psql -v ON_ERROR_STOP=1 --username "$POSTGRES_USER" --dbname "demo" -f "$file"
done

# Schema (sorted by filename) - run on demo database
echo "Executing Schema Scripts..."
for file in $(find /scripts/schema-scripts/001-demo_ops/ -name "*.sql" | sort); do
    echo "Running: $file"
    psql -v ON_ERROR_STOP=1 --username "$POSTGRES_USER" --dbname "demo" -f "$file"
done

# Data (sorted by filename) - run on demo database
echo "Executing Data Scripts..."
for file in $(find /scripts/data-scripts/ -name "*.sql" | sort); do
    echo "Running: $file"
    psql -v ON_ERROR_STOP=1 --username "$POSTGRES_USER" --dbname "demo" -f "$file"
done

echo "Database initialization completed successfully!"

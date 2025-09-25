DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1
        FROM pg_catalog.pg_namespace
        WHERE nspname = 'demo_ops'
    ) THEN
        -- Create the schema if it does not exist
        EXECUTE 'CREATE SCHEMA demo_ops';
        RAISE NOTICE 'Schema "demo_ops" created.';
    ELSE
        RAISE NOTICE 'Schema "demo_ops" already exists.';
    END IF;
END
$$;
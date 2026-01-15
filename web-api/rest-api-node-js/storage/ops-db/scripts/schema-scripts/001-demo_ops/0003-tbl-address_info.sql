DO $$
BEGIN
    IF EXISTS (SELECT 1 
                FROM information_schema.tables 
                WHERE table_name = 'address_info'
                AND table_schema = 'demo_ops') THEN
		RAISE NOTICE 'Table "demo_ops.address_info" already exists.';
	ELSE
        RAISE NOTICE 'Table "demo_ops.address_info": Creating..';
        CREATE TABLE demo_ops.address_info (
            uid                 uuid            DEFAULT gen_random_uuid(),
            address_id          varchar(50)     PRIMARY KEY,
            emp_id              varchar(50)     NOT NULL, --ref: TODO: FK emp_info.emp_id, TODO: index
            address_line1       varchar(500)    NOT NULL,
            address_line2       varchar(500),
            city                varchar(100)    NOT NULL,
            state               varchar(100)    NOT NULL,
            zip_code            varchar(20)     NOT NULL,
            country             varchar(100)    NOT NULL,
            created_by          varchar(100)    NOT NULL,
            updated_by          varchar(100)    NOT NULL,
            create_date         timestamptz     DEFAULT (timezone('utc', now())),
            update_date         timestamptz
        );


        RAISE NOTICE 'Table "demo_ops.address_info": Completed';
    END IF;
END $$;

DO $$
BEGIN
    IF EXISTS (SELECT 1 
                FROM pg_indexes 
                WHERE indexname = 'idx_demo_ops_address_info' 
                AND tablename = 'address_info'
                AND schemaname = 'demo_ops') THEN
		RAISE NOTICE 'Index "idx_demo_ops_address_info" already exists.';
	ELSE
        RAISE NOTICE 'Index "idx_demo_ops_address_info": Creating..';
        CREATE UNIQUE INDEX idx_demo_ops_address_info 
        ON demo_ops.address_info(uid);
        RAISE NOTICE 'Index "idx_demo_ops_address_info": Completed';
    END IF;
END $$;

DO $$
BEGIN
    IF EXISTS (SELECT 1
                FROM pg_trigger
                JOIN pg_class 
                    ON pg_trigger.tgrelid = pg_class.oid
                WHERE
                    pg_class.relname = 'address_info'
                    AND pg_class.relnamespace = (SELECT oid FROM pg_namespace WHERE nspname = 'demo_ops')
                    AND pg_trigger.tgname = 'address_info_sync_lastmod') THEN
		RAISE NOTICE 'Trigger "address_info_sync_lastmod" already exists.';
	ELSE
        RAISE NOTICE 'Trigger "address_info_sync_lastmod": Creating..';
        CREATE TRIGGER address_info_sync_lastmod
        BEFORE INSERT OR UPDATE ON
            demo_ops.address_info
        FOR EACH ROW EXECUTE PROCEDURE
            sync_lastmod();
        RAISE NOTICE 'Trigger "address_info_sync_lastmod": Completed';
    END IF;
END $$;


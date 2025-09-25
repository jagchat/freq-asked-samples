DO $$
BEGIN
    IF EXISTS (SELECT 1 
                FROM information_schema.tables 
                WHERE table_name = 'dept_info'
                AND table_schema = 'demo_ops') THEN
		RAISE NOTICE 'Table "demo_ops.dept_info" already exists.';
	ELSE
        RAISE NOTICE 'Table "demo_ops.dept_info": Creating..';
        CREATE TABLE demo_ops.dept_info (
            uid               uuid          DEFAULT gen_random_uuid(),
            dept_id           varchar(50)   PRIMARY KEY,
            dept_name         varchar(500)  NOT NULL,
            created_by        varchar(100)  NOT NULL,
            updated_by        varchar(100)  NOT NULL,    
            create_date       timestamptz   DEFAULT (timezone('utc', now())),
            update_date       timestamptz
        );
        RAISE NOTICE 'Table "demo_ops.dept_info": Completed';
    END IF;
END $$;

DO $$
BEGIN
    IF EXISTS (SELECT 1 
                FROM pg_indexes 
                WHERE indexname = 'idx_demo_ops_dept_info' 
                AND tablename = 'dept_info'
                AND schemaname = 'demo_ops') THEN
		RAISE NOTICE 'Index "idx_demo_ops_dept_info" already exists.';
	ELSE
        RAISE NOTICE 'Index "idx_demo_ops_dept_info": Creating..';
        CREATE UNIQUE INDEX idx_demo_ops_dept_info 
        ON demo_ops.dept_info(uid);
        RAISE NOTICE 'Index "idx_demo_ops_dept_info": Completed';
    END IF;
END $$;

DO $$
BEGIN
    IF EXISTS (SELECT 1
                FROM pg_trigger
                JOIN pg_class 
                    ON pg_trigger.tgrelid = pg_class.oid
                WHERE
                    pg_class.relname = 'dept_info'
                    AND pg_class.relnamespace = (SELECT oid FROM pg_namespace WHERE nspname = 'demo_ops')
                    AND pg_trigger.tgname = 'dept_info_sync_lastmod') THEN
		RAISE NOTICE 'Trigger "dept_info_sync_lastmod" already exists.';
	ELSE
        RAISE NOTICE 'Trigger "dept_info_sync_lastmod": Creating..';
        CREATE TRIGGER dept_info_sync_lastmod
        BEFORE INSERT OR UPDATE ON
            demo_ops.dept_info
        FOR EACH ROW EXECUTE PROCEDURE
            sync_lastmod();
        RAISE NOTICE 'Trigger "dept_info_sync_lastmod": Completed';
    END IF;
END $$;


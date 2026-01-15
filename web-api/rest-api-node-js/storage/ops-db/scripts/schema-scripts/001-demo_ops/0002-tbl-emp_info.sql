DO $$
BEGIN
    IF EXISTS (SELECT 1 
                FROM information_schema.tables 
                WHERE table_name = 'emp_info'
                AND table_schema = 'demo_ops') THEN
		RAISE NOTICE 'Table "demo_ops.emp_info" already exists.';
	ELSE
        RAISE NOTICE 'Table "demo_ops.emp_info": Creating..';
        CREATE TABLE demo_ops.emp_info (
            uid                   uuid          DEFAULT gen_random_uuid(),
            emp_id                varchar(50)   PRIMARY KEY,
            first_name            varchar(50)   NOT NULL,
            last_name             varchar(100)  NOT NULL,
            dept_id               varchar(50)   NOT NULL, --ref: TODO: FK dept_info.dept_id, TODO: index
            created_by            varchar(100)  NOT NULL,
            updated_by            varchar(100)  NOT NULL,
            create_date           timestamptz   DEFAULT (timezone('utc', now())),
            update_date           timestamptz
        );

        RAISE NOTICE 'Table "demo_ops.emp_info": Completed';
    END IF;
END $$;

DO $$
BEGIN
    IF EXISTS (SELECT 1 
                FROM pg_indexes 
                WHERE indexname = 'idx_demo_ops_emp_info' 
                AND tablename = 'emp_info'
                AND schemaname = 'demo_ops') THEN
		RAISE NOTICE 'Index "idx_demo_ops_emp_info" already exists.';
	ELSE
        RAISE NOTICE 'Index "idx_demo_ops_emp_info": Creating..';
        CREATE UNIQUE INDEX idx_demo_ops_emp_info 
        ON demo_ops.emp_info(uid);
        RAISE NOTICE 'Index "idx_demo_ops_emp_info": Completed';
    END IF;
END $$;

DO $$
BEGIN
    IF EXISTS (SELECT 1
                FROM pg_trigger
                JOIN pg_class 
                    ON pg_trigger.tgrelid = pg_class.oid
                WHERE
                    pg_class.relname = 'emp_info'
                    AND pg_class.relnamespace = (SELECT oid FROM pg_namespace WHERE nspname = 'demo_ops')
                    AND pg_trigger.tgname = 'emp_info_sync_lastmod') THEN
		RAISE NOTICE 'Trigger "emp_info_sync_lastmod" already exists.';
	ELSE
        RAISE NOTICE 'Trigger "emp_info_sync_lastmod": Creating..';
        CREATE TRIGGER emp_info_sync_lastmod
        BEFORE INSERT OR UPDATE ON
            demo_ops.emp_info
        FOR EACH ROW EXECUTE PROCEDURE
            sync_lastmod();
        RAISE NOTICE 'Trigger "emp_info_sync_lastmod": Completed';
    END IF;
END $$;


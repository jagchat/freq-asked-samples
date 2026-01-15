DO $$
BEGIN
    IF EXISTS (SELECT 1 
                FROM pg_proc 
                WHERE proname = 'sync_lastmod' 
                AND pronargs = 0 
                AND prorettype = 'trigger'::regtype) THEN
		RAISE NOTICE 'Function "sync_lastmod" already exists.';
	ELSE
        RAISE NOTICE 'Function "sync_lastmod": Creating..';
		CREATE FUNCTION sync_lastmod() 
		RETURNS trigger 
		LANGUAGE plpgsql AS 
		$func$
			BEGIN
                NEW.update_date := CURRENT_TIMESTAMP;
			
                RETURN NEW;
			END;		
		$func$;
        RAISE NOTICE 'Function "sync_lastmod": Completed';
    END IF;
END $$;
\connect admission
ALTER SCHEMA public OWNER TO admission_user;
GRANT USAGE, CREATE, DROP ON SCHEMA public TO admission_user;

\connect auth
ALTER SCHEMA public OWNER TO auth_user;
GRANT USAGE, CREATE, DROP ON SCHEMA public TO auth_user;

\connect dictionary
ALTER SCHEMA public OWNER TO dictionary_user;
GRANT USAGE, CREATE, DROP ON SCHEMA public TO dictionary_user;

\connect mail_inbox
ALTER SCHEMA public OWNER TO mail_inbox_user;
GRANT USAGE, CREATE, DROP ON SCHEMA public TO mail_inbox_user;
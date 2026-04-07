CREATE USER admission_user WITH PASSWORD 'password';
CREATE USER auth_user WITH PASSWORD 'password';
CREATE USER dictionary_user WITH PASSWORD 'password';
CREATE USER mail_inbox_user WITH PASSWORD 'password';
       
CREATE DATABASE admission OWNER admission_user;
CREATE DATABASE auth OWNER auth_user;
CREATE DATABASE dictionary OWNER dictionary_user;
CREATE DATABASE mail_inbox OWNER mail_inbox_user;
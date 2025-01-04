# TDM Engine - Table Driven Migration Engine

## What is it?

`TDMEngine` is an engine that can execute SQL queries on a database connection.
The primary purpose of `TDMEngine` is to maintain and migrate SQL Server databases
through various versions of the database schema.
The SQL commands are stored in a Microsoft Access database and each migration version
is stored within each table.
In addition to DDL and DML instructions, `TDMEngine` can also load updated stored procedures
specific to each version of the database schema.

CREATE TABLE IF NOT EXISTS "User" (
	"Id"	INTEGER PRIMARY KEY AUTOINCREMENT,
	"Username"	TEXT NOT NULL UNIQUE,
	"Password"	TEXT NOT NULL
)

CREATE TABLE IF NOT EXISTS "Employees" (
	"Id"	INTEGER PRIMARY KEY AUTOINCREMENT,
	"UserId"	INTEGER,
	"Name"	BLOB,
	"PhoneNumber"	BLOB,
	"UnableWeekDays"	BLOB,
	"UnableWorkDays"	INTEGER,
	"Roles"	BLOB
);

CREATE TABLE IF NOT EXISTS "Shifts" (
	"Id"	INTEGER PRIMARY KEY AUTOINCREMENT,
	"Name"	BLOB,
	"Roles"	BLOB
);

CREATE TABLE IF NOT EXISTS "Roles" (
	"Id"	INTEGER PRIMARY KEY AUTOINCREMENT,
	"Name"	BLOB
);

CREATE TABLE IF NOT EXISTS "Schedules" (
	"Id"	INTEGER PRIMARY KEY AUTOINCREMENT,
	"Date"	BLOB,
	"Data"	BLOB
);

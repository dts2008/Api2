CREATE SCHEMA backoffice;

USE backoffice;

CREATE TABLE userinfo 
(
	id int NOT NULL AUTO_INCREMENT,
	name varchar(256),
	login varchar(128) NOT NULL,
    password varchar(128) NOT NULL,
	email varchar(512),
	contacts varchar(4096),
    role int,
    activity bigint,
	PRIMARY KEY (`id`)
);

insert into userinfo (name, login, password, email, contacts, role, activity) values('manager','manager', '202cb962ac59075b964b07152d234b70', '', '', 1, 0);

CREATE TABLE partnerinfo
(
	id int NOT NULL AUTO_INCREMENT,
    name varchar(256),
    website varchar(512),    
	manager int,
	added bigint,
    status int,
    description varchar(4096),
    clientType int,
	currency int,
	nextContact bigint,
	PRIMARY KEY (`id`)
 );
 
CREATE TABLE contactinfo
(
	id int NOT NULL AUTO_INCREMENT,
    name varchar(256),
    partnerid int,
    email varchar(512),
    phone varchar(128),
    skype varchar(128),
    telegram varchar(128),
    whatsapp varchar(128),
    comment varchar(4096),
	PRIMARY KEY (`id`)
);

CREATE TABLE partnerfileinfo
(
	id int NOT NULL AUTO_INCREMENT,
    description varchar(4096),
    name varchar(256),
    size bigint,
    added bigint,
	partnerid int,
	fileToken varchar(128),
	PRIMARY KEY (`id`)
);

CREATE TABLE eventinfo
(
	id int NOT NULL AUTO_INCREMENT,
	partnerid int,
    contactid int,
	contactType int,
    eventtime bigint,
	description varchar(4096),
	PRIMARY KEY (`id`)
);

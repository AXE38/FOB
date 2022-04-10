use KP_TP

create sequence dbo.MAIN_SEQ
as decimal(15,0)
start with 1
increment by 1;

create table DICT_ALL (
	id decimal(15,0) PRIMARY KEY,
	group_code varchar(16),
	code varchar(16),
	description varchar(255)
);

insert into DICT_ALL values (NEXT VALUE FOR dbo.MAIN_SEQ, 'LOAN_TYPE', 'CRED', 'Кредит');
insert into DICT_ALL values (NEXT VALUE FOR dbo.MAIN_SEQ, 'LOAN_TYPE', 'CC', 'Кредитная карта');
insert into DICT_ALL values (NEXT VALUE FOR dbo.MAIN_SEQ, 'LOAN_TYPE', 'DC', 'Дебетовая карта');

insert into DICT_ALL values (NEXT VALUE FOR dbo.MAIN_SEQ, 'DB_RESULT', '200', 'ОК');
insert into DICT_ALL values (NEXT VALUE FOR dbo.MAIN_SEQ, 'DB_RESULT', '-1', 'Запрос не содержит XML');
insert into DICT_ALL values (NEXT VALUE FOR dbo.MAIN_SEQ, 'DB_RESULT', '-2', 'Запрос содержит невалидный XML');
insert into DICT_ALL values (NEXT VALUE FOR dbo.MAIN_SEQ, 'DB_RESULT', '-3', 'Неверный логин или пароль');

create table ABS_USER (
	id decimal(15,0) PRIMARY KEY,
	first_name varchar(255),
	last_name varchar(255),
	middle_name varchar(255),
	position varchar(255),
	abs_login varchar(16),
	abs_password varchar(255),
	last_login_date datetime
);

alter table ABS_USER add token varchar(255);
select * from ABS_USER
--пароль ADMIN
insert into ABS_USER values (NEXT VALUE FOR dbo.MAIN_SEQ, 'Администратор Системы', NULL, NULL, NULL, 'ADMIN', '835d6dc88b708bc646d6db82c853ef4182fabbd4a8de59c213f2b5ab3ae7d9be', NULL, NULL);

create table CLIENT (
	id decimal(15,0) PRIMARY KEY,
	fullname varchar(255),
	pass varchar(255),
	phone varchar(255),
	create_date datetime,
	create_user decimal(15,0) references ABS_USER,
	last_update_date datetime,
	last_update_user decimal(15,0) references ABS_USER
);

insert into CLIENT values (NEXT VALUE FOR dbo.MAIN_SEQ, 'Иванов Иван Иванович', '2564111111', '+711111111', getdate(), (select id from ABS_USER where abs_login = 'ADMIN'), NULL, NULL);

create table ACCOUNT (
	id decimal(15,0) PRIMARY KEY,
	collection_id decimal(15,0) references CLIENT,
	num varchar(255),
	agreement_id varchar(255),
	card_pan varchar(255),
	loan_type decimal(15,0) references DICT_ALL,
	iss_s decimal(15,2),
	s decimal(15,2),
	open_date datetime,
	plan_close_date datetime,
	close_date datetime,
	create_date datetime,
	create_user decimal(15,0) references ABS_USER,
	last_update_date datetime,
	last_update_user decimal(15,0) references ABS_USER
);
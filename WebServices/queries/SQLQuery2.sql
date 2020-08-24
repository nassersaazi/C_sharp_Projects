

CREATE PROCEDURE spSaveDetails
	@name varchar(10),
    @type varchar(10)
AS
BEGIN
	insert into users (name,type)
    values(@name,@type)
END

CREATE PROCEDURE spGetAllClients
AS
BEGIN
	select name, type from users where type = 'client'
END



CREATE PROCEDURE spGetAllAdmins
AS
BEGIN
	select name, type from users where type = 'admin'
END
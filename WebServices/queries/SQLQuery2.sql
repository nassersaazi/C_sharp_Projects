select * from users

select type from users

select * from users where type = 'admin'

CREATE PROCEDURE spSaveDetails
	@name varchar(10),
    @type varchar(10)
AS
BEGIN
	insert into users (name,type)
    values(@name,@type)
END
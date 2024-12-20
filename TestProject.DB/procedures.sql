use TestProjectDb;
------------------------------------------------------------------------------------------------------------------------

alter procedure dbo.sp_generate_token @platform_key varchar(12)
as
begin
    set nocount on;

    if not exists (select 1 from dbo.platforms where platform_key = @platform_key and status = 1)
        return;

    insert into dbo.sessions (platform_id, expired_date)
    values (@platform_key, dateadd(hour, 3, getutcdate()));

    if @@rowcount = 1
        select token as Token
        from dbo.sessions
        where id = scope_identity()
        for json path, without_array_wrapper;
end
go

select *
from dbo.platforms

exec dbo.sp_generate_token '3c4d7cab8fc6'

select *
from dbo.sessions
order by id desc

------------------------------------------------------------------------------------------------------------------------

alter procedure dbo.sp_validate_token_and_return_info @token varchar(20)
as
begin
    set nocount on;

    declare @status_id int, @expired_date datetime, @user_id varchar(12), @role varchar(15);

    select @status_id = s.status,
           @expired_date = s.expired_date,
           @user_id = u.user_id,
           @role = IIF(r.[name] is null, 'Guest', r.[name])
    from dbo.sessions s
             left join dbo.user_accounts u on s.user_id = u.user_id
             left join dbo.user_roles ur on u.user_id = ur.user_id
             left join dbo.roles r on ur.role_id = r.role_id
    where s.token = @token;

    if @status_id is null or @status_id = 0 or @status_id = 3
        return;

    if getutcdate() > @expired_date
        begin
            update dbo.sessions
            set status      = 0,
                update_date = getutcdate()
            where token = @token;

            return;
        end;

    select @token         as token,
           @user_id       as user_id,
           @role          as role,
           @expired_date  as expired_date,
           ss.status_name as status
    from dbo.session_statuses ss
    where ss.status_id = @status_id
    for json path, without_array_wrapper;
end
go

select *
from dbo.sessions

exec dbo.sp_validate_token_and_return_info '0c6cb9ec02fc925645ab'

select *
from dbo.sessions
order by id desc

------------------------------------------------------------------------------------------------------------------------

alter proc dbo.sp_client_sign_up @token varchar(20),
                                 @name varchar(50),
                                 @dob datetime,
                                 @password_hash varchar(100)
as
begin
    set nocount on;

    /*
    if exists(select 1 from dbo.sessions where token = @token and user_id is not null)
        return;
    */

    if exists(select user_id from dbo.user_accounts where name = @name and status = 1)
        return;

    begin try
        begin transaction ;
        insert into dbo.user_accounts(name, dob, password_hash)
        values (@name, @dob, @password_hash);

        if @@rowcount = 1
            begin
                declare @user_id varchar(12) = (select user_id from dbo.user_accounts where id = scope_identity());

                insert into dbo.user_roles(user_id, role_id)
                values (@user_id, 0);

                if @@rowcount = 1
                    begin
                        update dbo.sessions
                        set login_date=getutcdate(),
                            expired_date=dateadd(day, 1, getutcdate()),
                            user_id = @user_id
                        where token = @token;

                        if @@rowcount = 1
                            begin
                                select @token as Token
                                for json path , without_array_wrapper;
                                commit;
                                return;
                            end
                    end
            end
        rollback
    end try
    begin catch
        rollback;
    end catch;
end
go

exec dbo.sp_client_sign_up '2ae8071482861f916203', 'Omar', '10/20/2001', 'Aa123456!'

select *
from dbo.user_accounts

select *
from dbo.sessions
order by id desc

------------------------------------------------------------------------------------------------------------------------

/*
alter procedure dbo.sp_client_sign_in @token varchar(20),
                                      @name varchar(50),
                                      @password varchar(256)
as
begin
    set nocount on;

    declare @user_id varchar(12) = (select user_id from dbo.user_accounts where name = @name and status = 1);
    declare @password_hash varchar(256) = (select password_hash from dbo.user_accounts where user_id = @user_id);

    if @user_id is null or @password_hash is null
        begin
            select dbo.fn_select_null_result();
            return;
        end

    if @password_hash != @password
        begin
            select dbo.fn_select_null_result();
            return;
        end

    begin try
        begin transaction;

        update dbo.sessions
        set login_date   = getutcdate(),
            expired_date = dateadd(day, 1, getutcdate()),
            user_id      = @user_id
        where token = @token;

        if @@rowcount = 1
            begin
                select @token as Token
                for json path, without_array_wrapper;
                commit;
                return;
            end
        rollback;
    end try
    begin catch
        rollback;
        select dbo.fn_select_null_result();
    end catch;
end
go
*/

alter procedure dbo.sp_client_sign_in @token varchar(20),
                                      @name varchar(50)
as
begin
    set nocount on;

    declare @user_id varchar(12) = (select user_id from dbo.user_accounts where name = @name and status = 1);
    declare @password_hash varchar(256) = (select password_hash from dbo.user_accounts where user_id = @user_id);

    if @user_id is null
        return;

    select @token         as Token,
           @user_id       as UserId,
           @password_hash as PasswordHash
    for json path, without_array_wrapper;
end
go


------------------------------------------------------------------------------------------------------------------------

alter procedure dbo.sp_client_sign_in_update @token varchar(20),
                                             @user_id varchar(12),
                                             @sql_res bit output
as
begin
    set nocount on;

    set @sql_res = 0;


    update dbo.sessions
    set login_date   = getutcdate(),
        expired_date = dateadd(day, 1, getutcdate()),
        user_id      = @user_id
    where token = @token;

    if @@rowcount = 1
        begin
            set @sql_res = 1;
        end
end
go

------------------------------------------------------------------------------------------------------------------------

alter proc dbo.sp_get_user_date @token varchar(20)
as
begin
    set nocount on;
    declare @user_id varchar(12)=(select user_id from dbo.sessions where token = @token and status = 1)

    select name as Name, dob as Dob from dbo.user_accounts where user_id = @user_id for json path ,without_array_wrapper
end
go

exec dbo.sp_get_user_date '3d73be015c95ea426379'

------------------------------------------------------------------------------------------------------------------------

-- ax^2+bx+c=0

alter procedure dbo.sp_solve_quadratic_equation @a float,
                                                @b float,
                                                @c float,
                                                @root1 float output,
                                                @root2 float output,
                                                @solution_status nvarchar(50) output
as
begin
    if @a = 0
        begin
            set @solution_status = 'Not a quadratic equation';
            set @root1 = null;
            set @root2 = null;
            return;
        end

    declare @discriminant float;
    set @discriminant = (@b * @b) - (4 * @a * @c);

    if @discriminant < 0
        begin
            set @solution_status = 'No real roots';
            set @root1 = null;
            set @root2 = null;
        end
    else
        if @discriminant = 0
            begin
                set @root1 = -@b / (2 * @a);
                set @root2 = @root1;
                set @solution_status = 'One real root';
            end
        else
            begin
                set @root1 = (-@b + sqrt(@discriminant)) / (2 * @a);
                set @root2 = (-@b - sqrt(@discriminant)) / (2 * @a);
                set @solution_status = 'Two real roots';
            end
end
go

declare @root1 float, @root2 float, @status nvarchar(50);

exec dbo.sp_solve_quadratic_equation
     @a = 2,
     @b = 4,
     @c = 2,
     @root1 = @root1 output,
     @root2 = @root2 output,
     @solution_status = @status output;

select @root1 as Root1, @root2 as Root2, @status as Status;
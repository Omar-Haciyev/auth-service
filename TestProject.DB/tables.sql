use TestProjectDb
------------------------------------------------------------------------------------------------------------------------

create table dbo.platforms
(
    id           int identity (1,1),
    platform_key varchar(12) primary key default lower(left(replace(newid(), '-', ''), 12)),
    [name]       varchar(10) not null,
    status       bit                     default 1,
    update_date  datetime                default getutcdate()
)

insert into dbo.platforms([name])
values ('web'),
       ('mobile')

select *
from dbo.platforms

------------------------------------------------------------------------------------------------------------------------

create table dbo.roles
(
    id          int identity (1,1),
    role_id     int primary key,
    [name]      varchar(15) not null,
    status      bit      default 1,
    update_date datetime default getutcdate()
)

insert into dbo.roles(role_id, [name])
values (0, 'Client'),
       (1, 'Admin')

------------------------------------------------------------------------------------------------------------------------

create table dbo.user_accounts
(
    id            int identity (1,1),
    user_id       varchar(12) primary key default lower(left(replace(newid(), '-', ''), 12)),
    name          varchar(50)  not null unique,
    dob           datetime     not null,
    password_hash varchar(100) not null,
    status        bit                     default 1,
    created_date  datetime                default getutcdate(),
    update_date   datetime                default getutcdate()
)

------------------------------------------------------------------------------------------------------------------------

create table dbo.user_roles
(
    id          int identity (1,1),
    user_id     varchar(12) not null foreign key references dbo.user_accounts (user_id),
    role_id     int         not null foreign key references dbo.roles (role_id),
    update_date datetime default getutcdate(),
    status      bit      default 1,
    primary key clustered (user_id, role_id)
)

------------------------------------------------------------------------------------------------------------------------

create table dbo.session_statuses
(
    status_id   int primary key,
    status_name varchar(50) not null,
    update_date datetime default getutcdate(),
    status      bit      default 1
)

insert into dbo.session_statuses (status_id, status_name)
values (0, 'terminated'),
       (1, 'active'),
       (2, 'pending'),
       (3, 'blocked')

------------------------------------------------------------------------------------------------------------------------

create table dbo.sessions
(
    id           int identity (1,1),
    platform_id  varchar(12) foreign key references dbo.platforms (platform_key),
    token        varchar(20) primary key                                  default lower(convert(varchar(20), crypt_gen_random(10), 2)),
    user_id      varchar(12) foreign key references dbo.user_accounts (user_id),
    status       int not null foreign key references dbo.session_statuses default 1,
    created_date datetime                                                 default getutcdate(),
    update_date  datetime                                                 default getutcdate(),
    login_date   datetime,
    logout_date  datetime,
    expired_date datetime,
)
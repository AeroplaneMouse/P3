create table if not exists departments
(
    id         bigint unsigned auto_increment      primary key,
    name       varchar(255)                        not null,
    created_at timestamp default CURRENT_TIMESTAMP null,
    updated_at timestamp                           null
) 
CHARACTER SET utf8 COLLATE utf8_general_ci;

create table if not exists assets
(
    id            bigint unsigned auto_increment         primary key,
    name          varchar(255)                           not null,
    identifier    varchar(255) default ''                null,
    description   text                                   null,
    options       json                                   null,
    department_id bigint unsigned                        not null,
    created_at    timestamp    default CURRENT_TIMESTAMP null,
    updated_at    timestamp                              null,
    deleted_at    timestamp                              null,

    constraint assets_department_id_foreign
        foreign key (department_id) references departments (id)
)
CHARACTER SET utf8 COLLATE utf8_general_ci;

create index assets_identifier_index
    on assets (identifier);

create index assets_name_index
    on assets (name);

create table if not exists users
(
    id                 bigint unsigned auto_increment       primary key,
    domain             varchar(255)                         not null,
    username           varchar(255)                         not null,
    description        text                                 null,
    enabled            tinyint(1) default 1                 not null,
    default_department bigint     default 0                 null,
    admin              tinyint(1) default 0                 not null,
    created_at         timestamp  default CURRENT_TIMESTAMP null,
    updated_at         timestamp                            null,

    constraint users_username_uindex
        unique (username)
)
CHARACTER SET utf8 COLLATE utf8_general_ci;

create index users_domain_username_index
    on users (domain, username);

create table if not exists comments
(
    id         bigint unsigned auto_increment      primary key,
    user_id    bigint unsigned                     not null,
    asset_id   bigint unsigned                     not null,
    content    text                                not null,
    created_at timestamp default CURRENT_TIMESTAMP null,
    updated_at timestamp                           null,
    deleted_at timestamp                           null,

    constraint comments_asset_id_foreign
        foreign key (asset_id) references assets (id)
            on delete cascade,
    constraint comments_users_id_fk
        foreign key (user_id) references users (id)
)
CHARACTER SET utf8 COLLATE utf8_general_ci;

create index comments_user_id_index
    on comments (user_id);

create table if not exists tags
(
    id            bigint unsigned auto_increment      primary key,
    parent_id     bigint unsigned                     not null,
    department_id bigint unsigned                     null,
    label         varchar(255)                        not null,
    color         varchar(9)                          not null,
    options       json                                not null,
    created_at    timestamp default CURRENT_TIMESTAMP null,
    updated_at    timestamp                           null,

    constraint tags_department_id_parent_id_label_uindex
        unique (department_id, parent_id, label),
    constraint tags_department_id_foreign
        foreign key (department_id) references departments (id)
)
CHARACTER SET utf8 COLLATE utf8_general_ci;

INSERT INTO tags (label, color, options, department_id, parent_id, updated_at)
    VALUES ( 'bruger', '#e0218a', '[]', null, 0, CURRENT_TIMESTAMP());

create table if not exists asset_tags
(
    asset_id      bigint unsigned not null,
    tag_id        bigint unsigned not null,
    attached_date timestamp default CURRENT_TIMESTAMP null,

    primary key (asset_id, tag_id),
    constraint asset_tags_asset_id_foreign
        foreign key (asset_id) references assets (id)
            on delete cascade,
    constraint asset_tags_tag_id_foreign
        foreign key (tag_id) references tags (id)
            on delete cascade
)
CHARACTER SET utf8 COLLATE utf8_general_ci;

create index tags_parent_id_index
    on tags (parent_id);

create table if not exists asset_users
(
    asset_id      bigint unsigned not null,
    user_id       bigint unsigned not null,
    attached_date timestamp default CURRENT_TIMESTAMP null,

    primary key (asset_id, user_id),
    constraint asset_users_assets_id_fk
        foreign key (asset_id) references assets (id)
            on delete cascade,
    constraint asset_users_users_id_fk
        foreign key (user_id) references users (id)
            on delete cascade
)
CHARACTER SET utf8 COLLATE utf8_general_ci;

create table if not exists log
(
    id               bigint unsigned auto_increment      primary key,
    user_id          bigint unsigned                     null,
    logged_item_id   bigint unsigned                     null,
    logged_item_type varchar(255)                        null,
    entry_type       varchar(255)                        not null,
    description      text                                not null,
    changes          json                                null,
    created_at       timestamp default CURRENT_TIMESTAMP null,

    constraint log_users_id_fk
        foreign key (user_id) references users (id)
)
CHARACTER SET utf8 COLLATE utf8_general_ci;

create index log_logged_item_id_logged_item_type_index
    on log (logged_item_id, logged_item_type);

create index log_user_id_index
    on log (user_id);


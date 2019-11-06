create table departments
(
    id         bigint unsigned auto_increment
        primary key,
    name       varchar(255)                        not null,
    created_at timestamp default CURRENT_TIMESTAMP null,
    updated_at timestamp                           null
)
    collate = utf8mb4_unicode_ci;

create table assets
(
    id            bigint unsigned auto_increment
        primary key,
    name          varchar(255)                        not null,
    identifier    varchar(255)                        null,
    description   text                                null,
    options       json                                null,
    department_id bigint unsigned                     not null,
    created_at    timestamp default CURRENT_TIMESTAMP null,
    updated_at    timestamp                           null,
    deleted_at    timestamp                           null,
    constraint assets_department_id_foreign
        foreign key (department_id) references departments (id)
)
    collate = utf8mb4_unicode_ci;

create index assets_identifier_index
    on assets (identifier);

create index assets_name_index
    on assets (name);

create table comments
(
    id         bigint unsigned auto_increment
        primary key,
    asset_id   bigint unsigned                     not null,
    username   varchar(255)                        not null,
    content    text                                not null,
    created_at timestamp default CURRENT_TIMESTAMP null,
    updated_at timestamp                           null,
    deleted_at timestamp                           null,
    constraint comments_asset_id_foreign
        foreign key (asset_id) references assets (id)
            on delete cascade
)
    collate = utf8mb4_unicode_ci;

create index comments_username_index
    on comments (username);

create table log
(
    id           bigint unsigned auto_increment
        primary key,
    logable_id   bigint unsigned                     not null,
    logable_type varchar(255)                        not null,
    username     varchar(255)                        not null,
    description  text                                not null,
    options      json                                null,
    created_at   timestamp default CURRENT_TIMESTAMP null,
    updated_at   timestamp                           null
)
    collate = utf8mb4_unicode_ci;

create index log_username_index
    on log (username);

create table tags
(
    id            bigint unsigned auto_increment
        primary key,
    parent_id     bigint unsigned                     not null,
    department_id bigint unsigned                     not null,
    label         varchar(255)                        not null,
    color         varchar(9)                          not null,
    options       json                                not null,
    created_at    timestamp default CURRENT_TIMESTAMP null,
    updated_at    timestamp                           null,
    constraint label
        unique (parent_id, label),
    constraint tags_department_id_foreign
        foreign key (department_id) references departments (id)
)
    collate = utf8mb4_unicode_ci;

create table asset_tags
(
    asset_id   bigint unsigned                     not null,
    tag_id     bigint unsigned                     not null,
    created_at timestamp default CURRENT_TIMESTAMP not null,
    primary key (asset_id, tag_id),
    constraint asset_tags_asset_id_foreign
        foreign key (asset_id) references assets (id)
            on delete cascade,
    constraint asset_tags_tag_id_foreign
        foreign key (tag_id) references tags (id)
            on delete cascade
)
    collate = utf8mb4_unicode_ci;


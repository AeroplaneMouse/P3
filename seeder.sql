SET FOREIGN_KEY_CHECKS = 0;
TRUNCATE TABLE ds303e19.comments;
TRUNCATE TABLE ds303e19.log;
TRUNCATE TABLE ds303e19.assets;
TRUNCATE TABLE ds303e19.tags;
TRUNCATE TABLE ds303e19.asset_tags;
TRUNCATE TABLE ds303e19.departments;
TRUNCATE TABLE ds303e19.users;
SET FOREIGN_KEY_CHECKS = 1;

INSERT INTO departments (id, name, updated_at) VALUES
    (1, 'IT',           CURRENT_TIMESTAMP),
    (2, 'HR',           CURRENT_TIMESTAMP),
    (3, 'Dyrepasserne', CURRENT_TIMESTAMP),
    (4, 'Pedel',        CURRENT_TIMESTAMP);

INSERT INTO assets (id, name, identifier, description, options, department_id, updated_at) VALUES
    (1, 'D-Link Gigabit Netværk Switch Pro', 'WS-C3560G-48PS-E', '8 Port Gigabit Netværk Switch i metalkabinet fra D-Link. 10/100/1000 Mbps, Full Duplex. Denne 8 Port gigabit netværk switch i metalkabinet gør det muligt at tilslutte flere computere eller enheder til et LAN netværk. Metalkabinettet sikrer optimal varmeafledning, da varme forkorter elektroniks levetid gør medvirker metalkabinettet til en lang og driftsikker levetid på switchen.', '[]', 1, current_timestamp),
    (2, 'HP 14-cb001no 14" Bærbar', 'HP-COMPUTER-14', 'Nøglespecifikationer 14 " / HD Celeron / N3060 / 4 GB,32 GB / SSD, Intel HD Graphics 400, Styresystem: Windows 10 Home 64-bit Edition, Batteritid: 8 timer (op til), Intet optisk drev', '[]', 1, current_timestamp),
    (3, 'HP 14-cb001no 14" Bærbar', 'HP-COMPUTER-14', 'Nøglespecifikationer 14 " / HD Celeron / N3060 / 4 GB,32 GB / SSD, Intel HD Graphics 400, Styresystem: Windows 10 Home 64-bit Edition, Batteritid: 8 timer (op til), Intet optisk drev', '[]', 1, current_timestamp),
    (4, 'Cisco 4000 Series Integrated Services Routers', 'CISCO-4000', 'Protect your branch site and prepare it for the future with the Cisco 4000 Series ISR digital-ready platform. Simplify day-to-day IT management. Provide a scalable, flexible foundation so you can quickly integrate leading IT initiatives like SD-WAN and edge compute, while meeting the explosive network performance need driven by cloud-based application adoption.', '[]', 1, current_timestamp),
    (5, 'Logitech M705 Marathon Mouse', 'LOG-MUS-M705', 'Mus, laser sensor, 7 knapper, 1000 dpi, trådløs - 2.4 GHz med op til 10 meters rækkevidde, op til 36 måneders batterilevetid, kan bruges til højrehåndede, sort / grå', '[]', 1, current_timestamp);

INSERT INTO tags (id, parent_id, department_id, label, color, options, updated_at) VALUES
    (1,  0, 1, 'bruger',          '#FFFFFF', '[]', current_timestamp),
    (2,  0, 1, 'placering',       '#FFFFFF', '[]', current_timestamp),
    (3,  0, 1, 'status',          '#FFFFFF', '[]', current_timestamp),
    (4,  2, 1, 'løvehuset',       '#FFFFFF', '[]', current_timestamp),
    (5,  2, 1, 'elefanthuset',    '#FFFFFF', '[]', current_timestamp),
    (6,  2, 1, 'skoletjenesten',  '#FFFFFF', '[]', current_timestamp),
    (7,  2, 1, 'receptionen',     '#FFFFFF', '[]', current_timestamp),
    (8,  2, 1, 'adminstrationen', '#FFFFFF', '[]', current_timestamp),
    (9,  3, 1, 'ødelagt',         '#FFFFFF', '[]', current_timestamp),
    (10, 3, 1, 'reperation',      '#FFFFFF', '[]', current_timestamp),
    (11, 3, 1, 'udlånt',          '#FFFFFF', '[]', current_timestamp),
    (12, 0, 1, 'switch',          '#FFFFFF', '[]', current_timestamp),
    (13, 0, 1, 'router',          '#FFFFFF', '[]', current_timestamp),
    (14, 0, 1, 'computer',        '#FFFFFF', '[]', current_timestamp),
    (15, 0, 1, 'mus',             '#FFFFFF', '[]', current_timestamp);

INSERT INTO users (id, name, username, admin, enabled, default_department, updated_at) VALUES
    (1, 'Thomas Lorentzen' , 'DESKTOP-7H0OV05\\Thomas Lorentzen', 1, 1, 1, current_timestamp),
    (2, 'Niels Vistisen'   , 'DESKTOP-HS1LFMI\\Niels',            1, 1, 1, current_timestamp),
    (3, 'Alexander Nykjær' , 'DESKTOP-184RS8M\\Alexander N',      1, 1, 1, current_timestamp),
    (4, 'Jakob Sønderby'   , 'DESKTOP-BJDTN5J\\jakob',            1, 1, 1, current_timestamp),
    (5, 'Daniel Fly'       , 'BACON-SURFACE\\Daniel',             1, 1, 1, current_timestamp),
    (6, 'Ane Jørgensen'    , 'DESKTOP-SE0PHJK\\aneso',            1, 1, 1, current_timestamp),
    (7, 'Michelle Terpling', 'STUDENT\\mterpl18',                 1, 1, 1, current_timestamp);

INSERT INTO asset_tags (asset_id, tag_id) VALUES
    (1, 12), (1, 5), (1, 9),
    (2, 14), (2, 8),
    (3, 14), (3, 7),
    (4, 13), (4, 6),
    (5, 15);

INSERT INTO comments (id, asset_id, username, content, updated_at) VALUES
    (1, 1, 'tgl', 'Dette er en kommentar!', current_timestamp);
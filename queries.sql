/* Get all assets */
SELECT a.* FROM assets a WHERE a.department_id = 1;

/* Get assets with tags */
SELECT a.* FROM assets AS a
INNER JOIN asset_tags AS atr ON (a.id = atr.asset_id)
WHERE atr.tag_id IN (1,5,6) GROUP BY a.id;

/* Get asset tags */
SET @asset_id = 1;
SELECT t.* FROM tags AS t
INNER JOIN asset_tags a on t.id = a.tag_id
WHERE a.asset_id = @asset_id;

/* Get asset comments */
SET @asset_id = 1;
SELECT * FROM comments WHERE asset_id = @asset_id;

/* Find tags while writing */
SELECT t.id, t.label FROM tags t WHERE department_id = 1 AND t.label LIKE 'u%';

/* Truncate all tables in integrationtest DB */
SET FOREIGN_KEY_CHECKS = 0;
TRUNCATE TABLE ds303e19_test.comments;
TRUNCATE TABLE ds303e19_test.log;
TRUNCATE TABLE ds303e19_test.assets;
TRUNCATE TABLE ds303e19_test.tags;
TRUNCATE TABLE ds303e19_test.asset_tags;
TRUNCATE TABLE ds303e19_test.departments;
SET FOREIGN_KEY_CHECKS = 1;
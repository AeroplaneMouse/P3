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


/* HAVE TO CONTAIN ALL TAGS AND USERS */
SELECT a.id, a.name
FROM assets AS a
INNER JOIN asset_tags AS at
    ON at.asset_id = a.id
INNER JOIN asset_users AS au
    ON au.asset_id = a.id
WHERE at.tag_id IN (12,14) AND au.user_id IN (1)
GROUP BY a.id
HAVING count(DISTINCT at.tag_id) = 2
   AND count(DISTINCT au.user_id) = 1;


/* DOES NOT HAVE TO CONTAIN BUT JUST HAVE ONE OR MORE TAGS AND USERS*/
SELECT a.id, a.name
FROM assets AS a
INNER JOIN asset_tags AS at
    ON at.asset_id = a.id
INNER JOIN asset_users AS au
    ON au.asset_id = a.id
WHERE at.tag_id IN (12,14) AND au.user_id IN (1)
GROUP BY a.id;
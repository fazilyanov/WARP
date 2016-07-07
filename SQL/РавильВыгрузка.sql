USE [testbase]
GO

SELECT  a.id_user,_users.name, _bases.name as base, _roles.name AS role
FROM  _bases
JOIN ( SELECT * FROM _users_roles_bases WHERE deleted=0 ) a ON a.id_base = _bases.id 
JOIN _roles ON a.id_role = _roles.id AND _roles.deleted=0
JOIN _users ON a.id_user = _users.id AND _users.deleted=0
WHERE _bases.deleted = 0 
Order by a.id_user
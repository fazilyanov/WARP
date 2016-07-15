CREATE TABLE Files (
fileGUID uniqueidentifier default newid() unique rowguidcol not null,
fileDATA varbinary(max) filestream
)
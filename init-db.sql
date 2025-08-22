-- Script de inicialização do banco local
CREATE DATABASE SimulacaoCredito;
GO

-- Criar usuário hackathon
CREATE LOGIN hackathon WITH PASSWORD = 'TimeBECIDNaSegundaFase';
GO

USE SimulacaoCredito;
GO

CREATE USER hackathon FOR LOGIN hackathon;
GO

-- Dar permissões ao usuário
ALTER ROLE db_owner ADD MEMBER hackathon;
GO

-- As tabelas serão criadas via EF Core Migrations

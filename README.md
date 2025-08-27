# API de Simulação de Crédito

Sistema para simulação de crédito com cálculos de amortização SAC e PRICE, desenvolvido em .NET 8 com Clean Architecture.

## 🚀 Como Executar

### **Pré-requisitos**
- Docker Desktop
- .NET 8.0 SDK (para desenvolvimento local)

### **🐳 Execução via Docker**

```bash
# Clonar o repositório
git clone <url-do-repositorio>
cd desafio-tecnico-simulacao-credito

# Subir toda a aplicação
docker-compose up -d build

# Acessar Swagger UI
http://localhost:5077/swagger
```

### **💻 Execução Local**

```bash
# Subir apenas o banco de dados
docker-compose up -d sqlserver

# Executar a aplicação
cd SimulacaoCredito
dotnet run

# Acessar Swagger UI
http://localhost:5077/swagger
```

## 🔐 Autenticação

A API utiliza **autenticação JWT Bearer Token**. Todos os endpoints (exceto autenticação) requerem um token válido.

### **Usuários de Teste**

Os usuários são armazenados no banco de dados com senhas hasheadas via BCrypt.

**Usuário criado via migration:**
- **Username:** `testuser`
- **Password:** `admin123`
- **Email:** `testuser@simulacaocredito.com`

> **🔒 Segurança:** Todas as senhas são armazenadas com hash BCrypt (work factor 12) no banco de dados. Nenhuma credencial está hardcoded no código.

### **Consultar Usuários no Banco**

Para verificar usuários existentes no banco de dados:

```sql
-- Listar todos os usuários
SELECT Id, Username, Email, NomeCompleto, Ativo, DataCriacao, TentativasLogin, ContaBloqueada
FROM Usuarios;

-- Verificar usuário específico
SELECT * FROM Usuarios WHERE Username = 'testuser';
```

### **Criar Novos Usuários**

Para criar novos usuários, você pode:

1. **Via SQL direto no banco:**
```sql
INSERT INTO Usuarios (Username, PasswordHash, Email, NomeCompleto, Ativo, DataCriacao, TentativasLogin, ContaBloqueada)
VALUES (
    'novouser',
    '$2a$12$[HASH_BCRYPT_AQUI]',
    'novouser@exemplo.com',
    'Novo Usuário',
    1,
    SYSDATETIMEOFFSET(),
    0,
    0
);
```

2. **Via migration (recomendado):**
```bash
dotnet ef migrations add AdicionarNovoUsuario --project SimulacaoCredito/SimulacaoCredito.csproj
# Editar a migration para incluir INSERT SQL
dotnet ef database update --project SimulacaoCredito/SimulacaoCredito.csproj
```

### **Obter Token JWT**
```bash
curl -X POST "http://localhost:5077/api/v1/auth/token" \
  -H "Content-Type: application/json" \
  -d '{"username": "testuser", "password": "admin123"}'
```

**Resposta:**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "tokenType": "Bearer",
  "expiresAt": "2025-08-27T18:30:00Z",
  "username": "testuser"
}
```

## 📋 Endpoints da API

**⚠️ Todos os endpoints (exceto `/auth/token`) requerem autenticação JWT**

### **🔐 Autenticação**
- `POST /api/v1/auth/token` - Gerar token JWT (público)
- `GET /api/v1/auth/validate` - Validar token atual

### **📊 Simulações**
- `POST /api/v1/simulacoes` - Criar nova simulação
- `GET /api/v1/simulacoes` - Listar simulações (paginado)
- `GET /api/v1/simulacoes/{id}` - Obter simulação por ID
- `GET /api/v1/simulacoes/por-produto` - Volume por produto/dia

### **📈 Telemetria**
- `GET /api/v1/telemetria` - Dados de observabilidade

### **🏪 Produtos**
- `GET /api/v1/produtos` - Listar todos os produtos
- `GET /api/v1/produtos/elegiveis` - Produtos elegíveis por valor/prazo

## 📦 Como Usar com Postman

### **📥 Importar Collection**

1. **Baixe os arquivos:**
   - `SimulacaoCredito.postman_collection.json`
   - `SimulacaoCredito.postman_environment.json`

2. **No Postman:**
   - Clique em **Import** → **File**
   - Selecione ambos os arquivos
   - Escolha o environment **"Simulação Crédito - Local"**

### **🚀 Usar a Collection**

1. **Gerar Token:**
   - Execute `POST Generate JWT Token` na pasta **Authentication**
   - O token será **automaticamente salvo** para uso nas outras requests

2. **Fazer Requests:**
   - Todos os outros endpoints usarão o token automaticamente
   - Não precisa configurar nada manualmente!

### **⚙️ Configurar Environment (Opcional)**

Se precisar alterar configurações:

| Variável | Valor Padrão | Descrição |
|----------|--------------|-----------|
| `base_url` | `http://localhost:5077` | URL da API |
| `username` | `testuser` | Usuário para autenticação |
| `password` | `admin123` | Senha para autenticação |

## 🧪 Exemplos com cURL

### **1. Obter Token**
```bash
curl -X POST "http://localhost:5077/api/v1/auth/token" \
  -H "Content-Type: application/json" \
  -d '{"username": "testuser", "password": "admin123"}'
```

### **2. Criar Simulação**
```bash
curl -X POST "http://localhost:5077/api/v1/simulacoes" \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer SEU_TOKEN_AQUI" \
  -d '{
    "valorDesejado": 50000,
    "prazo": 12
  }'
```

### **3. Listar Simulações**
```bash
curl -X GET "http://localhost:5077/api/v1/simulacoes?pagina=1&tamanhoPagina=10" \
  -H "Authorization: Bearer SEU_TOKEN_AQUI"
```

### **4. Obter Simulação por ID**
```bash
curl -X GET "http://localhost:5077/api/v1/simulacoes/1" \
  -H "Authorization: Bearer SEU_TOKEN_AQUI"
```

### **5. Verificar Produtos Elegíveis**
```bash
curl -X GET "http://localhost:5077/api/v1/produtos/elegiveis?valor=50000&prazo=12" \
  -H "Authorization: Bearer SEU_TOKEN_AQUI"
```

## 🗃️ Configuração de Bancos

### **Banco Externo (Consulta de Produtos)**
- **Host:** dbhackathon.database.windows.net:1433
- **Database:** hack
- **Usuário:** hack
- **Senha:** Password23
- **Tabela:** dbo.Produto

### **Banco Local (Persistência - Docker)**
- **Host:** localhost:1433
- **Database:** SimulacaoCredito
- **Usuário:** hackathon
- **Senha:** TimeBECIDNaSegundaFase

### **Conectar no DBeaver:**
```
Tipo: SQL Server
Host: localhost
Porta: 1433
Database: SimulacaoCredito
Usuário: hackathon
Senha: TimeBECIDNaSegundaFase
```

## ⚙️ **Configuração Avançada**

### **Variáveis de Ambiente**
Para configuração personalizada, você pode definir as seguintes variáveis:

```bash
# Banco de dados local
ConnectionStrings__DefaultConnection="Server=localhost,1433;Database=SimulacaoCredito;User Id=hackathon;Password=TimeBECIDNaSegundaFase;TrustServerCertificate=true;"

# EventHub (opcional - para integração com Azure)
EventHub__ConnectionString="Endpoint=sb://..."
EventHub__EventHubName="simulacao-eventos"

# Serilog - Configuração de Logs
Serilog__MinimumLevel__Default="Information"
Serilog__MinimumLevel__Override__Microsoft="Warning"
Serilog__MinimumLevel__Override__System="Warning"
Serilog__WriteTo__0__Name="Console"
Serilog__WriteTo__1__Name="File"
Serilog__WriteTo__1__Args__path="logs/simulacao-credito-.log"
Serilog__WriteTo__1__Args__rollingInterval="Day"
```

### **Configuração Docker**
Para usar variáveis de ambiente no Docker, adicione ao `docker-compose.yml`:

```yaml
environment:
  - Serilog__MinimumLevel__Default=Information
  - Serilog__WriteTo__0__Name=Console
  - Serilog__WriteTo__1__Name=File
  - Serilog__WriteTo__1__Args__path=/app/logs/simulacao-credito-.log
```

## 🏗️ Arquitetura (Clean Architecture + DDD)

```
SimulacaoCredito/
├── Domain/                  # 🎯 Entidades de negócio
│   └── Entities/            # Simulacao, Parcela, Produto, TelemetriaRequisicao
├── Application/             # 📋 Camada de aplicação
│   ├── Contracts/           # Contratos de validação
│   ├── DTOs/                # Contratos de entrada/saída
│   └── Interfaces/          # Abstrações de serviços
├── Infrastructure/          # 🔧 Implementações
│   ├── Data/                # Contextos e factory
│   ├── Services/            # Serviços concretos (SAC/PRICE/EventHub)
│   ├── Repositories/        # Repositórios de dados
│   ├── Security/            # Autenticação e autorização
│   └── Middleware/          # Telemetria e observabilidade
├── Controllers/             # 🌐 Endpoints da API
├── Migrations/              # 🚀 Migrações do banco de dados
└── Tests/                   # 🧪 Testes automatizados
```

## ✅ Recursos Implementados

### **🧮 Cálculos Financeiros**
- **Sistema SAC** - Amortização constante, juros decrescentes
- **Sistema PRICE** - Prestação fixa, amortização crescente
- **Algoritmos precisos** com arredondamentos matemáticos corretos

### **🔌 Integrações**
- **Azure EventHub** - Publicação automática de eventos de simulação
- **SQL Server Externo** - Consulta de produtos parametrizados
- **SQL Server Local** - Persistência de simulações via Docker

### **📊 Observabilidade**
- **Telemetria completa** - Middleware para capturar métricas
- **Logs estruturados** - Rastreamento de requisições e erros
- **Métricas de performance** - Tempo de resposta, taxa de sucesso

### **🛡️ Qualidade**
- **Validações robustas** - Biblioteca Flunt com contratos de validação personalizados
- **Regras de negócio** - Validações específicas para diferentes cenários de crédito
- **Tratamento global de erros** - Exception Handler middleware com respostas RFC 7807 (Problem Details)
- **Logs avançados** - Serilog com structured logging e múltiplos sinks
- **Testes automatizados** - Cobertura completa da aplicação incluindo validações

### **🚨 Tratamento de Erros**
A API implementa um sistema robusto de tratamento global de erros:

#### **Exception Handler Global**
- **Middleware personalizado** captura todas as exceções não tratadas
- **Respostas padronizadas** seguindo RFC 7807 (Problem Details)
- **Stack traces protegidos** - informações sensíveis não expostas em produção
- **Logging estruturado** para monitoramento e debugging

#### **Mapeamento de Exceções**
| Tipo de Erro | Status Code | Resposta |
|--------------|-------------|----------|
| Parâmetros inválidos | 400 | Bad Request com detalhes da validação |
| Parâmetro obrigatório ausente | 400 | Bad Request |
| Operação inválida | 400 | Bad Request |
| Acesso negado | 401 | Unauthorized |
| Recurso não encontrado | 404 | Not Found |
| Tempo limite excedido | 408 | Request Timeout |
| Funcionalidade não implementada | 501 | Not Implemented |
| Erro interno | 500 | Internal Server Error |

#### **Formato de Resposta de Erro**
```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "Parâmetros inválidos",
  "detail": "Um ou mais parâmetros fornecidos são inválidos.",
  "status": 400,
  "instance": "/api/v1/simulacoes"
}
```

## 🔧 Troubleshooting

### **❌ Problemas Comuns de Autenticação**

#### **401 Unauthorized**
```json
{
  "type": "https://tools.ietf.org/html/rfc7235#section-3.1",
  "title": "Unauthorized",
  "status": 401,
  "detail": "Token JWT ausente ou inválido"
}
```

**Soluções:**
- Verifique se o header `Authorization: Bearer TOKEN` está presente
- Confirme se o token não expirou (válido por 60 minutos)
- Gere um novo token usando `/api/v1/auth/token`

#### **403 Forbidden**
- Token válido mas sem permissões suficientes
- Verifique se está usando as credenciais corretas

#### **Token Expirado**
- Tokens JWT expiram em **60 minutos**
- Gere um novo token quando necessário
- No Postman, execute novamente "Generate JWT Token"

### **🔍 Validar Token**
```bash
curl -X GET "http://localhost:5077/api/v1/auth/validate" \
  -H "Authorization: Bearer SEU_TOKEN_AQUI"
```

### **📊 Logs Avançados**
A aplicação utiliza **Serilog** para logging estruturado e avançado:

#### **Características do Sistema de Logs**
- **Structured Logging** - Logs com contexto estruturado e propriedades tipadas
- **Múltiplos Sinks** - Console para desenvolvimento e arquivos para produção
- **Formatação personalizada** - Templates otimizados para legibilidade
- **Enrichers** - Contexto automático (aplicação, ambiente, correlação)
- **Níveis configuráveis** - Controle granular por namespace

#### **Configuração de Sinks**
| Sink | Uso | Formato |
|------|-----|---------|
| **Console** | Desenvolvimento | `[HH:mm:ss INF] SourceContext: Message` |
| **File** | Produção | `[yyyy-MM-dd HH:mm:ss.fff INF] SourceContext: Message {Properties}` |
| **Rolling Files** | Arquivos diários | `logs/simulacao-credito-YYYYMMDD.log` |

#### **Logs Estruturados Implementados**
- **Simulações** - Criação, busca e listagem com contexto completo
- **Produtos** - Consultas de elegibilidade com parâmetros
- **Erros** - Exceções com contexto da requisição (path, method, IP)
- **Telemetria** - Métricas de performance e uso da API
- **EventHub** - Publicação de eventos com tamanho dos dados

#### **Exemplo de Log Estruturado**
```
[15:26:45 INF] SimulacaoController: Iniciando criação de simulação. ValorDesejado: 5000, Prazo: 12
[15:26:45 INF] SimulacaoController: Simulação criada com sucesso. ID: 14, Produto: 1, ValorTotal: 5000.00
[15:26:45 ERR] EventHubService: Erro ao publicar evento no EventHub. TipoEvento: SimulacaoCriada, DataSize: 156 bytes
```

### **✅ Validações Implementadas**
- **SimulacaoRequestContract** - Validação de dados de entrada para simulações
  - Valor desejado entre R$ 0,01 e R$ 10.000.000,00
  - Prazo entre 1 e 420 meses
  - Regras de negócio específicas (ex: valores baixos com prazos altos)
- **ProdutoElegivelContract** - Validação de parâmetros para consulta de produtos
- **PaginacaoContract** - Validação de parâmetros de paginação
- **TelemetriaContract** - Validação de datas para consulta de telemetria

## 📊 Dados de Teste

O sistema vem com produtos pré-configurados no banco externo:

### **Produtos Disponíveis**
- **Produto 1**: Taxa 1,79% | 0-24 meses | R$ 200-10.000
- **Produto 2**: Taxa 1,75% | 25-48 meses | R$ 10.000-100.000
- **Produto 3**: Taxa 1,82% | 49-96 meses | R$ 100.000-1.000.000
- **Produto 4**: Taxa 1,51% | 97+ meses | R$ 1.000.000+

## 🧪 Testes

```bash
# Executar todos os testes
dotnet test

# Executar apenas testes de validação Flunt
dotnet test --filter "FluntValidationTests"

# Executar com cobertura
dotnet test --collect:"XPlat Code Coverage"
```

### **Testes de Validação**
Os testes incluem validação de:
- ✅ Dados válidos e inválidos para simulações
- ✅ Regras de negócio específicas (valor vs prazo)
- ✅ Validação de parâmetros de consulta
- ✅ Validação de paginação
- ✅ Validação de datas para telemetria

### **Reset Completo**
```bash
docker-compose down -v
docker-compose up -d --build
```

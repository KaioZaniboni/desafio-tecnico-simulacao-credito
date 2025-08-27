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
docker-compose up -d

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

## 📋 Endpoints da API

Todos os endpoints têm o prefixo `/api/v1/`:

### **Simulações**
- `POST /api/v1/simulacoes` - Criar nova simulação
- `GET /api/v1/simulacoes` - Listar simulações (paginado)
- `GET /api/v1/simulacoes/{id}` - Obter simulação por ID
- `GET /api/v1/simulacoes/por-produto` - Volume por produto/dia

### **Telemetria**
- `GET /api/v1/telemetria` - Dados de observabilidade

### **Produtos (Debug)**
- `GET /api/v1/produtos` - Listar todos os produtos
- `GET /api/v1/produtos/elegiveis` - Produtos elegíveis por valor/prazo

## 🧪 Exemplos de Uso

### **Criar Simulação**
**JSON de exemplo:**
```json
{
  "valorDesejado": 5000,
  "prazo": 12
}
```

### **Listar Simulações**
```bash
curl -X GET "http://localhost:5077/api/v1/simulacoes?pagina=1&tamanhoPagina=10"
```

### **Obter Simulação por ID**
```bash
curl -X GET "http://localhost:5077/api/v1/simulacoes/1"
```

### **Verificar Produtos Elegíveis**
```bash
curl -X GET "http://localhost:5077/api/v1/produtos/elegiveis?valor=5000&prazo=12"
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
│   └── Middleware/          # Telemetria e observabilidade
├── Controllers/             # 🌐 Endpoints da API
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

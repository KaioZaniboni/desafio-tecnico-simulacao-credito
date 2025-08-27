# API de Simulação de Crédito

Sistema para simulação de crédito com cálculos de amortização SAC e PRICE, desenvolvido em .NET 8 com Clean Architecture.

## 🚀 Como Executar

### **Pré-requisitos**
- Docker Desktop

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
- **Tabela:** dbo.Produto (4 produtos disponíveis)

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

## 🏗️ Arquitetura (Clean Architecture + DDD)

```
SimulacaoCredito/
├── Domain/                  # 🎯 Entidades de negócio
│   └── Entities/            # Simulacao, Parcela, Produto, TelemetriaRequisicao
├── Application/             # 📋 Camada de aplicação
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
- **Validações robustas** - Data Annotations nos DTOs
- **Tratamento de erros** - Respostas estruturadas com Problem Details
- **Testes automatizados** - Cobertura completa da aplicação

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

# Executar com cobertura
dotnet test --collect:"XPlat Code Coverage"
```

### **Reset Completo**
```bash
docker-compose down -v
docker-compose up -d --build
```

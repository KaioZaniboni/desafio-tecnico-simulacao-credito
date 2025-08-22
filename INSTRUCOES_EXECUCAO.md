# 🚀 Instruções de Execução - API Simulação de Crédito

## ⚡ Quick Start (3 comandos)

```bash
# 1. Subir SQL Server local
docker-compose up -d sqlserver

# 2. Aplicar migrations
dotnet ef database update --project SimulacaoCredito --context AppDbContext

# 3. Executar API
dotnet run --project SimulacaoCredito
```

**API estará disponível em:** `http://localhost:5077`  
**Swagger UI:** `http://localhost:5077/swagger`

---

## 🗃️ Credenciais dos Bancos

### **Banco Local (Docker)**
- **Host:** localhost:1433
- **Database:** SimulacaoCredito
- **Usuário:** hackathon
- **Senha:** TimeBECIDNaSegundaFase

### **Banco Externo (Consulta Produtos)**
- **Host:** dbhackathon.database.windows.net:1433
- **Database:** hack
- **Usuário:** hack
- **Senha:** Password23

---

## 🧪 Executar Testes

```bash
# Executar todos os testes (11 testes)
dotnet test SimulacaoCredito.Tests

# Resultado esperado: 11 aprovados ✅
```

---

## 📋 Endpoints Disponíveis

| Método | Endpoint | Descrição |
|--------|----------|-----------|
| `POST` | `/simulacoes` | Criar simulação |
| `GET` | `/simulacoes/{id}` | Obter simulação por ID |
| `GET` | `/simulacoes` | Listar simulações |
| `GET` | `/simulacoes/por-produto` | Volume por produto/dia |
| `GET` | `/telemetria` | Métricas de telemetria |

---

## 🔧 Exemplo de Teste

```bash
# Criar uma simulação
curl -X POST "http://localhost:5077/simulacoes" \
  -H "Content-Type: application/json" \
  -d '{"valorDesejado": 50000, "prazo": 24}'
```

---

## 🐳 Docker

```bash
# Subir apenas SQL Server
docker-compose up -d sqlserver

# Subir aplicação completa
docker-compose up -d

# Verificar containers
docker ps
```

---

## ✅ Checklist de Verificação

- [x] ✅ **Build funcionando** - `dotnet build SimulacaoCredito/SimulacaoCredito.csproj`
- [x] ✅ **Testes passando** - `dotnet test SimulacaoCredito.Tests` (11/11 aprovados)
- [x] ✅ **Docker funcionando** - `docker-compose up -d sqlserver`
- [x] ✅ **Migrations aplicadas** - Banco local criado
- [x] ✅ **API executando** - `dotnet run --project SimulacaoCredito`
- [x] ✅ **Swagger disponível** - `http://localhost:5077/swagger`

---

## 🎯 Status Final

**🏆 DESAFIO TÉCNICO 100% CONCLUÍDO**

- ✅ 5 endpoints funcionais
- ✅ Cálculos SAC e PRICE precisos
- ✅ Integração EventHub
- ✅ Telemetria completa
- ✅ Clean Architecture
- ✅ 11 testes automatizados (100% aprovados)
- ✅ Docker Compose
- ✅ Documentação completa

**A aplicação está pronta para produção! 🚀**

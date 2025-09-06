# Sales Management API 

[![.NET 8](https://img.shields.io/badge/.NET-8-purple)](https://dotnet.microsoft.com/)
[![PostgreSQL](https://img.shields.io/badge/PostgreSQL-13+-blue)](https://www.postgresql.org/)
[![Swagger](https://img.shields.io/badge/API-Swagger-green)](http://localhost:5119/swagger)
[![Tests](https://img.shields.io/badge/Tests-41_Passing-brightgreen)](#-testes)
[![License](https://img.shields.io/badge/License-Evaluation_Only-red.svg)](LICENSE)

Uma API REST robusta para gerenciamento de vendas, implementada com **Clean Architecture**, **DDD** e **CQRS patterns**.

## ⚠️ IMPORTANTE - LICENÇA RESTRITIVA

**🔒 Este código é apenas para AVALIAÇÃO e TESTE**
- ✅ Permitido: Baixar, executar e testar localmente
- ❌ Proibido: Uso comercial, modificação, distribuição ou reutilização
- 📋 Veja o arquivo [LICENSE](LICENSE) para detalhes completos

## 📋 Índice

- [Sobre o Projeto](#-sobre-o-projeto)
- [Tecnologias](#-tecnologias)
- [Pré-requisitos](#-pré-requisitos)
- [Instalação Rápida](#-instalação-rápida)
- [Configuração](#️-configuração)
- [Como Executar](#-como-executar)
- [Testando a API](#-testando-a-api)
- [Estrutura do Projeto](#-estrutura-do-projeto)
- [Regras de Negócio](#-regras-de-negócio)
- [API Endpoints](#-api-endpoints)
- [Testes](#-testes)

## 🎯 Sobre o Projeto

Sistema completo de gestão de vendas com operações CRUD, sistema de descontos automáticos e validações de negócio. Desenvolvido seguindo as melhores práticas de **Clean Architecture** e **Domain-Driven Design**.

### ✨ Principais Funcionalidades

- 🛒 **CRUD completo de vendas**
- 💰 **Sistema de descontos automáticos por quantidade**
- 📊 **Listagem paginada de vendas**
- ✅ **Validações de regras de negócio**
- 🔔 **Domain Events** (SaleCreated, SaleModified, SaleCancelled)
- 📚 **Documentação automática com Swagger**

## 🛠 Tecnologias

- **.NET 8.0** - Framework principal
- **C# 12** - Linguagem de programação
- **PostgreSQL** - Banco de dados
- **Entity Framework Core** - ORM
- **MediatR** - Padrão CQRS
- **FluentValidation** - Validações
- **Swagger/OpenAPI** - Documentação da API
- **xUnit** - Testes unitários

## 📋 Pré-requisitos

### Obrigatório
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) 
- [PostgreSQL 13+](https://www.postgresql.org/download/) ou [Docker](https://www.docker.com/)

### Recomendado
- [Visual Studio 2022](https://visualstudio.microsoft.com/) ou [VS Code](https://code.visualstudio.com/)
- [Git](https://git-scm.com/)

### ✅ Verificar Instalações
```bash
dotnet --version    # Deve mostrar 8.x.x
psql --version     # PostgreSQL
git --version      # Git
```

## 🚀 Instalação Rápida

### 1️⃣ Clone o Repositório
```bash
git clone https://github.com/johnnydevsolutions/sales-management-api.git
cd sales-management-api
```

### 2️⃣ Instale as Dependências
```bash
dotnet restore Ambev.DeveloperEvaluation.sln
```

### 3️⃣ Configure o Banco de Dados

**Opção A: PostgreSQL Local (Recomendado)**

1. Certifique-se que o PostgreSQL está rodando localmente
2. Crie o banco e usuário:

```sql
-- Conectar ao PostgreSQL e executar:
CREATE DATABASE developer_evaluation;
-- Usuário postgres já existe, apenas certifique-se da senha
```

3. Atualize a senha no arquivo de configuração:
   - Edite `src/Ambev.DeveloperEvaluation.WebApi/appsettings.Development.json`
   - Substitua `YOUR_PASSWORD_HERE` pela senha do seu PostgreSQL local

> Nota: se você já tiver o PostgreSQL local rodando na porta padrão (5432), deixe como está. A opção Docker (B) usa a porta 5433 por padrão para evitar conflito com essa instalação local.
  
**Opção B: Docker (Alternativa)**

```bash
docker run --name postgres-sales -e POSTGRES_DB=sales_db -e POSTGRES_USER=sales_user -e POSTGRES_PASSWORD=sales_pass -p 5433:5432 -d postgres:13
```

> **Nota:** Usando porta 5433 para evitar conflito com PostgreSQL local.

### 4️⃣ Execute as Migrações
```bash
# Comando padrão (usando appsettings)
dotnet ef database update --project src/Ambev.DeveloperEvaluation.ORM --startup-project src/Ambev.DeveloperEvaluation.WebApi
```

> Se ocorrer erro de autenticação (28P01) e você quiser isolar a connection string sem editar arquivos de configuração, rode:
```bash
dotnet ef database update --connection "Host=localhost;Port=5432;Database=developer_evaluation;Username=postgres;Password=YOUR_PASSWORD" --project src/Ambev.DeveloperEvaluation.ORM --startup-project src/Ambev.DeveloperEvaluation.WebApi
```

Verifique também a conexão interativa com o cliente psql (substitua o caminho se necessário):

```bash
# No PowerShell
& 'D:\Postgres\bin\psql.exe' -h localhost -p 5432 -U postgres -d developer_evaluation

# Dentro do psql, verificar arquivo de configuração ativo
SHOW hba_file;
```

Se o `pg_hba.conf` não permitir autenticação por senha em 127.0.0.1, ajuste para usar `md5` e reinicie o serviço do PostgreSQL.

### 5️⃣ Execute a Aplicação
```bash
cd src/Ambev.DeveloperEvaluation.WebApi
dotnet run
```

### 6️⃣ Acesse a API
- **🌐 Swagger UI:** http://localhost:5119/swagger
- **📡 API Base:** http://localhost:5119/api

## ⚙️ Configuração

### Configurar Banco de Dados

O projeto utiliza PostgreSQL. Configure conforme sua preferência:

**PostgreSQL Local:**

Edite: `src/Ambev.DeveloperEvaluation.WebApi/appsettings.Development.json`

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=developer_evaluation;User Id=postgres;Password=SUA_SENHA_AQUI;Port=5432"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
```

**Docker (Alternativa):**

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5433;Database=sales_db;Username=sales_user;Password=sales_pass"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information", 
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
```

### Alternativa: PostgreSQL com Docker

Se preferir usar Docker ao invés do PostgreSQL local:

```sql
-- Use porta 5433 para evitar conflito com PostgreSQL local
docker run --name postgres-sales -e POSTGRES_DB=sales_db -e POSTGRES_USER=sales_user -e POSTGRES_PASSWORD=sales_pass -p 5433:5432 -d postgres:13
```

E ajuste a connection string para usar a porta 5433.

## 🏃‍♂️ Como Executar

### Modo Desenvolvimento
```bash
# Navegar para o projeto
cd src/Ambev.DeveloperEvaluation.WebApi

# Executar aplicação
dotnet run

# Ou com hot reload
dotnet watch run
```

### Modo Produção
```bash
# Build de produção
dotnet build --configuration Release

# Executar em produção
dotnet run --configuration Release
```

### Usando Docker
```bash
# Build da imagem
docker build -t sales-api .

# Executar container
docker run -p 5119:5119 --name sales-api-container sales-api
```

## 🔧 Testando a API

### 🎯 Swagger UI (Recomendado)

1. Acesse: **http://localhost:5119/swagger**
2. Explore todos os endpoints disponíveis
3. Teste diretamente pela interface

### 📝 Exemplo Prático

#### Criar uma Venda (POST /api/Sales)

```json
{
  "saleNumber": "SALE-2025-001234",
  "customerId": "CUST-789456", 
  "customerName": "Maria Silva Santos",
  "branchId": "BR-SP-001",
  "branchName": "Filial São Paulo Centro",
  "items": [
    {
      "productId": "PROD-001",
      "productName": "Notebook Dell Inspiron 15",
      "quantity": 2,
      "unitPrice": 2500.00
    },
    {
      "productId": "PROD-002",
      "productName": "Mouse Wireless Logitech", 
      "quantity": 5,
      "unitPrice": 85.50
    },
    {
      "productId": "PROD-003",
      "productName": "Teclado Mecânico RGB",
      "quantity": 15,
      "unitPrice": 299.99
    }
  ]
}
```

#### Resultado Esperado:
- **Item 1:** 2 unidades = R$ 5.000,00 (sem desconto)
- **Item 2:** 5 unidades = R$ 384,75 (10% desconto)  
- **Item 3:** 15 unidades = R$ 3.599,88 (20% desconto)
- **Total:** R$ 8.984,63

### 🔄 Fluxo de Teste Completo

```powershell
# 1. Criar venda (POST) - copie o ID retornado
Invoke-RestMethod -Uri "http://localhost:5119/api/Sales" `
  -Method POST -ContentType "application/json" `
  -Body '{ seu-json-aqui }'

# 2. Buscar venda específica (GET)
Invoke-RestMethod -Uri "http://localhost:5119/api/Sales/{id}" -Method GET

# 3. Listar todas as vendas (GET)
Invoke-RestMethod -Uri "http://localhost:5119/api/Sales?page=1&size=10" -Method GET

# 4. Atualizar venda (PUT)
Invoke-RestMethod -Uri "http://localhost:5119/api/Sales/{id}" `
  -Method PUT -ContentType "application/json" `
  -Body '{ json-atualizado }'

# 5. Cancelar venda (DELETE)
Invoke-RestMethod -Uri "http://localhost:5119/api/Sales/{id}" -Method DELETE
```

## 📁 Estrutura do Projeto

```
├── src/                                    # Código fonte
│   ├── Ambev.DeveloperEvaluation.WebApi/      # 🌐 API Controllers & Swagger
│   ├── Ambev.DeveloperEvaluation.Application/ # 📋 Use Cases & Handlers  
│   ├── Ambev.DeveloperEvaluation.Domain/      # 🏗️ Entidades & Regras de Negócio
│   ├── Ambev.DeveloperEvaluation.ORM/         # 🗄️ Acesso a Dados & Migrações
│   ├── Ambev.DeveloperEvaluation.IoC/         # 🔧 Injeção de Dependência
│   └── Ambev.DeveloperEvaluation.Common/      # 🛠️ Utilitários Compartilhados
├── tests/                                  # Testes
│   ├── Ambev.DeveloperEvaluation.Unit/        # 🧪 Testes Unitários
│   ├── Ambev.DeveloperEvaluation.Integration/ # 🔗 Testes de Integração
│   └── Ambev.DeveloperEvaluation.Functional/  # ⚡ Testes Funcionais
├── README.md                               # 📖 Documentação
├── LICENSE                                 # 📄 Licença (EVALUATION ONLY)
├── .gitignore                              # 🚫 Arquivos ignorados
└── Ambev.DeveloperEvaluation.sln          # 🎯 Solução .NET
```

### 🏗️ Clean Architecture

```
┌─────────────────────┐
│   🌐 WebApi Layer   │  ← Controllers, Middleware, Swagger
├─────────────────────┤
│ 📋 Application Layer │  ← Use Cases, Commands, Queries
├─────────────────────┤
│  🏗️ Domain Layer    │  ← Entities, Business Rules, Events
├─────────────────────┤
│ 🗄️ Infrastructure   │  ← Database, External Services
└─────────────────────┘
```

## 📐 Regras de Negócio

### 💰 Sistema de Descontos por Quantidade

| Quantidade de Itens | Desconto Aplicado | Status |
|---------------------|-------------------|---------|
| 1 - 3 itens        | **0%**            | ✅ Sem desconto |
| 4 - 9 itens        | **10%**           | ✅ Desconto aplicado |  
| 10 - 20 itens      | **20%**           | ✅ Desconto máximo |
| 21+ itens          | **❌ ERRO**        | 🚫 Não permitido |

### ✅ Validações Implementadas

- 🔢 **Número da venda** deve ser único
- 📦 **Quantidade mínima** de 1 item por produto
- 📦 **Quantidade máxima** de 20 itens por produto  
- 💵 **Preço unitário** deve ser maior que zero
- 👤 **Dados do cliente** são obrigatórios
- 🏢 **Dados da filial** são obrigatórios
- 🛡️ **Validação completa** de entrada de dados

## 🌐 API Endpoints

### 📊 Sales Controller

| Método | Endpoint | Descrição | Retorno |
|--------|----------|-----------|---------|
| **POST** | `/api/Sales` | ➕ Criar nova venda | 201 Created |
| **GET** | `/api/Sales` | 📋 Listar vendas (paginado) | 200 OK |
| **GET** | `/api/Sales/{id}` | 🔍 Buscar venda específica | 200 OK / 404 |
| **PUT** | `/api/Sales/{id}` | ✏️ Atualizar venda existente | 200 OK / 404 |
| **DELETE** | `/api/Sales/{id}` | 🗑️ Cancelar venda | 204 No Content / 404 |

### 📈 Parâmetros de Consulta (GET /api/Sales)

```bash
# Paginação
GET /api/Sales?page=1&size=10

# Exemplo de resposta
{
  "data": {
    "data": [...],        # Lista de vendas
    "totalItems": 50,     # Total de itens
    "currentPage": 1,     # Página atual
    "totalPages": 5,      # Total de páginas
    "hasNextPage": true,  # Tem próxima página
    "hasPreviousPage": false  # Tem página anterior
  },
  "success": true,
  "message": "Sales retrieved successfully"
}
```

### 📋 Códigos de Status HTTP

- **200 OK** - Operação realizada com sucesso
- **201 Created** - Recurso criado com sucesso  
- **204 No Content** - Operação realizada sem conteúdo de retorno
- **400 Bad Request** - Erro de validação nos dados enviados
- **404 Not Found** - Recurso não encontrado
- **500 Internal Server Error** - Erro interno do servidor

## 🧪 Testes

### Executar Todos os Testes
```bash
dotnet test
```

### Executar por Categoria
```bash
# Testes unitários (41 testes)
dotnet test tests/Ambev.DeveloperEvaluation.Unit

# Testes de integração  
dotnet test tests/Ambev.DeveloperEvaluation.Integration

# Testes funcionais
dotnet test tests/Ambev.DeveloperEvaluation.Functional
```

### Relatório de Cobertura
```bash
# Executar com cobertura de código
dotnet test --collect:"XPlat Code Coverage"

# Instalar ferramenta de relatório (uma vez)
dotnet tool install -g dotnet-reportgenerator-globaltool

# Gerar relatório HTML
reportgenerator `
  -reports:**/coverage.cobertura.xml `
  -targetdir:coverage-report `
  -reporttypes:Html

# Abrir relatório
start coverage-report/index.html
```

### 📊 Métricas dos Testes

- **✅ 41 testes unitários** - 100% passando
- **🎯 Cobertura de código** - Entidades, validações e handlers
- **⚡ Execução rápida** - Testes isolados e eficientes
- **🔧 Testes de integração** - Validação end-to-end

## 🚀 Deploy e Produção

### Docker Compose Completo

Crie um arquivo `docker-compose.yml`:

```yaml
version: '3.8'

services:
  # API
  api:
    build: .
    ports:
      - "5119:80"
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
      - ConnectionStrings__DefaultConnection=Host=db;Database=sales_db;Username=postgres;Password=SalesPass123!
    depends_on:
      db:
        condition: service_healthy
    restart: unless-stopped

  # Banco de Dados
  db:
    image: postgres:13-alpine
    environment:
      POSTGRES_DB: sales_db
      POSTGRES_PASSWORD: SalesPass123!
    ports:
      - "5432:5432"  
    volumes:
      - postgres_data:/var/lib/postgresql/data
    healthcheck:
      test: ["CMD-SHELL", "pg_isready -U postgres"]
      interval: 30s
      timeout: 10s
      retries: 3
    restart: unless-stopped

volumes:
  postgres_data:
    driver: local
```

### Executar com Docker Compose
```bash
# Iniciar todos os serviços
docker-compose up -d

# Ver logs
docker-compose logs -f api

# Parar serviços  
docker-compose down

# Parar e remover volumes
docker-compose down -v
```

## 🔧 Troubleshooting

### Problemas Comuns

#### ❌ Erro de Conexão com Banco
```bash
# Verificar se PostgreSQL está rodando
docker ps | findstr postgres

# Verificar logs do container
docker logs postgres-sales
```

#### ❌ Porta 5119 em Uso
```bash
# Verificar o que está usando a porta
netstat -ano | findstr :5119

# Ou mudar a porta no launchSettings.json
```

#### ❌ Erro de Migração
```bash
# Resetar banco (CUIDADO: apaga dados)
dotnet ef database drop --force --project src/Ambev.DeveloperEvaluation.ORM
dotnet ef database update --project src/Ambev.DeveloperEvaluation.ORM
```

### 📞 Suporte

- **🐛 Issues:** [GitHub Issues](../../issues)
- **📚 Documentação:** [Swagger UI](http://localhost:5119/swagger)
- **🏗️ Arquitetura:** Clean Architecture + DDD + CQRS

---

## 📄 Licença

Este projeto está licenciado sob uma **LICENÇA RESTRITIVA** - veja o arquivo [LICENSE](LICENSE) para detalhes completos.

### 🔒 Resumo das Restrições:
- ✅ **Permitido:** Download, execução e teste local para avaliação
- ❌ **Proibido:** Uso comercial, modificação, distribuição, reutilização
- 🚫 **Não use este código em projetos comerciais ou de trabalho**
- 📧 **Contato:** Para outros usos, entre em contato com o autor

---

**💼 Projeto desenvolvido exclusivamente para demonstração de habilidades técnicas em avaliação**

**⚠️ USO COMERCIAL PROIBIDO - APENAS PARA AVALIAÇÃO E TESTE**
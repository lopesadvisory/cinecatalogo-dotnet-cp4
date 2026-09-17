# CineCatálogo API

API RESTful para gerenciamento de um catálogo de filmes e diretores, desenvolvida em **ASP.NET Core / .NET 8** seguindo os princípios de **Clean Architecture**.

Projeto desenvolvido para o **Checkpoint 4 — Advanced Business Development with .NET (2026)**, cujo foco é arquitetura em camadas, otimização de performance, resiliência, testes automatizados e observabilidade.

🔗 **Demo publicada (Render):** https://cinecatalogo-api.onrender.com/swagger — instância gratuita, pode levar ~30-50s para responder no primeiro acesso após um período de inatividade (cold start). As instruções para rodar localmente (necessário para demonstrar a execução dos testes automatizados) estão abaixo.

## Integrantes

| Nome | RM |
|---|---|
| Hebert Lopes do Santos | 563192 |
| Marcus Vinícius Vila Nova da Silva | 558771 |
| Nicolas Monteiro Ramiro | 562380 |

## Sumário

- [Descrição do projeto](#descrição-do-projeto)
- [Arquitetura e componentes utilizados](#arquitetura-e-componentes-utilizados)
- [Como rodar o projeto](#como-rodar-o-projeto)
- [Como executar os testes](#como-executar-os-testes)
- [Configurações necessárias](#configurações-necessárias)
- [Endpoints disponíveis](#endpoints-disponíveis)
- [Observabilidade](#observabilidade)

## Descrição do projeto

O **CineCatálogo** é uma API para cadastro e consulta de **filmes** e **diretores**. Cada filme pertence a um diretor (relação 1:N), e a API expõe operações completas de CRUD para as duas entidades, além de:

- Listagem paginada de filmes e diretores, com filtros por gênero e por diretor;
- Validações de negócio (ex.: não permitir dois diretores com o mesmo nome, não permitir remover um diretor que ainda possui filmes cadastrados);
- Compressão de resposta (gzip/brotli) e limitação de requisições (rate limiting);
- Testes automatizados de unidade e de integração;
- Health checks, logging estruturado e integração com Application Insights.

O banco de dados utilizado é **SQLite**, escolhido para que o projeto rode localmente sem necessidade de nenhuma infraestrutura externa (facilita a correção e a apresentação). A base é criada e populada automaticamente (seed) na primeira execução.

## Arquitetura e componentes utilizados

O projeto segue o padrão de **Clean Architecture**, dividido em 4 camadas (uma por projeto .csproj), mais os projetos de teste:

```
CineCatalogo.sln
├── src/
│   ├── CineCatalogo.Domain          -> Entidades, interfaces de repositório, exceções de domínio
│   ├── CineCatalogo.Application     -> DTOs, mapeamentos, interfaces e implementações de serviços (regras de negócio)
│   ├── CineCatalogo.Infrastructure  -> EF Core (DbContext, configurações, migrations), repositórios, health check de banco
│   └── CineCatalogo.Api             -> Controllers, Program.cs (composição/DI), middlewares, Swagger
└── tests/
    ├── CineCatalogo.Tests.Unit         -> Testes de unidade (serviços e mapeamentos), com Moq
    └── CineCatalogo.Tests.Integration  -> Testes de integração (WebApplicationFactory), ponta a ponta via HTTP
```

**Regra de dependência:** `Api` → `Infrastructure`/`Application` → `Domain`. O `Domain` não depende de nenhuma outra camada.

### Principais componentes/pacotes utilizados

| Componente | Pacote / Tecnologia |
|---|---|
| ORM / Banco de dados | Entity Framework Core 8 + SQLite |
| Repository Pattern | Interfaces no `Domain`, implementações no `Infrastructure` |
| DTOs e mapeamento | Records/classes em `Application.Dtos` + métodos de extensão em `Application.Mappings` |
| Paginação | `PaginationParams` / `PagedResult<T>` (Domain) e `PagedResultDto<T>` (Application) |
| Compressão de resposta | `Microsoft.AspNetCore.ResponseCompression` (Gzip + Brotli) |
| Rate Limiting | `Microsoft.AspNetCore.RateLimiting` (Fixed Window, por IP) |
| Documentação | Swashbuckle (Swagger) + `Swashbuckle.AspNetCore.Annotations` |
| Logging estruturado | Serilog (console + arquivo) |
| Health Checks | `Microsoft.Extensions.Diagnostics.HealthChecks` + verificação customizada de conexão com o banco |
| Tracing/Métricas | `Microsoft.ApplicationInsights.AspNetCore` |
| Testes de unidade | xUnit + Moq |
| Testes de integração | xUnit + `Microsoft.AspNetCore.Mvc.Testing` (WebApplicationFactory) |

## Como rodar o projeto

Pré-requisitos: **.NET 8 SDK**.

```bash
git clone <url-deste-repositorio>
cd cinecatalogo-dotnet-cp4
dotnet restore
dotnet build
dotnet run --project src/CineCatalogo.Api
```

Ao iniciar, a API:

1. Aplica automaticamente as migrations do Entity Framework Core (cria o arquivo `cinecatalogo.db` na pasta do projeto `CineCatalogo.Api`);
2. Popula o banco com diretores e filmes de exemplo, caso ainda esteja vazio (seed).

A API sobe, por padrão, em `https://localhost:7xxx` e `http://localhost:5xxx` (portas definidas em `src/CineCatalogo.Api/Properties/launchSettings.json`). Em ambiente de **Development**, o Swagger fica disponível em:

```
/swagger
```

## Como executar os testes

Para rodar toda a suíte (testes de unidade + integração):

```bash
dotnet test
```

Para rodar apenas um dos projetos de teste:

```bash
dotnet test tests/CineCatalogo.Tests.Unit
dotnet test tests/CineCatalogo.Tests.Integration
```

- Os **testes de unidade** cobrem as regras de negócio dos serviços (`DiretorService`, `FilmeService`) usando mocks dos repositórios, e os mapeamentos entre entidades e DTOs.
- Os **testes de integração** sobem a API inteira em memória (via `WebApplicationFactory`, com SQLite em memória) e testam os fluxos reais de CRUD, paginação, filtros e rate limiting através de requisições HTTP.

## Configurações necessárias

Todas as configurações ficam em `src/CineCatalogo.Api/appsettings.json` (e podem ser sobrescritas por variáveis de ambiente, como de costume em ASP.NET Core).

### Banco de dados

```json
"ConnectionStrings": {
  "DefaultConnection": "Data Source=cinecatalogo.db"
}
```

Por padrão usa um arquivo SQLite local. Não é necessário instalar nenhum servidor de banco de dados.

### Rate Limiting

```json
"RateLimiting": {
  "PermitLimit": 30,
  "WindowSeconds": 10,
  "QueueLimit": 0
}
```

Limite de 30 requisições a cada 10 segundos por endereço IP (janela fixa). Requisições excedentes recebem `429 Too Many Requests`.

### Application Insights

```json
"ApplicationInsights": {
  "ConnectionString": ""
}
```

Para habilitar o envio real de telemetria (traces, dependências, métricas), configure a connection string de um recurso do Application Insights no Azure:

```json
"ApplicationInsights": {
  "ConnectionString": "InstrumentationKey=xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx;IngestionEndpoint=https://..."
}
```

Também é possível definir via variável de ambiente `APPLICATIONINSIGHTS_CONNECTION_STRING`. Sem essa configuração, o SDK do Application Insights continua registrado e instrumentando a aplicação (a integração e os middlewares de tracing/telemetria funcionam normalmente), apenas não há envio de dados para um recurso no Azure.

## Endpoints disponíveis

Documentação interativa completa (com exemplos de request/response) disponível no Swagger em `/swagger` (ambiente de Development).

### Diretores

| Método | Rota | Descrição |
|---|---|---|
| GET | `/api/diretores?pageNumber=1&pageSize=10` | Lista diretores de forma paginada |
| GET | `/api/diretores/{id}` | Busca um diretor pelo id |
| POST | `/api/diretores` | Cadastra um novo diretor |
| PUT | `/api/diretores/{id}` | Atualiza um diretor existente |
| DELETE | `/api/diretores/{id}` | Remove um diretor (falha com `409` se ele possuir filmes) |

Exemplo de requisição (`POST /api/diretores`):

```json
{
  "nome": "Christopher Nolan",
  "nacionalidade": "Britânico",
  "dataNascimento": "1970-07-30"
}
```

### Filmes

| Método | Rota | Descrição |
|---|---|---|
| GET | `/api/filmes?pageNumber=1&pageSize=10&genero=Drama&diretorId=1` | Lista filmes de forma paginada, com filtros opcionais |
| GET | `/api/filmes/{id}` | Busca um filme pelo id |
| POST | `/api/filmes` | Cadastra um novo filme |
| PUT | `/api/filmes/{id}` | Atualiza um filme existente |
| DELETE | `/api/filmes/{id}` | Remove um filme |

Exemplo de requisição (`POST /api/filmes`):

```json
{
  "titulo": "A Origem",
  "genero": "Ficção Científica",
  "anoLancamento": 2010,
  "duracaoMinutos": 148,
  "sinopse": "Um ladrão que invade sonhos recebe a missão de plantar uma ideia na mente de um executivo.",
  "notaMedia": 8.8,
  "diretorId": 1
}
```

Exemplo de resposta paginada (`GET /api/filmes?pageSize=2`):

```json
{
  "items": [ { "id": 1, "titulo": "A Origem", "genero": "Ficção Científica", "...": "..." } ],
  "pageNumber": 1,
  "pageSize": 2,
  "totalCount": 4,
  "totalPages": 2,
  "hasPreviousPage": false,
  "hasNextPage": true
}
```

### Health Check

| Método | Rota | Descrição |
|---|---|---|
| GET | `/health` | Retorna o status da API e de suas dependências (banco de dados) |

## Observabilidade

- **Logging estruturado:** Serilog grava logs no console e em arquivo (`logs/log-*.txt`), incluindo o log de todas as requisições HTTP (método, rota, status, tempo de resposta) e o tratamento centralizado de exceções (`ExceptionHandlingMiddleware`), que também gera `application/problem+json` para o cliente.
- **Health Checks:** endpoint `/health` verifica a conectividade com o banco de dados e retorna um JSON detalhado com o status de cada verificação.
- **Tracing e métricas:** `Microsoft.ApplicationInsights.AspNetCore` instrumenta automaticamente requisições HTTP, dependências (chamadas ao banco) e exceções, prontos para envio ao Azure Application Insights assim que a connection string for configurada.

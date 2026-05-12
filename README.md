# Simulador de Micro-ondas Digital

Simulador de micro-ondas desenvolvido para avaliação técnica, focando na aplicação rigorosa de padrões de projeto, SOLID e Clean Code.

## Tecnologias e Frameworks
- **Linguagem:** C# (.NET 10.0)
- **Interface:** Console Application
- **Persistência:** Entity Framework Core (SQL Server via Docker)
- **Testes:** xUnit, Moq, FluentAssertions, Bogus
- **Padrões:** Orientação a Objetos, SOLID, TDD, Clean Code

## Decisões Arquiteturais e Padrões (Nível 1)

Durante o Nível 1, algumas decisões arquiteturais chaves foram tomadas para priorizar o design de software limpo:

1. **Aplicação Console:** A fim de focar estritamente nas regras de negócio (Backend) sem ferir a separação de responsabilidades criando acoplamentos prematuros de UI Web ou Web APIs (pois são requisitos exclusivos do Nível 4), a interface visual foi construída via Terminal interativo. 
2. **SOLID e Orientação a Objetos:** A classe `MaquinaMicroondas` foi desenhada como uma Entidade rica de Domínio. Utilizamos `private set` para blindar o estado da máquina. Toda mutação ocorre através de métodos de negócio que protegem as invariantes do processo de aquecimento.
3. **Clean Code:** O código flui linearmente usando as técnicas de *Early Return* e *Guard Clauses*, melhorando drasticamente a legibilidade.
4. **Constantes e Padrões Comportamentais:** O código injeta constantes via uma classe estática semântica (`ValoresPadrao`). Lógicas não sujam o fluxo principal e estão devidamente abstraídas.
5. **Padrão Builder (Testes):** Os testes em xUnit utilizam o design pattern *Test Data Builder* com auxílio do `Bogus`, herdando de um `BaseBuilder<T>` para geração padronizada de entidades.

## Decisões Arquiteturais e Padrões (Nível 2)

Durante o Nível 2 (Programas Pré-definidos), as seguintes abordagens foram aplicadas para suportar a complexidade e garantir a extensibilidade:

1. **Padrão Repository:** Para manter um código limpo e preparar o terreno para a persistência em banco de dados, o acesso aos programas ocorre de forma abstraída através da interface `IProgramaRepository`.
2. **Desacoplamento e Separação da UI:** O monolítico inicial da UI foi fracionado. A apresentação visual agora está concentrada na classe `MicroondasVisor`, enquanto o roteamento de escolhas via teclado e inputs são domínios da classe `MicroondasMenu`.
3. **Eliminação de Magic Numbers:** A classe `ValoresPadrao` foi estendida para centralizar rigorosamente tempos e potências de todos os 5 programas de nível 2, deixando a parametrização visível num único local do sistema.
4. **Correção de Bug (Cancelamento de Fluxo Manual):** Foi implementada uma solução robusta onde, ao deixar um valor vazio ou digitar "0" nas entradas do console, o sistema aborta o fluxo manual e limpa a máquina, resolvendo a inconsistência de o usuário ficar "preso".

## Decisões Arquiteturais e Padrões (Nível 3)

A evolução para o Nível 3 consolidou a estrutura backend para um cenário real de mercado:

1. **Persistência com EF Core e Docker:** Implementação de um banco de dados relacional SQL Server containerizado via Docker, mantendo o ambiente de desenvolvimento limpo e previsível.
2. **Refatoração do Domínio (Composição sobre Herança):** O modelo abstrato baseado em herança foi substituído por uma única entidade rica `ProgramaAquecimento` com a propriedade `EhPadrao`. Isso evita complexidade desnecessária e permite persistir todos os programas em uma única tabela fluida.
3. **Arquitetura Totalmente Assíncrona:** A aplicação inteira foi reescrita utilizando o padrão `async/await`, desde o repositório até o loop principal do Console UI (`MicroondasInterface`), simulando o comportamento não-bloqueante de Web APIs modernas.
4. **Data Seeding Controlado:** A inserção dos 5 programas originais foi garantida via *Migrations Seeding*, evitando a necessidade de scripts manuais e mantendo a integridade dos dados protegidos do sistema.
5. **Aprimoramento de UX e Validação Imediata:** A experiência do console foi enriquecida com validação imediata campo-a-campo no momento da criação de um programa customizado, além de exibir os programas personalizados dinamicamente em formato *itálico*.

---

## Como Instalar e Usar

### Pré-requisitos
- [.NET 10.0 SDK](https://dotnet.microsoft.com/) ou superior.
- [Docker Desktop](https://www.docker.com/products/docker-desktop/) (necessário para o banco de dados).

### Executando a Aplicação
No diretório raiz do projeto, suba o contêiner do banco de dados e inicie a aplicação:

```powershell
# 1. Subir a infraestrutura de Banco de Dados
docker-compose up -d

# 2. Executar a simulação (as migrations e seed ocorrem de forma automática)
dotnet run --project Microondas.Console
```
*O menu interativo guiará as opções de configuração e as simulações em tempo real.*

### Rodando a Suíte de Testes
Para atestar a integridade de todas as regras de domínio, comportamentos e builders (100% dos testes rodando em `FluentAssertions`), execute:
```powershell
dotnet test
```

>  This is a challenge by [Coodesh](https://coodesh.com/)

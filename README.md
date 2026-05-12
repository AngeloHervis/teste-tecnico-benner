# Simulador de Micro-ondas Digital

Simulador de micro-ondas desenvolvido para avaliação técnica, focando na aplicação rigorosa de padrões de projeto, SOLID e Clean Code.

## Tecnologias e Frameworks
- Linguagem: C# (.NET 10.0)
- Interface: Console Application
- Testes: xUnit, Moq, FluentAssertions, Bogus
- Padrões: Orientação a Objetos, SOLID, TDD, Clean Code

## Decisões Arquiteturais e Padrões (Nível 1)

Durante o Nível 1, algumas decisões arquiteturais chaves foram tomadas para priorizar o design de software limpo:

1. **Aplicação Console:** A fim de focar estritamente nas regras de negócio (Backend) sem ferir a separação de responsabilidades criando acoplamentos prematuros de UI Web ou Web APIs (pois são requisitos exclusivos do Nível 4), a interface visual foi construída via Terminal interativo. 
2. **SOLID e Orientação a Objetos:** A classe `MaquinaMicroondas` foi desenhada como uma Entidade rica de Domínio. Utilizamos `private set` para blindar o estado da máquina. Toda mutação ocorre através de métodos de negócio que protegem as invariantes do processo de aquecimento.
3. **Clean Code:** O código flui linearmente usando as técnicas de *Early Return* e *Guard Clauses*, melhorando drasticamente a legibilidade.
4. **Constantes e Padrões Comportamentais:** O código injeta constantes via uma classe estática semântica (`ValoresPadrao`). Lógicas não sujam o fluxo principal e estão devidamente abstraídas.
5. **Padrão Builder (Testes):** Os testes em xUnit utilizam o design pattern *Test Data Builder* com auxílio do `Bogus`, herdando de um `BaseBuilder<T>`.

## Como Instalar e Usar

### Pré-requisitos
- [.NET 10.0 SDK](https://dotnet.microsoft.com/) ou superior.

### Executando a Aplicação
No diretório raiz do projeto, abra o seu terminal e execute:
```bash
dotnet run --project Microondas.Console
```
*O menu interativo guiará as opções de configuração e as simulações em tempo real.*

### Rodando a Suíte de Testes
Para atestar a integridade das regras do Nível 1, execute:
```bash
dotnet test
```

>  This is a challenge by [Coodesh](https://coodesh.com/)

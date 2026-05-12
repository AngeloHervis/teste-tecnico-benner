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

## Decisões Arquiteturais e Padrões (Nível 2)

Durante o Nível 2 (Programas Pré-definidos), as seguintes abordagens foram aplicadas para suportar a complexidade e garantir a extensibilidade:

1. **Princípio Aberto/Fechado (SOLID):** A arquitetura foi expandida usando herança sobre a classe abstrata `ProgramaAquecimento`. Todos os programas garantem imutabilidade (exigência da regra de negócio) e isso permite a adição infinita de novos alimentos sem a necessidade de modificar o núcleo da Máquina de Micro-ondas.
2. **Padrão Repository:** Para manter um código limpo e já preparar terreno para o Nível 3 (Persistência EM SQL Server), o acesso à lista dos 5 programas pré-definidos ocorre limpidamente através da abstração `IProgramaRepository`.
3. **Desacoplamento e Separação da UI:** O monolítico inicial da UI foi fracionado. A apresentação visual agora está concentrada na classe `MicroondasVisor`, enquanto o roteamento de escolhas via teclado e inputs são domínios exclusivos da classe `MicroondasMenu`.
4. **Eliminação de Magic Numbers:** A classe `ValoresPadrao` foi estendida para centralizar rigorosamente os tempos (em segundos) e as potências pré-definidas de todos os 5 programas de nível 2, deixando a parametrização visível num único local do sistema.
5. **Correção de Bug (Cancelamento de Fluxo Manual):** Foi identificado e corrigido um bug onde o usuário ficava "preso" nas perguntas de entrada de Tempo e Potência após escolher a opção manual, não tendo nenhum botão designado para cancelar a ação. A solução implementada permitiu que ao deixar o valor vazio ou digitar "0", o sistema aborta o fluxo e limpa a máquina (equivalente direto à tecla Pausar/Cancelar), resolvendo a inconsistência.

## Como Instalar e Usar

### Pré-requisitos
- [.NET 10.0 SDK](https://dotnet.microsoft.com/) ou superior.

### Executando a Aplicação
No diretório raiz do projeto, abra o seu terminal e execute:
```bash
cd Microondas.Console
dotnet run
```
*O menu interativo guiará as opções de configuração e as simulações em tempo real.*

### Rodando a Suíte de Testes
Para atestar a integridade das regras do Nível 1 e Nível 2, execute:
```bash
dotnet test
```

>  This is a challenge by [Coodesh](https://coodesh.com/)

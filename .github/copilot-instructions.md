# Diretrizes para o GitHub Copilot

Este documento define as diretrizes e melhores práticas a serem seguidas ao gerar ou modificar código neste projeto. O objetivo é garantir um código limpo, eficiente, seguro e de fácil manutenção.

## 1. Princípios Gerais

- **SOLID**: Siga rigorosamente os cinco princípios SOLID.
  - **S**ingle Responsibility Principle (Princípio da Responsabilidade Única): Cada classe ou método deve ter uma única responsabilidade.
  - **O**pen/Closed Principle (Princípio Aberto/Fechado): As entidades de software devem ser abertas para extensão, mas fechadas para modificação.
  - **L**iskov Substitution Principle (Princípio da Substituição de Liskov): Objetos de uma superclasse devem ser substituíveis por objetos de uma subclasse sem afetar a correção do programa.
  - **I**nterface Segregation Principle (Princípio da Segregação de Interface): Crie interfaces pequenas e específicas para o cliente, em vez de interfaces grandes e genéricas.
  - **D**ependency Inversion Principle (Princípio da Inversão de Dependência): Dependa de abstrações, não de implementações. Use Injeção de Dependência (DI).
- **DRY (Don't Repeat Yourself)**: Evite a duplicação de código. Abstraia e reutilize a lógica comum.
- **KISS (Keep It Simple, Stupid)**: Prefira soluções simples e diretas em vez de complexas.
- **Clean Code**: Escreva um código legível e autoexplicativo. Use nomes significativos para variáveis, métodos e classes. Comente apenas o "porquê", não o "o quê".

## 2. C# e .NET (.NET 8+)

- **Recursos Modernos da Linguagem**:
  - Use `file-scoped namespaces`.
  - Use `records` para tipos de dados imutáveis, especialmente para DTOs.
  - Use `primary constructors` quando simplificarem a definição da classe.
  - Utilize `using` declarations em vez de blocos `using`.
  - Prefira `switch expressions` em vez de `switch statements` quando apropriado.
  - Use `pattern matching` para simplificar condicionais complexas.
- **Programação Assíncrona**:
  - Use `async`/`await` para todas as operações de I/O (rede, banco de dados, sistema de arquivos).
  - Evite `async void`, exceto para manipuladores de eventos. Prefira `async Task`.
  - Não use `.Result` ou `.Wait()` em código assíncrono para evitar deadlocks.
- **Nullability**:
  - Habilite `nullable reference types` (`<Nullable>enable</Nullable>`) em todos os projetos.
  - Resolva todos os avisos de nulidade para evitar `NullReferenceException` em tempo de execução.
- **Injeção de Dependência (DI)**:
  - Registre os serviços no contêiner de DI com o tempo de vida apropriado (`Singleton`, `Scoped`, `Transient`).
  - Injete dependências através do construtor.
- **JSON**: Use `System.Text.Json` por padrão. Recorra ao `Newtonsoft.Json` apenas se um recurso específico não estiver disponível no `System.Text.Json`.

## 3. ASP.NET Core (APIs)

- **Minimal APIs**: Considere o uso de Minimal APIs para endpoints simples e focados. Para APIs mais complexas, continue usando o padrão com Controllers.
- **RESTful**: Projete APIs seguindo os princípios REST.
  - Use os verbos HTTP corretamente: `GET` (recuperar), `POST` (criar), `PUT` (substituir), `PATCH` (atualizar parcialmente), `DELETE` (remover).
  - Use os códigos de status HTTP apropriados (200, 201, 204, 400, 404, 500, etc.).
- **DTOs (Data Transfer Objects)**:
  - Nunca exponha entidades de domínio (modelos do EF Core) diretamente nas APIs.
  - Use DTOs para modelar as entradas (requests) e saídas (responses) da API. Use `records` para DTOs imutáveis.
- **Validação**: Use `FluentValidation` ou `Data Annotations` para validar os DTOs de entrada.

## 4. Entity Framework Core

- **Configuração**: Prefira a **Fluent API** em vez de `Data Annotations` para configurar o modelo de dados no método `OnModelCreating`. Isso mantém a configuração centralizada e as classes de entidade limpas.
- **Consultas**:
  - Use os métodos assíncronos (`SaveChangesAsync`, `ToListAsync`, `FirstOrDefaultAsync`, etc.).
  - Para consultas de apenas leitura, use `AsNoTracking()` para melhorar o desempenho.
  - Evite o problema de consulta N+1 usando `Include()` e `ThenInclude()` para carregar dados relacionados de forma eager.
- **Migrações**: Use migrações do EF Core para gerenciar e versionar o esquema do banco de dados.

## 5. Testes

- **Framework**: Use **xUnit** como o framework de teste padrão.
- **Mocking**: Use **NSubstitute** ou **Moq** para criar mocks de dependências.
- **Padrão AAA**: Estruture os testes seguindo o padrão **Arrange, Act, Assert**.
  - **Arrange**: Configure o ambiente de teste (crie objetos, mocks, etc.).
  - **Act**: Execute o método que está sendo testado.
  - **Assert**: Verifique se o resultado é o esperado.
- **Cobertura**: Escreva testes unitários para toda a lógica de negócios.

## 6. Estilo de Código e Formatação

- **Convenções da Microsoft**: Siga as [convenções de codificação C# da Microsoft](https://docs.microsoft.com/pt-br/dotnet/csharp/fundamentals/coding-style/coding-conventions).
- **`var`**: Use `var` quando o tipo da variável for óbvio a partir do lado direito da atribuição. Caso contrário, use o nome explícito do tipo para melhorar a clareza.
- **Campos Privados**: Use um prefixo de sublinhado (`_`) para nomes de campos privados (`_privateField`).
- **Formatação**: Mantenha a formatação do código consistente com o restante do projeto.
# Doe Sangue

## Estrutura do Projeto

Neste projeto usamos Clean Architecture para modularizar o sistema a fim de facilitar a manutenção.

Abaixo teremos a explicação de cada camada e suas responsabilidades.

### Domain
Na camada de domnínio temos as Entidades da aplicação e seus comportamentos, cada entidade possui contratos para interagir com o sistema, como por exemplo, as interfaces de repositório.

### Application
Na camada de aplicação, temos os casos de uso, serviços e tudo que se refira ao comportamento da aplicação.

### Infrastructure
Na camada de infraestrutura, temos todas as integrações que não fazem parte do núcleo da aplicação, como por exemplo: banco de dados, mensageria, envio de email/SMS.

### API
Na camada de API, será feito o controle de request e response, é o ponto de entrada e saída da aplicação, também conhecido como Presentation.
Podemos ter as controllers, exception filters etc.

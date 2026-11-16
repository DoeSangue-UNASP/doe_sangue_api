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

## Funcionalidades da API

Os cadastros de doador e hemocentro e o login são públicos. Os demais fluxos abaixo exigem JWT Bearer e autorização por perfil.

### Doador

- `GET /api/Doador/me`: consultar o próprio perfil.
- `PUT /api/Doador/me`: atualizar o próprio perfil.
- `DELETE /api/Doador/me`: excluir a própria conta.
- `GET /api/Agendamento/me`: consultar os próprios agendamentos.
- `POST /api/Agendamento`: criar agendamento para o usuário autenticado.
- `PATCH /api/Agendamento/{id}/cancelar`: cancelar um agendamento próprio.

### Hemocentro

- `GET /api/Hemocentro/me`: consultar o próprio perfil.
- `GET /api/Doador` e `GET /api/Doador/{id}`: consultar doadores que tenham agendamento vinculado ao próprio hemocentro.
- `GET /api/Agendamento/hemocentro`: consultar a agenda do próprio hemocentro; aceita os filtros opcionais `data` (`yyyy-MM-dd`) e `documento` (CPF).
- `PATCH /api/Agendamento/{id}/cancelar` e `PATCH /api/Agendamento/{id}/concluir`: gerir os próprios agendamentos.
- `POST /api/BolsaSangue`: cadastrar bolsa somente para agendamento concluído do próprio hemocentro.
- `GET /api/BolsaSangue/hemocentro`: consultar bolsas e resumo do estoque por status.
- `GET /api/BolsaSangue/{id}`: consultar uma bolsa do próprio hemocentro.
- `PATCH /api/BolsaSangue/{id}/status`: atualizar o status informando `EM_ANALISE`, `DISPONIVEL`, `VENCIDA` ou `DESCARTADA`; as transições são validadas pelo domínio.

O agendamento deve ocorrer dentro do horário de funcionamento do hemocentro. Como o modelo ainda não define duração ou capacidade por horário, a regra atual impede duplicidade de agendamentos ativos no mesmo instante exato para o mesmo hemocentro.

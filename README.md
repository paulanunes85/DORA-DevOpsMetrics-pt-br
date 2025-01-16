# Métricas DevOps

[![Build](https://GitHub.com/samsmithnz/DevOpsMetrics/workflows/CI/CD/badge.svg)](https://GitHub.com/samsmithnz/DevOpsMetrics/actions?query=workflow%3ACI%2FCD)
[![Coverage Status](https://coveralls.io/repos/github/samsmithnz/DevOpsMetrics/badge.svg?branch=main)](https://coveralls.io/github/samsmithnz/DevOpsMetrics?branch=main)
[![Code Smells](https://sonarcloud.io/api/project_badges/measure?project=samsmithnz_DevOpsMetrics&metric=code_smells)](https://sonarcloud.io/summary/new_code?id=samsmithnz_DevOpsMetrics)
[![Current Release](https://img.shields.io/github/release/samsmithnz/DevOpsMetrics/all.svg)](https://github.com/samsmithnz/DevOpsMetrics/releases)

**Por que devemos nos importar com as métricas DevOps e o que são elas?** Toda engenharia, incluindo software, precisa de métricas para acompanhar o desempenho, mas muitas métricas, quando medidas individualmente, podem ser 'manipuladas' ou não incentivam os comportamentos ou incentivos corretos. Isso tem sido um problema com métricas por muitos anos. As [métricas DORA](https://services.google.com/fh/files/misc/state-of-devops-2019.pdf) são um passo na direção certa, combinando várias métricas que incentivam os comportamentos e incentivos - e, portanto, incentivam as equipes DevOps a terem um alto nível de desempenho. As métricas DORA não são perfeitas, mas ainda são as melhores disponíveis hoje.
- Um [site de demonstração exibindo essas métricas pode ser visualizado aqui](https://devops-prod-eu-web.azurewebsites.net/).
- Insights que observei sobre a implementação das métricas DevOps DORA podem ser encontrados em um [post de blog aqui](https://samlearnsazure.blog/2020/04/30/high-performing-devops-metrics/)

Este projeto é focado em ajudá-lo a coletar e analisar quatro métricas chave de alto desempenho DevOps do GitHub e Azure DevOps. A [pesquisa "State of DevOps" da DORA](https://cloud.google.com/blog/products/devops-sre/announcing-dora-2021-accelerate-state-of-devops-report) e [Accelerate](https://www.amazon.com/Accelerate-Software-Performing-Technology-Organizations/dp/1942788339) destacaram quatro indicadores principais de equipes DevOps de alto desempenho. Embora essas quatro métricas sejam amplamente usadas na discussão DevOps, é desafiador implementar e capturar todas as métricas.

- **Frequência de implantação: Número de implantações em produção.** Isso é importante, pois destaca com que frequência você pode implantar em produção - o que, por sua vez, indica que há um teste automatizado maduro e um pipeline CI/CD maduro para liberar para produção.
- **Tempo de lead para mudanças: Tempo desde a confirmação de uma mudança até a implantação em produção.** Quão rapidamente podemos mudar uma linha de código e tê-la rodando em produção? Novamente, isso indica testes automatizados maduros e um pipeline CI/CD maduro capaz de lidar com mudanças.
- **Tempo médio para restaurar (MTTR): Quão rapidamente ocorre a restauração da produção em uma interrupção ou degradação.** Quando há uma degradação, quão rapidamente o sistema pode se auto-recuperar, escalar para lidar com o aumento da carga e/ou Isso é controverso, pois é desafiador comparar diferentes eventos que causam degradação.
- **Taxa de falha de mudança: Após uma implantação em produção, foi bem-sucedida? Ou foi necessário um conserto ou rollback após o fato?** Com que frequência uma mudança que fizemos foi 'bem-sucedida'? Isso se encaixa bem com a frequência de implantação e o tempo de lead para mudanças, mas é desafiador de medir - pois requer uma aprovação de sucesso. Não apenas que o código foi implantado corretamente, mas que não houve efeitos adversos ou degradação da implantação no sistema.

![Métricas de alto desempenho](https://user-images.githubusercontent.com/8389039/212061370-6984b2c3-bc13-4d92-8afc-0068be4cdde1.png)
[^1]

## A solução atual:
**Atualmente temos todas as quatro métricas implementadas e em fase piloto. (O widget do Azure DevOps atualmente não está planejado - mas é possível se alguém quiser construí-lo!).**

- **Frequência de Implantação**, tanto no Azure DevOps quanto no GitHub:
  - Como funciona? Olhamos para o número de execuções de pipeline bem-sucedidas.
  - Suposições/coisas que não podemos medir atualmente:
      - A build é multi-stage e leva a uma implantação em um ambiente de produção.
      - Olhamos apenas para um único branch (geralmente o branch principal), portanto ignoramos branches de feature (pois provavelmente não estão implantando em produção)
  - Limitações atuais: Apenas uma build/run/branch pode ser especificada
![Frequência de Implantação](https://github.com/samsmithnz/DevOpsMetrics/blob/main/ReadmeImages/DeploymentFrequencyDemo.png)

- **Tempo de lead para mudanças**, tanto no Azure DevOps quanto no GitHub:
  - Como funciona? Olhamos para o número de execuções de pipeline bem-sucedidas e combinamos com Pull Requests
  - Suposições/coisas que não podemos medir atualmente:
      - Atualmente contamos as durações do pull request e da implantação, calculando a média para o período de tempo para criar a métrica de tempo de lead para mudanças.
      - Começamos a medir no último commit de um branch até o tempo de fechamento/merge do PR. O desenvolvimento é variável que depende da tarefa e não ajuda com essa medição.
      - Assumimos que estamos seguindo um processo de git flow, criando branches de feature e mesclando de volta ao branch principal, que é implantado em produção na conclusão dos pull requests
      - Assumimos que o usuário exige pull requests para mesclar o trabalho no branch principal - estamos olhando para todo o trabalho que não está nesse branch principal - portanto, atualmente suportamos apenas um branch principal.
  - Limitações atuais: Apenas um repositório e branch principal podem ser especificados
![Tempo de lead para mudanças](https://github.com/samsmithnz/DevOpsMetrics/blob/main/ReadmeImages/LeadTimeForChanges.png)

- **Tempo para restaurar serviço**, no Azure
  - Como funciona? Configuramos alertas do Azure Monitor em nossos recursos, por exemplo, em nosso serviço web, onde temos alertas para erros HTTP500 e HTTP403, bem como monitoramento de CPU e RAM. Se algum desses alertas for acionado, capturamos o alerta em uma função do Azure e salvamos em um armazenamento de tabela do Azure, onde podemos agregar e medir o tempo da interrupção. Quando o alerta é resolvido posteriormente, isso também aciona o mesmo fluxo de trabalho para salvar a resolução e registrar a restauração do serviço.
  - Suposições/coisas que não podemos medir atualmente:
      - Nosso projeto está hospedado no Azure
      - O ambiente de produção está contido em um único grupo de recursos
      - Existem alertas apropriados configurados em cada um dos recursos, cada um com grupos de ação para salvar o alerta no Armazenamento do Azure
      - Geramos um SLA, mas é inteiramente baseado no tempo MTTR - assumindo que a aplicação está "indisponível" durante esse tempo
  - Limitações atuais:
      - Apenas um grupo de recursos de produção pode ser especificado
      - Se houver falha catastrófica do grupo de recursos (por exemplo, excluído), há uma grande chance de que alguns/todos os alertas também sejam excluídos
![Tempo para restaurar serviço](https://github.com/samsmithnz/DevOpsMetrics/blob/main/ReadmeImages/TimeToRestoreService.png)

- **Taxa de falha de mudança**, no Azure DevOps e GitHub
  - Como funciona? Olhamos para builds e deixamos o usuário indicar se foi bem-sucedida ou uma falha. Por padrão (atualmente), a build é considerada uma falha. (Vamos mudar isso para sucesso por padrão mais tarde)
  - Suposições/coisas que não podemos medir atualmente:
      - A build é multi-stage e leva a uma implantação em um ambiente de produção.
      - Olhamos apenas para um único branch (geralmente o branch principal), portanto ignoramos branches de feature (pois provavelmente não estão implantando em produção)
      - O usuário revisou a build/implantação e confirmou que a implantação em produção foi bem-sucedida
  - Limitações atuais: Apenas uma build/run pode ser especificada
![Taxa de falha de mudança](https://github.com/samsmithnz/DevOpsMetrics/blob/main/ReadmeImages/ChangeFailureRate.png)

# Arquitetura
Desenvolvido em .NET 8. Uma ação do GitHub executa o processo CI/CD.

Atualmente, o processo CI/CD:
1. Compila o código
2. Executa os testes unitários
3. Implanta o serviço web em um aplicativo web do Azure (https://devops-prod-eu-service.azurewebsites.net)
4. Implanta o site de demonstração em um aplicativo web do Azure (https://devops-prod-eu-web.azurewebsites.net)
4. Implanta o site da função em uma função do Azure

Dependabot executa diariamente para verificar atualizações de dependências.

![Diagrama de Arquitetura](https://github.com/samsmithnz/DevOpsMetrics/blob/main/ReadmeImages/Architecture.png)

## Badges
A API pode gerar uma URL para badges estáticos, alguns exemplos são mostrados abaixo:
[![Build](https://img.shields.io/badge/Lead%20time%20for%20changes-High-green)](https://img.shields.io/badge/Lead%20time%20for%20changes-High-green) [![Build](https://img.shields.io/badge/Time%20to%20restore%20service-Medium-orange)](https://img.shields.io/badge/Time%20to%20restore%20service-Medium-orange) [![Build](https://img.shields.io/badge/Change%20failure%20rate-Low-red)](https://img.shields.io/badge/Change%20failure%20rate-Low-red)

# Configuração

## Implantando no Azure

- Execute o script de configuração de infraestrutura [Atualmente \src\DevOpsMetrics.Infrastructure\DeployInfrastructureToAzure2.ps1]
- Configuração do DevOpsMetrics.Service: URL do Keyvault e ID do Application Insights definidos como parte do script de configuração
- Navegue para [nome do site].azurewebsites.net/Home/Settings e configure seus projetos conforme necessário. Observe que todos os segredos são carregados no keyvault e são controlados por você!

## Para depurar/executar testes

# O que vem a seguir?
- Atualizações para empacotamento e configuração (em andamento)
- Atualizações para armazenar dados no CosmosDB (atualmente no armazenamento do Azure)
- Suporte para mais cenários, lançamentos, etc.
- ~~Integrações do marketplace do Azure DevOps, para que você possa ver as mudanças em tempo real no seu projeto/repositório.~~ (prioridade menor para focar no GitHub)

# Referências

- API do GitHub: https://developer.GitHub.com/v3/actions/workflow-runs/
- API do Azure DevOps: https://docs.microsoft.com/pt-br/rest/api/azure/devops/build/builds/list?view=azure-devops-rest-5.1

[^1]: Gráfico da [página 11 do relatório state of DevOps 2022](https://cloud.google.com/devops/state-of-devops)
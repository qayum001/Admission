### Работа с внешним справочником
> Был использован `NSwag` для генерации клиента.

1. Установить `NSwag`:
`dotnet tool install --global NSwag.ConsoleCore`

2. Сгенерировать клиента:
`nswag openapi2csclient /input:https://1c-mockup.kreosoft.space/swagger/v1/swagger.json /output:Generated\DictionaryClient.cs /classname:DictionaryClient /namespace:Dictionary.Integration /GenerateClientClasses:true /GenerateDtoTypes:true /OperationGenerationMode:MultipleClientsFromPathSegments`

### Docker Compose
Часто используемые команды для локального развертывания и обновления системы:

1. Первый запуск всех сервисов:
`docker compose up -d --build`

2. Пересобрать и поднять только backend-сервисы:
`docker compose up -d --build auth mail-manager`

3. Пересобрать и поднять только один сервис:
`docker compose up -d --build auth`
`docker compose up -d --build mail-manager`

4. Поднять сервисы без пересборки:
`docker compose up -d`

5. Посмотреть состояние контейнеров:
`docker compose ps`

6. Посмотреть логи конкретного сервиса:
`docker compose logs -f auth`
`docker compose logs -f mail-manager`

7. Остановить систему:
`docker compose down`

8. Остановить систему и удалить volumes:
`docker compose down -v`

9. Проверить итоговый статус и healthcheck контейнеров:
`docker ps`

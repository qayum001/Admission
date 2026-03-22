### Работа с внешним справочником
> был использован NSwag для генерации клиента

1. Надо установить NSwag - `dotnet tool install --global NSwag.ConsoleCore`

2. Запустить эту команду - `nswag openapi2csclient /input:https://1c-mockup.kreosoft.space/swagger/v1/swagger.json /output:Generated\DictionaryClient.cs /classname:DictionaryClient /namespace:Dictionary.Integration /GenerateClientClasses:true /GenerateDtoTypes:true /OperationGenerationMode:MultipleClientsFromPathSegments`
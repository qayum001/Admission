# Запуск

1. Надо склонировать репозиторий
2. Заходим в /Placeholder-Task-4472
3. Сам запуск:
   - Если есть докер - `docker compose up --build -d`
   - Если нету - то запустить по идее возможно, но я не гарантирую что сервисы найдут друг друга
4. Открыть ссылки которые будут ниже, и начать проверку системы
   - Auth swagger - http://localhost:5279/swagger/index.html
   - Dictionary swagger - http://localhost:5280/swagger/index.html
   - Mail Dev - http://localhost:1080

## ВАЖНО!!!
докер требудет что следующие порты были незаняты: 
- `5279` - Auth API
- `5280` - Dictionary API
- `5278` - Mail Manager API
- `1080` - MailDev Web UI
- `1025` - MailDev SMTP
- `5672` - RabbitMQ AMQP
- `15672` - RabbitMQ Management UI
- `5432` - PostgreSQL
- `6379` - Redis
- `5540` - Redis Insight
- `9000` - MinIO API (S3)
- `9001` - MinIO Console

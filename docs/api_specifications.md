# Спецификация API

## Система поступления в вуз

Документ описывает публичные API системы:
- **Auth API** — аутентификация, выдача токенов, управление учетными данными.
- **Admission API** — бизнес-функции для абитуриентов, менеджеров, главных менеджеров и администраторов.

> **Admin MVC** не имеет собственного публичного API. Он использует эндпоинты **Auth API** и **Admission API** от имени пользователя с ролью `Admin`.

---

## Роли

- `Applicant`
- `Manager`
- `GeneralManager`
- `Admin`

> `Admin` обладает всеми возможностями остальных ролей.

---

## Общие правила авторизации

- Аутентификация выполняется через **Auth API**.
- Клиенты получают пару токенов: `accessToken` + `refreshToken`.
- **Admission API** и **Admin MVC** принимают access token, выданный **Auth API**.
- Для защищенных эндпоинтов используется заголовок:

```http
Authorization: Bearer <access_token>
```

---

## Общие модели

### ApplicantProfile

```json
{
  "id": "7c70c1cc-6dbe-4a5b-8ae3-3a9f2f5a0c13",
  "fullName": "Иванов Иван Иванович",
  "email": "applicant@example.com",
  "phone": "+79990000000",
  "birthDate": "2006-05-20",
  "gender": "Male",
  "citizenship": "Russia"
}
```

### AdmissionStatus

- `Created`
- `InReview`
- `Approved`
- `Rejected`
- `Closed`

### PassportDocument

```json
{
  "id": "bb5f1e9f-e1f1-4e67-9b8e-6fcfdbbe0a30",
  "type": "Passport",
  "series": "1234",
  "number": "567890",
  "issuedBy": "ОВД Центрального района",
  "issuedAt": "2020-06-01",
  "birthPlace": "Москва",
  "files": [
    {
      "id": "3cf5c622-27f9-43a1-95a3-5e2cf03c1d6b",
      "fileName": "passport.pdf",
      "contentType": "application/pdf"
    }
  ]
}
```

### EducationDocument

```json
{
  "id": "35c4d308-d74e-4d52-bafc-b8b32df2bbd3",
  "type": "EducationDocument",
  "documentTypeId": "718d5c1d-bb04-4695-b557-fd315a7c5f59",
  "name": "Аттестат о среднем общем образовании",
  "educationLevelId": 1,
  "files": [
    {
      "id": "4a91f6ec-f79e-4edb-a9e9-86922ab9b06b",
      "fileName": "certificate.pdf",
      "contentType": "application/pdf"
    }
  ]
}
```

### Program

```json
{
  "id": "6deec183-cb8d-4cb5-bc30-c7a7d3f18a36",
  "name": "Прикладная информатика",
  "code": "09.03.03",
  "language": "Русский",
  "educationForm": "Очная",
  "faculty": {
    "id": "f18eaed6-d2b3-4a97-bb67-7ac61baf5167",
    "name": "Факультет информатики"
  },
  "educationLevel": {
    "id": 1,
    "name": "Бакалавриат"
  }
}
```

### Admission

```json
{
  "id": "cae95a6f-1bd7-4d56-8dd8-ef57ee421302",
  "applicantId": "7c70c1cc-6dbe-4a5b-8ae3-3a9f2f5a0c13",
  "status": "Created",
  "managerId": null,
  "updatedAt": "2026-03-14T12:00:00Z"
}
```

### ErrorResponse

```json
{
  "status": "error",
  "message": "Validation failed"
}
```

---

# 1. Auth API

## 1.1 Регистрация абитуриента

**Описание:** создание учетной записи абитуриента.

- **Метод:** `POST`
- **Эндпоинт:** `/api/auth/register`
- **Роли:** `Anonymous`

### Тело запроса

```json
{
  "fullName": "Иванов Иван Иванович",
  "email": "applicant@example.com",
  "phone": "+79990000000",
  "password": "StrongPassword123!"
}
```

### Ответ

`201 Created`

```json
{
  "userId": "7c70c1cc-6dbe-4a5b-8ae3-3a9f2f5a0c13",
  "role": "Applicant",
  "message": "User registered successfully"
}
```

---

## 1.2 Аутентификация пользователя

**Описание:** вход для всех ролей и получение access/refresh токенов.

- **Метод:** `POST`
- **Эндпоинт:** `/api/auth/login`
- **Роли:** `Anonymous`

### Тело запроса

```json
{
  "email": "user@example.com",
  "password": "StrongPassword123!"
}
```

### Ответ

`200 OK`

```json
{
  "accessToken": "jwt-access-token",
  "refreshToken": "refresh-token",
  "expiresIn": 900,
  "role": "Applicant"
}
```

---

## 1.3 Обновление access token

**Описание:** получение новой пары токенов по refresh token.

- **Метод:** `POST`
- **Эндпоинт:** `/api/auth/refresh`
- **Роли:** `Anonymous`

### Тело запроса

```json
{
  "refreshToken": "refresh-token"
}
```

### Ответ

`200 OK`

```json
{
  "accessToken": "new-jwt-access-token",
  "refreshToken": "new-refresh-token",
  "expiresIn": 900
}
```

---

## 1.4 Выход из системы

**Описание:** отзыв refresh token.

- **Метод:** `POST`
- **Эндпоинт:** `/api/auth/logout`
- **Роли:** `Applicant`, `Manager`, `GeneralManager`, `Admin`

### Тело запроса

```json
{
  "refreshToken": "refresh-token"
}
```

### Ответ

`200 OK`

```json
{
  "message": "Logged out successfully"
}
```

---

## 1.5 Смена пароля текущего пользователя

**Описание:** изменение пароля для текущего авторизованного пользователя.

- **Метод:** `POST`
- **Эндпоинт:** `/api/auth/change-password`
- **Роли:** `Applicant`, `Manager`, `GeneralManager`, `Admin`

### Тело запроса

```json
{
  "currentPassword": "OldPassword123!",
  "newPassword": "NewPassword123!"
}
```

### Ответ

`200 OK`

```json
{
  "message": "Password changed successfully"
}
```

---

## 1.6 Изменение email для входа

**Описание:** изменение email учетной записи сотрудника или администратора.

- **Метод:** `PATCH`
- **Эндпоинт:** `/api/auth/me/email`
- **Роли:** `Manager`, `GeneralManager`, `Admin`

### Тело запроса

```json
{
  "email": "new-email@example.com"
}
```

### Ответ

`200 OK`

```json
{
  "message": "Email updated successfully"
}
```

---

## 1.7 Создание менеджера или главного менеджера

**Описание:** создание учетной записи сотрудника. Используется из Admin MVC.

- **Метод:** `POST`
- **Эндпоинт:** `/api/auth/staff-users`
- **Роли:** `Admin`

### Тело запроса

```json
{
  "fullName": "Петров Петр Петрович",
  "email": "manager@example.com",
  "role": "Manager",
  "facultyId": "f18eaed6-d2b3-4a97-bb67-7ac61baf5167"
}
```

> Для `GeneralManager` поле `facultyId` может быть `null`.

### Ответ

`201 Created`

```json
{
  "userId": "0f4ce5b0-0132-4f8f-9a7a-c8af052a1f4d",
  "role": "Manager",
  "message": "Staff user created. Notification email sent"
}
```

---

## 1.8 Обновление учетных данных сотрудника

**Описание:** изменение email и роли сотрудника. Используется из Admin MVC.

- **Метод:** `PATCH`
- **Эндпоинт:** `/api/auth/staff-users/{userId}`
- **Роли:** `Admin`

### Тело запроса

```json
{
  "email": "manager-new@example.com",
  "role": "GeneralManager",
  "facultyId": null
}
```

### Ответ

`200 OK`

```json
{
  "message": "Staff user updated successfully"
}
```

---

## 1.9 Удаление учетной записи сотрудника

**Описание:** удаление менеджера или главного менеджера. Используется из Admin MVC.

- **Метод:** `DELETE`
- **Эндпоинт:** `/api/auth/staff-users/{userId}`
- **Роли:** `Admin`

### Ответ

`200 OK`

```json
{
  "message": "Staff user deleted successfully"
}
```

---

## 1.10 Получение списка сотрудников

**Описание:** список менеджеров и главных менеджеров. Может использоваться и главным менеджером, и администратором.

- **Метод:** `GET`
- **Эндпоинт:** `/api/auth/staff-users?role=Manager&page=1&size=20`
- **Роли:** `GeneralManager`, `Admin`

### Ответ

`200 OK`

```json
{
  "items": [
    {
      "id": "0f4ce5b0-0132-4f8f-9a7a-c8af052a1f4d",
      "fullName": "Петров Петр Петрович",
      "email": "manager@example.com",
      "role": "Manager",
      "facultyId": "f18eaed6-d2b3-4a97-bb67-7ac61baf5167"
    }
  ],
  "pagination": {
    "page": 1,
    "size": 20,
    "count": 1
  }
}
```

---

## 1.11 JWKS endpoint

**Описание:** публичные ключи для валидации JWT токенов другими сервисами.

- **Метод:** `GET`
- **Эндпоинт:** `/.well-known/jwks.json`
- **Роли:** `Anonymous`

### Ответ

`200 OK`

```json
{
  "keys": [
    {
      "kty": "RSA",
      "use": "sig",
      "alg": "RS256",
      "kid": "auth-key-1",
      "n": "base64url-modulus",
      "e": "AQAB"
    }
  ]
}
```

---

# 2. Admission API — абитуриент

## 2.1 Получение своего профиля

**Описание:** просмотр личного профиля абитуриента.

- **Метод:** `GET`
- **Эндпоинт:** `/api/applicants/me`
- **Роли:** `Applicant`, `Admin`

### Ответ

`200 OK`

```json
{
  "id": "7c70c1cc-6dbe-4a5b-8ae3-3a9f2f5a0c13",
  "fullName": "Иванов Иван Иванович",
  "email": "applicant@example.com",
  "phone": "+79990000000",
  "birthDate": "2006-05-20",
  "gender": "Male",
  "citizenship": "Russia"
}
```

---

## 2.2 Обновление своего профиля

**Описание:** изменение личных данных абитуриента. Недоступно, если статус поступления `Closed`.

- **Метод:** `PATCH`
- **Эндпоинт:** `/api/applicants/me`
- **Роли:** `Applicant`, `Admin`

### Тело запроса

```json
{
  "fullName": "Иванов Иван Иванович",
  "email": "applicant@example.com",
  "phone": "+79990000000",
  "birthDate": "2006-05-20",
  "gender": "Male",
  "citizenship": "Russia"
}
```

### Ответ

`200 OK`

```json
{
  "message": "Profile updated successfully"
}
```

---

## 2.3 Получение своего поступления

**Описание:** получение текущего заявления на поступление и его статуса.

- **Метод:** `GET`
- **Эндпоинт:** `/api/admissions/me`
- **Роли:** `Applicant`, `Admin`

### Ответ

`200 OK`

```json
{
  "id": "cae95a6f-1bd7-4d56-8dd8-ef57ee421302",
  "status": "Created",
  "managerId": null,
  "updatedAt": "2026-03-14T12:00:00Z"
}
```

---

## 2.4 Получение списка своих документов

**Описание:** получение всех документов абитуриента.

- **Метод:** `GET`
- **Эндпоинт:** `/api/applicants/me/documents`
- **Роли:** `Applicant`, `Admin`

### Ответ

`200 OK`

```json
{
  "passport": {
    "id": "bb5f1e9f-e1f1-4e67-9b8e-6fcfdbbe0a30",
    "type": "Passport"
  },
  "educationDocument": {
    "id": "35c4d308-d74e-4d52-bafc-b8b32df2bbd3",
    "type": "EducationDocument"
  }
}
```

---

## 2.5 Получение паспорта

**Описание:** просмотр данных паспорта.

- **Метод:** `GET`
- **Эндпоинт:** `/api/applicants/me/documents/passport`
- **Роли:** `Applicant`, `Admin`

### Ответ

`200 OK`

```json
{
  "id": "bb5f1e9f-e1f1-4e67-9b8e-6fcfdbbe0a30",
  "type": "Passport",
  "series": "1234",
  "number": "567890",
  "issuedBy": "ОВД Центрального района",
  "issuedAt": "2020-06-01",
  "birthPlace": "Москва",
  "files": [
    {
      "id": "3cf5c622-27f9-43a1-95a3-5e2cf03c1d6b",
      "fileName": "passport.pdf",
      "contentType": "application/pdf"
    }
  ]
}
```

---

## 2.6 Обновление паспорта

**Описание:** изменение данных паспорта. Недоступно, если статус поступления `Closed`.

- **Метод:** `PATCH`
- **Эндпоинт:** `/api/applicants/me/documents/passport`
- **Роли:** `Applicant`, `Admin`

### Тело запроса

```json
{
  "series": "1234",
  "number": "567890",
  "issuedBy": "ОВД Центрального района",
  "issuedAt": "2020-06-01",
  "birthPlace": "Москва"
}
```

### Ответ

`200 OK`

```json
{
  "message": "Passport updated successfully"
}
```

---

## 2.7 Получение документа об образовании

**Описание:** просмотр документа об образовании.

- **Метод:** `GET`
- **Эндпоинт:** `/api/applicants/me/documents/education`
- **Роли:** `Applicant`, `Admin`

### Ответ

`200 OK`

```json
{
  "id": "35c4d308-d74e-4d52-bafc-b8b32df2bbd3",
  "type": "EducationDocument",
  "documentTypeId": "718d5c1d-bb04-4695-b557-fd315a7c5f59",
  "name": "Аттестат о среднем общем образовании",
  "educationLevelId": 1,
  "files": []
}
```

---

## 2.8 Обновление документа об образовании

**Описание:** изменение данных документа об образовании. Недоступно, если статус поступления `Closed`.

- **Метод:** `PATCH`
- **Эндпоинт:** `/api/applicants/me/documents/education`
- **Роли:** `Applicant`, `Admin`

### Тело запроса

```json
{
  "documentTypeId": "718d5c1d-bb04-4695-b557-fd315a7c5f59",
  "name": "Аттестат о среднем общем образовании"
}
```

### Ответ

`200 OK`

```json
{
  "message": "Education document updated successfully"
}
```

---

## 2.9 Загрузка нового скана документа

**Описание:** загрузка файла скана для паспорта или документа об образовании. Недоступно, если статус поступления `Closed`.

- **Метод:** `POST`
- **Эндпоинт:** `/api/applicants/me/documents/{documentId}/files`
- **Роли:** `Applicant`, `Admin`

### Тело запроса

`multipart/form-data`

- `file` — бинарный файл

### Ответ

`201 Created`

```json
{
  "fileId": "3cf5c622-27f9-43a1-95a3-5e2cf03c1d6b",
  "fileName": "passport.pdf",
  "message": "File uploaded successfully"
}
```

---

## 2.10 Удаление скана документа

**Описание:** удаление файла документа. Недоступно, если статус поступления `Closed`.

- **Метод:** `DELETE`
- **Эндпоинт:** `/api/applicants/me/documents/{documentId}/files/{fileId}`
- **Роли:** `Applicant`, `Admin`

### Ответ

`200 OK`

```json
{
  "message": "File deleted successfully"
}
```

---

## 2.11 Получение ссылки на скачивание скана

**Описание:** получение временной ссылки на скачивание файла документа.

- **Метод:** `GET`
- **Эндпоинт:** `/api/applicants/me/documents/{documentId}/files/{fileId}/download-link`
- **Роли:** `Applicant`, `Admin`

### Ответ

`200 OK`

```json
{
  "url": "https://s3.example.com/...",
  "expiresAt": "2026-03-14T12:15:00Z"
}
```

---

## 2.12 Получение списка программ

**Описание:** список программ с пагинацией и фильтрацией.

- **Метод:** `GET`
- **Эндпоинт:** `/api/programs?page=1&size=10&facultyId={facultyId}&educationLevelId=1&educationForm=Очная&language=Русский&search=09.03`
- **Роли:** `Applicant`, `Manager`, `GeneralManager`, `Admin`

### Ответ

`200 OK`

```json
{
  "programs": [
    {
      "id": "6deec183-cb8d-4cb5-bc30-c7a7d3f18a36",
      "name": "Прикладная информатика",
      "code": "09.03.03",
      "language": "Русский",
      "educationForm": "Очная",
      "faculty": {
        "id": "f18eaed6-d2b3-4a97-bb67-7ac61baf5167",
        "name": "Факультет информатики"
      },
      "educationLevel": {
        "id": 1,
        "name": "Бакалавриат"
      }
    }
  ],
  "pagination": {
    "size": 10,
    "count": 1,
    "current": 1
  }
}
```

---

## 2.13 Получение выбранных программ

**Описание:** просмотр списка выбранных программ абитуриента.

- **Метод:** `GET`
- **Эндпоинт:** `/api/applicants/me/admission-programs`
- **Роли:** `Applicant`, `Admin`

### Ответ

`200 OK`

```json
{
  "items": [
    {
      "id": "da9d84f2-3ab5-4011-8fd9-8d1b0735c5d8",
      "priority": 1,
      "program": {
        "id": "6deec183-cb8d-4cb5-bc30-c7a7d3f18a36",
        "name": "Прикладная информатика",
        "code": "09.03.03"
      }
    }
  ]
}
```

---

## 2.14 Добавление программы в список выбранных

**Описание:** добавление программы в заявление абитуриента. Проверяются ограничения по количеству, уровню обучения и документу об образовании. Недоступно, если статус поступления `Closed`.

- **Метод:** `POST`
- **Эндпоинт:** `/api/applicants/me/admission-programs`
- **Роли:** `Applicant`, `Admin`

### Тело запроса

```json
{
  "programId": "6deec183-cb8d-4cb5-bc30-c7a7d3f18a36"
}
```

### Ответ

`201 Created`

```json
{
  "admissionProgramId": "da9d84f2-3ab5-4011-8fd9-8d1b0735c5d8",
  "priority": 1,
  "message": "Program added successfully"
}
```

---

## 2.15 Изменение приоритета программы

**Описание:** изменение приоритета выбранной программы. Недоступно, если статус поступления `Closed`.

- **Метод:** `PATCH`
- **Эндпоинт:** `/api/applicants/me/admission-programs/{admissionProgramId}`
- **Роли:** `Applicant`, `Admin`

### Тело запроса

```json
{
  "priority": 2
}
```

### Ответ

`200 OK`

```json
{
  "message": "Program priority updated successfully"
}
```

---

## 2.16 Удаление программы из выбранного списка

**Описание:** удаление программы из заявления. Недоступно, если статус поступления `Closed`.

- **Метод:** `DELETE`
- **Эндпоинт:** `/api/applicants/me/admission-programs/{admissionProgramId}`
- **Роли:** `Applicant`, `Admin`

### Ответ

`200 OK`

```json
{
  "message": "Program removed successfully"
}
```

---

# 3. Admission API — менеджеры и главные менеджеры

## 3.1 Получение списка заявок абитуриентов

**Описание:** список заявок с пагинацией, фильтрацией и сортировкой.

- **Метод:** `GET`
- **Эндпоинт:** `/api/admissions?page=1&size=20&search=иван&programId={programId}&facultyIds={facultyId1},{facultyId2}&status=Created&onlyUnassigned=true&onlyAssignedToMe=false&sortBy=updatedAt&sortOrder=desc`
- **Роли:** `Manager`, `GeneralManager`, `Admin`

### Ответ

`200 OK`

```json
{
  "items": [
    {
      "admissionId": "cae95a6f-1bd7-4d56-8dd8-ef57ee421302",
      "applicantId": "7c70c1cc-6dbe-4a5b-8ae3-3a9f2f5a0c13",
      "fullName": "Иванов Иван Иванович",
      "status": "Created",
      "managerId": null,
      "updatedAt": "2026-03-14T12:00:00Z"
    }
  ],
  "pagination": {
    "page": 1,
    "size": 20,
    "count": 1
  }
}
```

---

## 3.2 Получение карточки абитуриента по заявлению

**Описание:** просмотр полной информации по заявлению абитуриента.

- **Метод:** `GET`
- **Эндпоинт:** `/api/admissions/{admissionId}`
- **Роли:** `Manager`, `GeneralManager`, `Admin`

### Ответ

`200 OK`

```json
{
  "admission": {
    "id": "cae95a6f-1bd7-4d56-8dd8-ef57ee421302",
    "status": "Created",
    "managerId": null
  },
  "applicant": {
    "id": "7c70c1cc-6dbe-4a5b-8ae3-3a9f2f5a0c13",
    "fullName": "Иванов Иван Иванович",
    "email": "applicant@example.com",
    "phone": "+79990000000",
    "birthDate": "2006-05-20",
    "gender": "Male",
    "citizenship": "Russia"
  },
  "documents": {
    "passport": {
      "id": "bb5f1e9f-e1f1-4e67-9b8e-6fcfdbbe0a30"
    },
    "educationDocument": {
      "id": "35c4d308-d74e-4d52-bafc-b8b32df2bbd3"
    }
  },
  "programs": [
    {
      "id": "da9d84f2-3ab5-4011-8fd9-8d1b0735c5d8",
      "priority": 1,
      "programId": "6deec183-cb8d-4cb5-bc30-c7a7d3f18a36"
    }
  ]
}
```

---

## 3.3 Взять заявление в работу

**Описание:** менеджер назначает себя ответственным за заявление. Обычно статус переходит в `InReview`.

- **Метод:** `POST`
- **Эндпоинт:** `/api/admissions/{admissionId}/assign-to-me`
- **Роли:** `Manager`, `GeneralManager`, `Admin`

### Ответ

`200 OK`

```json
{
  "message": "Admission assigned successfully"
}
```

---

## 3.4 Отказаться от заявления

**Описание:** менеджер снимает себя с заявления и возвращает его в общий пул.

- **Метод:** `POST`
- **Эндпоинт:** `/api/admissions/{admissionId}/unassign`
- **Роли:** `Manager`, `GeneralManager`, `Admin`

### Ответ

`200 OK`

```json
{
  "message": "Admission returned to common pool"
}
```

---

## 3.5 Обновление личных данных абитуриента

**Описание:** менеджер обновляет личные данные абитуриента. Обычный менеджер может редактировать только свои заявки.

- **Метод:** `PATCH`
- **Эндпоинт:** `/api/admissions/{admissionId}/applicant-profile`
- **Роли:** `Manager`, `GeneralManager`, `Admin`

### Тело запроса

```json
{
  "fullName": "Иванов Иван Иванович",
  "email": "applicant@example.com",
  "phone": "+79990000000",
  "birthDate": "2006-05-20",
  "gender": "Male",
  "citizenship": "Russia"
}
```

### Ответ

`200 OK`

```json
{
  "message": "Applicant profile updated successfully"
}
```

---

## 3.6 Обновление паспорта абитуриента

**Описание:** менеджер редактирует паспортные данные абитуриента. Обычный менеджер может редактировать только свои заявки.

- **Метод:** `PATCH`
- **Эндпоинт:** `/api/admissions/{admissionId}/documents/passport`
- **Роли:** `Manager`, `GeneralManager`, `Admin`

### Тело запроса

```json
{
  "series": "1234",
  "number": "567890",
  "issuedBy": "ОВД Центрального района",
  "issuedAt": "2020-06-01",
  "birthPlace": "Москва"
}
```

### Ответ

`200 OK`

```json
{
  "message": "Passport updated successfully"
}
```

---

## 3.7 Обновление документа об образовании абитуриента

**Описание:** менеджер редактирует документ об образовании абитуриента. Обычный менеджер может редактировать только свои заявки.

- **Метод:** `PATCH`
- **Эндпоинт:** `/api/admissions/{admissionId}/documents/education`
- **Роли:** `Manager`, `GeneralManager`, `Admin`

### Тело запроса

```json
{
  "documentTypeId": "718d5c1d-bb04-4695-b557-fd315a7c5f59",
  "name": "Аттестат о среднем общем образовании"
}
```

### Ответ

`200 OK`

```json
{
  "message": "Education document updated successfully"
}
```

---

## 3.8 Загрузка скана документа абитуриента

**Описание:** менеджер загружает новый скан документа абитуриента. Обычный менеджер может редактировать только свои заявки.

- **Метод:** `POST`
- **Эндпоинт:** `/api/admissions/{admissionId}/documents/{documentId}/files`
- **Роли:** `Manager`, `GeneralManager`, `Admin`

### Тело запроса

`multipart/form-data`

- `file` — бинарный файл

### Ответ

`201 Created`

```json
{
  "fileId": "3cf5c622-27f9-43a1-95a3-5e2cf03c1d6b",
  "message": "File uploaded successfully"
}
```

---

## 3.9 Удаление скана документа абитуриента

**Описание:** менеджер удаляет скан документа абитуриента. Обычный менеджер может редактировать только свои заявки.

- **Метод:** `DELETE`
- **Эндпоинт:** `/api/admissions/{admissionId}/documents/{documentId}/files/{fileId}`
- **Роли:** `Manager`, `GeneralManager`, `Admin`

### Ответ

`200 OK`

```json
{
  "message": "File deleted successfully"
}
```

---

## 3.10 Получение ссылки на скачивание файла абитуриента

**Описание:** получение временной ссылки на скачивание скана документа абитуриента.

- **Метод:** `GET`
- **Эндпоинт:** `/api/admissions/{admissionId}/documents/{documentId}/files/{fileId}/download-link`
- **Роли:** `Manager`, `GeneralManager`, `Admin`

### Ответ

`200 OK`

```json
{
  "url": "https://s3.example.com/...",
  "expiresAt": "2026-03-14T12:15:00Z"
}
```

---

## 3.11 Изменение приоритета программы абитуриента

**Описание:** менеджер меняет приоритет программы. Обычный менеджер может редактировать только свои заявки.

- **Метод:** `PATCH`
- **Эндпоинт:** `/api/admissions/{admissionId}/admission-programs/{admissionProgramId}`
- **Роли:** `Manager`, `GeneralManager`, `Admin`

### Тело запроса

```json
{
  "priority": 2
}
```

### Ответ

`200 OK`

```json
{
  "message": "Program priority updated successfully"
}
```

---

## 3.12 Удаление программы из заявления абитуриента

**Описание:** менеджер удаляет программу из заявления. Обычный менеджер может редактировать только свои заявки.

- **Метод:** `DELETE`
- **Эндпоинт:** `/api/admissions/{admissionId}/admission-programs/{admissionProgramId}`
- **Роли:** `Manager`, `GeneralManager`, `Admin`

### Ответ

`200 OK`

```json
{
  "message": "Program removed successfully"
}
```

---

## 3.13 Изменение статуса поступления

**Описание:** изменение статуса заявления абитуриента. При установке статуса `Closed` абитуриент больше не может редактировать свои данные.

- **Метод:** `PATCH`
- **Эндпоинт:** `/api/admissions/{admissionId}/status`
- **Роли:** `Manager`, `GeneralManager`, `Admin`

### Тело запроса

```json
{
  "status": "Approved",
  "comment": "Данные проверены"
}
```

### Ответ

`200 OK`

```json
{
  "message": "Admission status updated successfully"
}
```

---

## 3.14 Назначение менеджера на свободное заявление

**Описание:** главный менеджер или администратор назначает менеджера на свободное заявление. Должны быть отправлены уведомления менеджеру и абитуриенту.

- **Метод:** `POST`
- **Эндпоинт:** `/api/admissions/{admissionId}/manager-assignment`
- **Роли:** `GeneralManager`, `Admin`

### Тело запроса

```json
{
  "managerId": "0f4ce5b0-0132-4f8f-9a7a-c8af052a1f4d"
}
```

### Ответ

`200 OK`

```json
{
  "message": "Manager assigned successfully. Notifications sent"
}
```

---

# 4. Applicant Dictionary API — справочники и импорт

## 4.1 Получение списка факультетов

**Описание:** получение списка факультетов.

- **Метод:** `GET`
- **Эндпоинт:** `/api/dictionary/faculties`
- **Роли:** `Applicant`, `Manager`, `GeneralManager`, `Admin`

### Ответ

`200 OK`

```json
[
  {
    "id": "f18eaed6-d2b3-4a97-bb67-7ac61baf5167",
    "createTime": "2026-03-01T10:00:00Z",
    "name": "Факультет информатики"
  }
]
```

---

## 4.2 Получение списка уровней образования

**Описание:** получение списка уровней образования.

- **Метод:** `GET`
- **Эндпоинт:** `/api/dictionary/education-levels`
- **Роли:** `Applicant`, `Manager`, `GeneralManager`, `Admin`

### Ответ

`200 OK`

```json
[
  {
    "id": 1,
    "name": "Бакалавриат"
  }
]
```

---

## 4.3 Получение списка типов документов об образовании

**Описание:** получение списка типов документов об образовании.

- **Метод:** `GET`
- **Эндпоинт:** `/api/dictionary/education-document-types`
- **Роли:** `Applicant`, `Manager`, `GeneralManager`, `Admin`

### Ответ

`200 OK`

```json
[
  {
    "id": "718d5c1d-bb04-4695-b557-fd315a7c5f59",
    "name": "Аттестат",
    "educationLevel": {
      "id": 1,
      "name": "Бакалавриат"
    },
    "nextEducationLevels": [
      {
        "id": 2,
        "name": "Магистратура"
      }
    ]
  }
]
```

---

## 4.4 Запуск импорта справочников

**Описание:** импорт факультетов, программ, уровней образования и типов документов из внешней системы. Используется из Admin MVC.

- **Метод:** `POST`
- **Эндпоинт:** `/api/admin/imports/dictionaries`
- **Роли:** `Admin`

### Тело запроса

```json
{
  "types": ["faculties", "programs", "educationLevels", "educationDocumentTypes"]
}
```

### Ответ

`202 Accepted`

```json
{
  "importId": "6dbf7ea3-70d7-4671-9b86-7f4e626dbfe4",
  "status": "Queued"
}
```

---

## 4.5 Получение статуса импорта справочников

**Описание:** просмотр статуса задачи импорта. Используется из Admin MVC.

- **Метод:** `GET`
- **Эндпоинт:** `/api/admin/imports/dictionaries/{importId}`
- **Роли:** `Admin`

### Ответ

`200 OK`

```json
{
  "importId": "6dbf7ea3-70d7-4671-9b86-7f4e626dbfe4",
  "status": "Completed",
  "startedAt": "2026-03-14T10:00:00Z",
  "finishedAt": "2026-03-14T10:01:30Z"
}
```

---

# 5. Замечания по бизнес-правилам

## 5.1 Ограничения для абитуриента

- Если статус поступления равен `Closed`, абитуриент не может:
    - редактировать личные данные;
    - редактировать документы;
    - загружать и удалять сканы;
    - добавлять, удалять и менять приоритет программ.

## 5.2 Ограничения по программам поступления

При добавлении программы в заявление Admission API обязан проверять:

- количество выбранных программ не больше `N`;
- все выбранные программы относятся к одной допустимой ступени обучения;
- если у абитуриента есть документ об образовании, уровень программы должен:
    - либо совпадать с уровнем документа,
    - либо входить в список доступных для дальнейшего обучения уровней.

## 5.3 Ограничения для менеджеров

- `Manager` может просматривать все заявления и данные всех абитуриентов;
- `Manager` может редактировать только те заявления, где он назначен ответственным менеджером;
- `GeneralManager` и `Admin` могут просматривать и редактировать все заявления.

## 5.4 Email-уведомления

Email-уведомления должны отправляться при следующих событиях:

- изменение статуса поступления;
- назначение менеджера на заявление;
- создание нового менеджера или главного менеджера.

---

# 6. Предлагаемая структура использования API

## Applicant client

- использует **Auth API** для регистрации, входа, обновления токенов и смены пароля;
- использует **Admission API** для профиля, документов, программ и просмотра своего поступления.

## Staff client / Manager UI

- использует **Auth API** для входа и изменения учетных данных;
- использует **Admission API** для работы с заявками абитуриентов.

## Admin MVC

- использует **Auth API** для входа администратора и управления учетными записями сотрудников;
- использует **Applicant Dictionary API** для импорта справочников.

---

# 7. Минимальные коды ответов

Для большинства эндпоинтов рекомендуется использовать следующие коды:

- `200 OK` — успешное выполнение запроса
- `201 Created` — сущность создана
- `202 Accepted` — задача поставлена в обработку
- `400 Bad Request` — ошибка валидации
- `401 Unauthorized` — пользователь не аутентифицирован
- `403 Forbidden` — недостаточно прав
- `404 Not Found` — сущность не найдена
- `409 Conflict` — конфликт бизнес-правил
- `500 Internal Server Error` — внутренняя ошибка сервиса

Пример ошибки:

```json
{
  "status": "error",
  "message": "Applicant cannot edit data when admission status is Closed"
}
```

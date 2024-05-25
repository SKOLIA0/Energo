# Energo

Таблица клиентов(ID + наименование)

Таблица со счетами клиентов(номер счета + остаток)

Реализовать просмотр списка клиентов и его счетов.

Реализовать простую проводку между счетами клиентов – просто дебет-кредит суммы с изменением остатка на счете.

Реализовать выгрузку импорт-экспорт списка клиентов со счетами в XML.

Реализовать историю операций проводок по счетам клиентов

Язык C# или Python, БД на усмотрение.


## Установка

1. Склонируйте репозиторий:
    ```sh
    git clone https://github.com/SKOLIA0/Energo.git
    ```

2. Перейдите в директорию проекта:
    ```sh
    cd Energo
    ```

3. Установите необходимые зависимости:
    ```sh
    dotnet restore
    ```

4. Создайте файл `.env` в корне проекта и добавьте следующие строки:
    ```env
    DB_HOST=your_database_host
    DB_USER=your_database_user
    DB_PASSWORD=your_database_password
    DB_DATABASE=your_database_name
    ```
## Развертывание базы данных
Тип базы данных

PostgreSQL 16

1 CPU • 1 Гб RAM • 8 Гб NVMe

в образце пользователь gen_user

Подключитесь к вашему серверу PostgreSQL и создайте новую базу данных:
    ```sql
    CREATE DATABASE energo_db;
    \c energo_db;
    ```

2. Выполните SQL-скрипт для создания таблиц, функций и триггеров:
```sql
-- Создание таблицы клиентов
-- Таблица Clients хранит информацию о клиентах
CREATE TABLE Clients (
    ID SERIAL PRIMARY KEY,       -- Уникальный идентификатор клиента
    Name VARCHAR(100) NOT NULL,  -- Имя клиента
    CreatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP,  -- Дата создания записи
    UpdatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP   -- Дата последнего обновления записи
);
```
```sql 
-- Создание таблицы счетов
-- Таблица Accounts хранит информацию о счетах клиентов
CREATE TABLE Accounts (
    AccountNumber SERIAL PRIMARY KEY,  -- Уникальный номер счета
    ClientID INT REFERENCES Clients(ID),  -- Идентификатор клиента, связанный со счетом
    Balance DECIMAL(18, 2) NOT NULL,   -- Баланс счета
    CreatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP,  -- Дата создания записи
    UpdatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP   -- Дата последнего обновления записи
);
```

```sql 
-- Создание таблицы транзакций
-- Таблица Transactions хранит информацию о транзакциях между счетами
CREATE TABLE Transactions (
    TransactionID SERIAL PRIMARY KEY,  -- Уникальный идентификатор транзакции
    FromAccount INT REFERENCES Accounts(AccountNumber),  -- Счет отправителя
    ToAccount INT REFERENCES Accounts(AccountNumber),  -- Счет получателя
    Amount DECIMAL(18, 2) NOT NULL,  -- Сумма транзакции
    TransactionDate TIMESTAMP DEFAULT CURRENT_TIMESTAMP,  -- Дата транзакции
    CreatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP,  -- Дата создания записи
    UpdatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP   -- Дата последнего обновления записи
);
```

```sql 
-- Создание функции для обновления поля UpdatedAt
-- Эта функция будет использоваться триггерами для автоматического обновления поля UpdatedAt при изменении записей
CREATE OR REPLACE FUNCTION update_updated_at_column()
RETURNS TRIGGER AS $$
BEGIN
    NEW.UpdatedAt = CURRENT_TIMESTAMP;
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;
```
```sql 
-- Создание триггера для таблицы Clients
-- Триггер обновляет поле UpdatedAt при изменении записей в таблице Clients
CREATE TRIGGER update_clients_updated_at
BEFORE UPDATE ON Clients
FOR EACH ROW
EXECUTE FUNCTION update_updated_at_column();
```
```sql 
-- Создание триггера для таблицы Accounts
-- Триггер обновляет поле UpdatedAt при изменении записей в таблице Accounts
CREATE TRIGGER update_accounts_updated_at
BEFORE UPDATE ON Accounts
FOR EACH ROW
EXECUTE FUNCTION update_updated_at_column();
```
```sql 
-- Вставка тестовых данных в таблицу Clients
INSERT INTO Clients (Name) VALUES 
('Client 1'), 
('Client 2'), 
('Client 3'), 
('Client 4'), 
('Client 5');
```
```sql 
-- Вставка тестовых данных в таблицу Accounts
INSERT INTO Accounts (ClientID, Balance) VALUES 
(1, 1000.00), (1, 1500.00),
(2, 2000.00), (2, 2500.00),
(3, 3000.00), (3, 3500.00),
(4, 4000.00), (4, 4500.00),
(5, 5000.00), (5, 5500.00);
```

```sql 
-- Создание триггера для таблицы Transactions
-- Триггер обновляет поле UpdatedAt при изменении записей в таблице Transactions
CREATE TRIGGER update_transactions_updated_at
BEFORE UPDATE ON Transactions
FOR EACH ROW
EXECUTE FUNCTION update_updated_at_column();
```
```sql 
-- Создание хранимой процедуры для выполнения транзакций между счетами
-- Хранимая процедура проверяет наличие достаточного баланса на счете отправителя, выполняет транзакцию и обновляет балансы
CREATE OR REPLACE FUNCTION transfer_amount(
    from_account INT,
    to_account INT,
    amount DECIMAL)
RETURNS VOID AS $$
DECLARE
    from_balance DECIMAL;
BEGIN
    -- Проверка наличия достаточного баланса на счете отправителя
    SELECT Balance INTO from_balance FROM Accounts WHERE AccountNumber = from_account;
    IF from_balance < amount THEN
        RAISE EXCEPTION 'Insufficient funds';
    END IF;

    -- Выполнение транзакции
    UPDATE Accounts SET Balance = Balance - amount, UpdatedAt = CURRENT_TIMESTAMP WHERE AccountNumber = from_account;
    UPDATE Accounts SET Balance = Balance + amount, UpdatedAt = CURRENT_TIMESTAMP WHERE AccountNumber = to_account;
    INSERT INTO Transactions (FromAccount, ToAccount, Amount) VALUES (from_account, to_account, amount);
END;
$$ LANGUAGE plpgsql;

-- Назначение владельца функций
ALTER FUNCTION update_updated_at_column() OWNER TO gen_user;
ALTER FUNCTION transfer_amount(integer, integer, numeric) OWNER TO gen_user;

-- Предоставление прав на выполнение функций
GRANT EXECUTE ON FUNCTION update_updated_at_column() TO PUBLIC;
GRANT EXECUTE ON FUNCTION transfer_amount(integer, integer, numeric) TO PUBLIC;
GRANT EXECUTE ON FUNCTION update_updated_at_column() TO gen_user;
GRANT EXECUTE ON FUNCTION transfer_amount(integer, integer, numeric) TO gen_user;
-- Скопируйте и выполните скрипт, представленный выше
```


## Использование

1. Откройте проект в Visual Studio.
2. Запустите проект (F5).

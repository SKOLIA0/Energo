using System;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using Npgsql;

namespace Energo
{
    public partial class MainForm : Form
    {
        private NpgsqlConnection connection;

        // Конструктор формы
        public MainForm()
        {
            InitializeComponent();

            try
            {
                // Получение переменных окружения
                string host = Environment.GetEnvironmentVariable("DB_HOST");
                string user = Environment.GetEnvironmentVariable("DB_USER");
                string password = Environment.GetEnvironmentVariable("DB_PASSWORD");
                string database = Environment.GetEnvironmentVariable("DB_DATABASE");

                // Проверка наличия необходимых переменных окружения
                if (string.IsNullOrEmpty(host) || string.IsNullOrEmpty(user) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(database))
                {
                    throw new ArgumentException("Одна или несколько переменных окружения отсутствуют или некорректны.");
                }

                // Формирование строки подключения к базе данных
                string connectionString = $"Host={host};Username={user};Password={password};Database={database}";
                connection = new NpgsqlConnection(connectionString);

                // Попытка открытия соединения для проверки правильности подключения
                connection.Open();
                connection.Close();

                // Загрузка списка клиентов при инициализации формы
                LoadClients();
            }
            catch (ArgumentException argEx)
            {
                MessageBox.Show($"Ошибка конфигурации: {argEx.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.Exit(1); // Завершение программы в случае отсутствия переменных окружения
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка соединения с базой данных: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.Exit(1); // Завершение программы в случае ошибки подключения
            }
            // Подписка на событие загрузки формы
            this.Load += new EventHandler(MainForm_Load);
        }

        // Метод для обработки загрузки формы
        private void MainForm_Load(object sender, EventArgs e)
        {
            LoadClients(); // Загрузка списка клиентов

            // Выбор первого клиента и его счета
            if (dataGridViewClients.Rows.Count > 0)
            {
                dataGridViewClients.Rows[0].Selected = true;
                int clientId = Convert.ToInt32(dataGridViewClients.Rows[0].Cells["ID"].Value);
                LoadAccounts(clientId);

                if (dataGridViewAccounts.Rows.Count > 0)
                {
                    dataGridViewAccounts.Rows[0].Selected = true;
                    int accountNumber = Convert.ToInt32(dataGridViewAccounts.Rows[0].Cells["AccountNumber"].Value);
                    LoadTransactions(accountNumber);
                }
            }
        }


        // Метод для загрузки списка клиентов
        private void LoadClients()
        {
            try
            {
                string query = "SELECT ID, Name FROM Clients"; // SQL-запрос для получения списка клиентов
                NpgsqlDataAdapter adapter = new NpgsqlDataAdapter(query, connection); // Адаптер для выполнения запроса
                DataTable clientsTable = new DataTable(); // Таблица для хранения результатов запроса
                adapter.Fill(clientsTable); // Заполнение таблицы данными
                dataGridViewClients.DataSource = clientsTable; // Установка таблицы в качестве источника данных для грида
                dataGridViewClients.Columns["ID"].Visible = false; // Скрытие колонки ID

                if (clientsTable.Rows.Count == 0)
                {
                    MessageBox.Show("Нет данных в таблице Clients", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных клиентов: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Обработчик события клика по ячейке в гриде клиентов
        private void dataGridViewClients_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && dataGridViewClients.Rows[e.RowIndex].Cells["ID"].Value != DBNull.Value)
            {
                DataGridViewRow row = dataGridViewClients.Rows[e.RowIndex];
                int clientId = Convert.ToInt32(row.Cells["ID"].Value); // Получение ID выбранного клиента
                LoadAccounts(clientId); // Загрузка счетов для выбранного клиента
            }
        }

        // Метод для загрузки счетов клиента
        private void LoadAccounts(int clientId)
        {
            try
            {
                string query = "SELECT AccountNumber, Balance FROM Accounts WHERE ClientID = @ClientID"; // SQL-запрос для получения счетов клиента
                NpgsqlDataAdapter adapter = new NpgsqlDataAdapter(query, connection); // Адаптер для выполнения запроса
                adapter.SelectCommand.Parameters.AddWithValue("@ClientID", clientId); // Добавление параметра запроса
                DataTable accountsTable = new DataTable(); // Таблица для хранения результатов запроса
                adapter.Fill(accountsTable); // Заполнение таблицы данными
                dataGridViewAccounts.DataSource = accountsTable; // Установка таблицы в качестве источника данных для грида

                if (accountsTable.Rows.Count == 0)
                {
                    MessageBox.Show("Нет данных в таблице Accounts для выбранного клиента", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    // Автоматический выбор первой строки и загрузка истории транзакций
                    dataGridViewAccounts.Rows[0].Selected = true;
                    int accountNumber = Convert.ToInt32(accountsTable.Rows[0]["AccountNumber"]);
                    LoadTransactions(accountNumber);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных счетов: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Обработчик события клика по ячейке в гриде счетов
        private void dataGridViewAccounts_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && dataGridViewAccounts.Rows[e.RowIndex].Cells["AccountNumber"].Value != DBNull.Value)
            {
                DataGridViewRow row = dataGridViewAccounts.Rows[e.RowIndex];
                int accountNumber = Convert.ToInt32(row.Cells["AccountNumber"].Value); // Получение номера счета
                LoadTransactions(accountNumber); // Загрузка транзакций для выбранного счета
            }
        }

        // Метод для загрузки транзакций по счету
        private void LoadTransactions(int accountNumber)
        {
            try
            {
                string query = @"
                    SELECT 
                        TransactionID, 
                        FromAccount, 
                        ToAccount, 
                        Amount, 
                        TransactionDate 
                    FROM Transactions 
                    WHERE FromAccount = @AccountNumber OR ToAccount = @AccountNumber 
                    ORDER BY TransactionDate DESC"; // SQL-запрос для получения транзакций по счету
                NpgsqlDataAdapter adapter = new NpgsqlDataAdapter(query, connection); // Адаптер для выполнения запроса
                adapter.SelectCommand.Parameters.AddWithValue("@AccountNumber", accountNumber); // Добавление параметра запроса
                DataTable transactionsTable = new DataTable(); // Таблица для хранения результатов запроса
                adapter.Fill(transactionsTable); // Заполнение таблицы данными
                dataGridViewAccountHistory.DataSource = transactionsTable; // Установка таблицы в качестве источника данных для грида

                if (transactionsTable.Rows.Count == 0)
                {
                    MessageBox.Show("Нет данных в таблице Transactions для выбранного счета", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных транзакций: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Обработчик события нажатия кнопки для выполнения транзакции
        private void buttonTransfer_Click(object sender, EventArgs e)
        {
            using (TransactionForm transactionForm = new TransactionForm(connection))
            {
                transactionForm.ShowDialog(); // Открытие формы для выполнения транзакции
            }
        }

        // Обработчик события нажатия кнопки для экспорта данных в XML
        private void buttonExport_Click(object sender, EventArgs e)
        {
            ExportToXml(); // Вызов метода для экспорта данных
        }

        // Обработчик события нажатия кнопки для импорта данных из XML
        private void buttonImport_Click(object sender, EventArgs e)
        {
            ImportFromXml(); // Вызов метода для импорта данных
        }

        // Метод для экспорта данных в XML файл
        private void ExportToXml()
        {
            try
            {
                DataSet dataSet = new DataSet("NewDataSet"); // Создание нового набора данных
                string clientsQuery = "SELECT * FROM Clients"; // SQL-запрос для получения всех клиентов
                string accountsQuery = "SELECT * FROM Accounts"; // SQL-запрос для получения всех счетов

                NpgsqlDataAdapter clientsAdapter = new NpgsqlDataAdapter(clientsQuery, connection); // Адаптер для выполнения запроса клиентов
                NpgsqlDataAdapter accountsAdapter = new NpgsqlDataAdapter(accountsQuery, connection); // Адаптер для выполнения запроса счетов

                DataTable clientsTable = new DataTable("Clients"); // Таблица для хранения данных клиентов
                DataTable accountsTable = new DataTable("Accounts"); // Таблица для хранения данных счетов

                clientsAdapter.Fill(clientsTable); // Заполнение таблицы данными клиентов
                accountsAdapter.Fill(accountsTable); // Заполнение таблицы данными счетов

                dataSet.Tables.Add(clientsTable); // Добавление таблицы клиентов в набор данных
                dataSet.Tables.Add(accountsTable); // Добавление таблицы счетов в набор данных

                // Формирование названия файла с текущей датой и временем
                string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmssfff");
                string fileName = $"EnergoLIST_{timestamp}.xml";

                // Настройки для записи XML файла
                XmlWriterSettings settings = new XmlWriterSettings
                {
                    Indent = true,
                    IndentChars = "  ",
                    NewLineChars = Environment.NewLine,
                    Encoding = Encoding.UTF8
                };

                // Запись данных в XML файл с применением схемы
                using (XmlWriter writer = XmlWriter.Create(fileName, settings))
                {
                    dataSet.WriteXml(writer, XmlWriteMode.WriteSchema);
                }

                MessageBox.Show($"Данные успешно экспортированы в файл: {fileName}", "Экспорт данных", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка экспорта данных в XML: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Метод для импорта данных из XML файла
        private void ImportFromXml()
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog
                {
                    Filter = "XML files (*.xml)|*.xml|All files (*.*)|*.*",
                    Title = "Выберите XML файл для импорта"
                };

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    DataSet dataSet = new DataSet();
                    dataSet.ReadXml(openFileDialog.FileName);

                    foreach (DataTable table in dataSet.Tables)
                    {
                        foreach (DataRow row in table.Rows)
                        {
                            // Формирование SQL-запроса для вставки данных
                            string insertQuery = $"INSERT INTO {table.TableName} ({string.Join(", ", table.Columns.Cast<DataColumn>().Select(c => c.ColumnName))}) VALUES ({string.Join(", ", row.ItemArray.Select((item, index) => $"@p{index}"))}) ON CONFLICT DO NOTHING";
                            using (NpgsqlCommand command = new NpgsqlCommand(insertQuery, connection))
                            {
                                for (int i = 0; i < row.ItemArray.Length; i++)
                                {
                                    object value = row.ItemArray[i];
                                    string columnName = table.Columns[i].ColumnName;
                                    Type columnType = table.Columns[i].DataType;

                                    // Обработка столбцов для таблицы Clients
                                    if (table.TableName == "Clients" && columnName == "id")
                                    {
                                        command.Parameters.Add(new NpgsqlParameter($"@p{i}", NpgsqlTypes.NpgsqlDbType.Integer) { Value = Convert.ToInt32(value) });
                                    }
                                    else if (table.TableName == "Clients" && (columnName == "createdat" || columnName == "updatedat"))
                                    {
                                        command.Parameters.Add(new NpgsqlParameter($"@p{i}", NpgsqlTypes.NpgsqlDbType.Timestamp) { Value = Convert.ToDateTime(value) });
                                    }
                                    // Обработка столбцов для таблицы Accounts
                                    else if (table.TableName == "Accounts" && (columnName == "accountnumber" || columnName == "clientid"))
                                    {
                                        command.Parameters.Add(new NpgsqlParameter($"@p{i}", NpgsqlTypes.NpgsqlDbType.Integer) { Value = Convert.ToInt32(value) });
                                    }
                                    else if (table.TableName == "Accounts" && columnName == "balance")
                                    {
                                        command.Parameters.Add(new NpgsqlParameter($"@p{i}", NpgsqlTypes.NpgsqlDbType.Numeric) { Value = Convert.ToDecimal(value) });
                                    }
                                    else if (table.TableName == "Accounts" && (columnName == "createdat" || columnName == "updatedat"))
                                    {
                                        command.Parameters.Add(new NpgsqlParameter($"@p{i}", NpgsqlTypes.NpgsqlDbType.Timestamp) { Value = Convert.ToDateTime(value) });
                                    }
                                    // Общая обработка для всех таблиц
                                    else if (columnType == typeof(int))
                                    {
                                        command.Parameters.Add(new NpgsqlParameter($"@p{i}", NpgsqlTypes.NpgsqlDbType.Integer) { Value = Convert.ToInt32(value) });
                                    }
                                    else if (columnType == typeof(decimal))
                                    {
                                        command.Parameters.Add(new NpgsqlParameter($"@p{i}", NpgsqlTypes.NpgsqlDbType.Numeric) { Value = Convert.ToDecimal(value) });
                                    }
                                    else if (columnType == typeof(DateTime))
                                    {
                                        command.Parameters.Add(new NpgsqlParameter($"@p{i}", NpgsqlTypes.NpgsqlDbType.Timestamp) { Value = Convert.ToDateTime(value) });
                                    }
                                    else
                                    {
                                        command.Parameters.Add(new NpgsqlParameter($"@p{i}", NpgsqlTypes.NpgsqlDbType.Text) { Value = value.ToString() });
                                    }
                                }

                                try
                                {
                                    connection.Open();
                                    command.ExecuteNonQuery();
                                    connection.Close();
                                }
                                catch (Exception ex)
                                {
                                    connection.Close();
                                    string debugInfo = $"Ошибка добавления данных в таблицу {table.TableName}.\nЗапрос: {insertQuery}\nПараметры:";
                                    for (int j = 0; j < command.Parameters.Count; j++)
                                    {
                                        debugInfo += $"\n@p{j} ({command.Parameters[$"@p{j}"].NpgsqlDbType}) = {command.Parameters[$"@p{j}"].Value}";
                                    }
                                    MessageBox.Show($"{debugInfo}\nОшибка: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                            }
                        }
                    }

                    MessageBox.Show("База данных успешно обновлена данными из XML файла.", "Импорт данных", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка импорта данных из XML: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}

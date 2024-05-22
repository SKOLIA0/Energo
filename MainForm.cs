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

                string connectionString = $"Host={host};Username={user};Password={password};Database={database}";
                connection = new NpgsqlConnection(connectionString);

                // Попытка открытия соединения для проверки правильности подключения
                connection.Open();
                connection.Close();

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
        }

        private void LoadClients()
        {
            try
            {
                string query = "SELECT ID, Name FROM Clients";
                NpgsqlDataAdapter adapter = new NpgsqlDataAdapter(query, connection);
                DataTable clientsTable = new DataTable();
                adapter.Fill(clientsTable);
                dataGridViewClients.DataSource = clientsTable;
                dataGridViewClients.Columns["ID"].Visible = false;

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

        private void dataGridViewClients_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridViewClients.Rows[e.RowIndex];
                int clientId = Convert.ToInt32(row.Cells["ID"].Value);
                LoadAccounts(clientId);
            }
        }

        private void LoadAccounts(int clientId)
        {
            try
            {
                string query = "SELECT AccountNumber, Balance FROM Accounts WHERE ClientID = @ClientID";
                NpgsqlDataAdapter adapter = new NpgsqlDataAdapter(query, connection);
                adapter.SelectCommand.Parameters.AddWithValue("@ClientID", clientId);
                DataTable accountsTable = new DataTable();
                adapter.Fill(accountsTable);
                dataGridViewAccounts.DataSource = accountsTable;

                if (accountsTable.Rows.Count == 0)
                {
                    MessageBox.Show("Нет данных в таблице Accounts для выбранного клиента", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных счетов: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dataGridViewAccounts_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridViewAccounts.Rows[e.RowIndex];
                int accountNumber = Convert.ToInt32(row.Cells["AccountNumber"].Value);
                LoadTransactions(accountNumber);
            }
        }

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
                    ORDER BY TransactionDate DESC";
                NpgsqlDataAdapter adapter = new NpgsqlDataAdapter(query, connection);
                adapter.SelectCommand.Parameters.AddWithValue("@AccountNumber", accountNumber);
                DataTable transactionsTable = new DataTable();
                adapter.Fill(transactionsTable);
                dataGridViewAccountHistory.DataSource = transactionsTable;

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

        private void buttonTransfer_Click(object sender, EventArgs e)
        {
            using (TransactionForm transactionForm = new TransactionForm(connection))
            {
                transactionForm.ShowDialog();
            }
        }

        private void buttonExport_Click(object sender, EventArgs e)
        {
            ExportToXml();
        }

        private void buttonImport_Click(object sender, EventArgs e)
        {
            ImportFromXml();
        }

        private void ExportToXml()
        {
            try
            {
                DataSet dataSet = new DataSet("NewDataSet");
                string clientsQuery = "SELECT * FROM Clients";
                string accountsQuery = "SELECT * FROM Accounts";

                NpgsqlDataAdapter clientsAdapter = new NpgsqlDataAdapter(clientsQuery, connection);
                NpgsqlDataAdapter accountsAdapter = new NpgsqlDataAdapter(accountsQuery, connection);

                DataTable clientsTable = new DataTable("Clients");
                DataTable accountsTable = new DataTable("Accounts");

                clientsAdapter.Fill(clientsTable);
                accountsAdapter.Fill(accountsTable);

                dataSet.Tables.Add(clientsTable);
                dataSet.Tables.Add(accountsTable);

                // Формируем название файла с текущей датой и временем
                string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmssfff");
                string fileName = $"EnergoLIST_{timestamp}.xml";

                // Сохраняем данные в XML файл с применением схемы
                XmlWriterSettings settings = new XmlWriterSettings
                {
                    Indent = true,
                    IndentChars = "  ",
                    NewLineChars = Environment.NewLine,
                    Encoding = Encoding.UTF8
                };

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

﻿using System;
using System.Data;
using System.Windows.Forms;
using Npgsql;

namespace Energo
{
    public partial class TransactionForm : Form
    {
        private NpgsqlConnection connection;

        // Конструктор формы транзакции
        public TransactionForm(NpgsqlConnection connection)
        {
            InitializeComponent();
            this.connection = connection;
            LoadClients(); // Загрузка списка клиентов при инициализации формы
        }

        // Метод для загрузки списка клиентов
        private void LoadClients()
        {
            string query = "SELECT ID, Name FROM Clients";
            NpgsqlDataAdapter adapter = new NpgsqlDataAdapter(query, connection);
            DataTable clientsTable = new DataTable();
            adapter.Fill(clientsTable);

            // Настройка данных для ComboBox отправителя
            comboBoxSenderClient.DataSource = clientsTable.Copy();
            comboBoxSenderClient.DisplayMember = "Name";
            comboBoxSenderClient.ValueMember = "ID";
            comboBoxSenderClient.SelectedIndex = -1;

            // Настройка данных для ComboBox получателя
            comboBoxRecipientClient.DataSource = clientsTable.Copy();
            comboBoxRecipientClient.DisplayMember = "Name";
            comboBoxRecipientClient.ValueMember = "ID";
            comboBoxRecipientClient.SelectedIndex = -1;

            // Подписка на события изменения выбора
            comboBoxSenderClient.SelectedIndexChanged += comboBoxSenderClient_SelectedIndexChanged;
            comboBoxRecipientClient.SelectedIndexChanged += comboBoxRecipientClient_SelectedIndexChanged;
            comboBoxSenderAccount.SelectedIndexChanged += comboBoxSenderAccount_SelectedIndexChanged;
            comboBoxRecipientAccount.SelectedIndexChanged += comboBoxRecipientAccount_SelectedIndexChanged;
        }

        // Обработчик события изменения выбора клиента-отправителя
        private void comboBoxSenderClient_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxSenderClient.SelectedIndex != -1)
            {
                LoadAccounts(comboBoxSenderClient, comboBoxSenderAccount);
                labelSenderBalance.Text = "Баланс:";
            }
        }

        // Обработчик события изменения выбора клиента-получателя
        private void comboBoxRecipientClient_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxRecipientClient.SelectedIndex != -1)
            {
                LoadAccounts(comboBoxRecipientClient, comboBoxRecipientAccount);
                labelRecipientBalance.Text = "Баланс:";
            }
        }

        // Метод для загрузки счетов выбранного клиента
        private void LoadAccounts(ComboBox clientComboBox, ComboBox accountComboBox)
        {
            if (clientComboBox.SelectedValue is DataRowView)
                return;

            int clientId;
            if (int.TryParse(clientComboBox.SelectedValue.ToString(), out clientId))
            {
                string query = "SELECT AccountNumber FROM Accounts WHERE ClientID = @ClientID";
                NpgsqlDataAdapter adapter = new NpgsqlDataAdapter(query, connection);
                adapter.SelectCommand.Parameters.AddWithValue("@ClientID", clientId);
                DataTable accountsTable = new DataTable();
                adapter.Fill(accountsTable);

                accountComboBox.DataSource = accountsTable;
                accountComboBox.DisplayMember = "AccountNumber";
                accountComboBox.ValueMember = "AccountNumber";
                accountComboBox.SelectedIndex = -1;
            }
        }

        // Обработчик события изменения выбора счета отправителя
        private void comboBoxSenderAccount_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxSenderAccount.SelectedIndex != -1)
            {
                UpdateBalance(comboBoxSenderAccount, labelSenderBalance);
            }
            else
            {
                labelSenderBalance.Text = "Баланс:";
            }
        }

        // Обработчик события изменения выбора счета получателя
        private void comboBoxRecipientAccount_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxRecipientAccount.SelectedIndex != -1)
            {
                UpdateBalance(comboBoxRecipientAccount, labelRecipientBalance);
            }
            else
            {
                labelRecipientBalance.Text = "Баланс:";
            }
        }

        // Метод для обновления баланса выбранного счета
        private void UpdateBalance(ComboBox accountComboBox, Label balanceLabel)
        {
            if (accountComboBox.SelectedValue != null)
            {
                int accountNumber;
                if (int.TryParse(accountComboBox.SelectedValue.ToString(), out accountNumber))
                {
                    string query = "SELECT Balance FROM Accounts WHERE AccountNumber = @AccountNumber";
                    using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@AccountNumber", accountNumber);
                        connection.Open();
                        try
                        {
                            object result = command.ExecuteScalar();
                            if (result != null)
                            {
                                balanceLabel.Text = "Баланс: " + result.ToString();
                            }
                            else
                            {
                                balanceLabel.Text = "Баланс:";
                            }
                        }
                        finally
                        {
                            connection.Close();
                        }
                    }
                }
            }
        }

        // Обработчик события нажатия кнопки для выполнения транзакции
        private void buttonSubmit_Click(object sender, EventArgs e)
        {
            int fromAccount;
            int toAccount;
            decimal amount;

            // Проверяем, что выбраны счета отправителя и получателя и что введена сумма перевода
            if (comboBoxSenderAccount.SelectedIndex != -1 && comboBoxRecipientAccount.SelectedIndex != -1 &&
                int.TryParse(comboBoxSenderAccount.SelectedValue.ToString(), out fromAccount) &&
                int.TryParse(comboBoxRecipientAccount.SelectedValue.ToString(), out toAccount) &&
                decimal.TryParse(textBoxAmount.Text, out amount))
            {
                // Проверка на одинаковые счета
                if (fromAccount == toAccount)
                {
                    MessageBox.Show("Нельзя перевести на тот же счёт");
                    return;
                }

                // Проверка, что сумма перевода больше нуля
                if (amount <= 0)
                {
                    MessageBox.Show("Сумма перевода должна быть больше нуля");
                    return;
                }

                // Проверка на минимальную сумму перевода
                if (amount < 0.01m)
                {
                    MessageBox.Show("Минимальная сумма перевода: 0,01");
                    return;
                }

                // Проверка наличия достаточного баланса на счёте отправителя
                decimal senderBalance = GetAccountBalance(fromAccount);
                if (senderBalance < amount)
                {
                    MessageBox.Show("Недостаточно средств на счёте отправителя");
                    return;
                }

                // Открываем соединение и начинаем транзакцию
                try
                {
                    connection.Open();

                    using (NpgsqlTransaction transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            // Использование хранимой процедуры для выполнения транзакции
                            string transferQuery = "SELECT transfer_amount(@FromAccount, @ToAccount, @Amount)";
                            using (NpgsqlCommand command = new NpgsqlCommand(transferQuery, connection))
                            {
                                command.Parameters.AddWithValue("@FromAccount", fromAccount);
                                command.Parameters.AddWithValue("@ToAccount", toAccount);
                                command.Parameters.AddWithValue("@Amount", amount);
                                command.ExecuteNonQuery();
                            }

                            // Фиксируем транзакцию
                            transaction.Commit();
                            MessageBox.Show("Транзакция успешно выполнена");
                        }
                        catch (Exception ex)
                        {
                            // Откатываем транзакцию в случае ошибки
                            transaction.Rollback();
                            MessageBox.Show("Ошибка выполнения транзакции: " + ex.Message);
                        }
                        finally
                        {
                            connection.Close();
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка соединения с базой данных: " + ex.Message);
                }

                // Обновляем баланс отправителя и получателя после транзакции
                UpdateBalance(comboBoxSenderAccount, labelSenderBalance);
                UpdateBalance(comboBoxRecipientAccount, labelRecipientBalance);
            }
            else
            {
                MessageBox.Show("Ошибка в данных. Пожалуйста, проверьте вводимые значения.");
            }
        }

        // Метод для получения баланса счета
        private decimal GetAccountBalance(int accountNumber)
        {
            string query = "SELECT Balance FROM Accounts WHERE AccountNumber = @AccountNumber";
            using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@AccountNumber", accountNumber);
                connection.Open();
                try
                {
                    object result = command.ExecuteScalar();
                    if (result != null && result != DBNull.Value)
                    {
                        return Convert.ToDecimal(result);
                    }
                    else
                    {
                        throw new Exception("Ошибка получения баланса счета");
                    }
                }
                finally
                {
                    connection.Close();
                }
            }
        }
    }
}

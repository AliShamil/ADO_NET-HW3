using Microsoft.Extensions.Configuration;
using System;
using System.Data.Common;
using System.Data.SqlClient;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ADO_NET_HW3;

public partial class MainWindow : Window
{
    IConfigurationRoot configuration = null;
    DbConnection connection = null;
    DbDataAdapter adapter = null;
    DbProviderFactory providerFactory = null;
    DataSet dataSet = null;
    string providerName = string.Empty;

    public MainWindow()
    {
        InitializeComponent();
        providerName = "System.Data.SqlClient";
        DbProviderFactories.RegisterFactory(providerName, typeof(SqlClientFactory));

        providerFactory = DbProviderFactories.GetFactory(providerName);

        configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

        connection = providerFactory.CreateConnection();
        connection.ConnectionString = configuration.GetConnectionString(providerName);
        adapter = providerFactory.CreateDataAdapter();
        
        dataSet = new DataSet();

    }

    private void Btn_Exec_Click(object sender, RoutedEventArgs e)
    {
        var cmd = providerFactory.CreateCommand();

        cmd.CommandText = Txt_Query.Text;
        cmd.Connection = connection;
        adapter.SelectCommand = cmd;


        if (tb_control.Items.Count > 1)
        {
            for (int i = tb_control.Items.Count - 1; i > 0; i--)
                tb_control.Items.RemoveAt(i);
        }

        dataSet.Tables.Clear();

        try
        {
            adapter.Fill(dataSet);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }

        foreach (DataTable table in dataSet.Tables)
        {
            var item = new TabItem();

            item.Header = table.TableName;

            var dataGrid = new DataGrid();
            dataGrid.ItemsSource = table.AsDataView();
            dataGrid.IsReadOnly = true;
            item.Content = dataGrid;

            tb_control.Items.Add(item);
            ++tb_control.SelectedIndex;
        }
    }

    private void Window_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
            Btn_Exec_Click(sender, e);
    }
}

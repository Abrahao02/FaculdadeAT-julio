using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

class Cliente
{
    public string CPF { get; set; }
    public string Nome { get; set; }
    public decimal SaldoContaCorrente { get; set; }
    public decimal SaldoContaInternacional { get; set; }
    public decimal SaldoContaCripto { get; set; }
    public decimal TaxaCambio { get; set; }
}

class SistemaTarifas
{
    public event Action<string, string, decimal, decimal, decimal> ArquivoGerado; // Adicione o nome do cliente como parâmetro

    public void CalcularSomatorios(List<Cliente> clientes, Action<string> callback)
    {
        foreach (var cliente in clientes)
        {
            decimal totalSaldoEmReais = cliente.SaldoContaCorrente + (cliente.SaldoContaInternacional * cliente.TaxaCambio) + (cliente.SaldoContaCripto * cliente.TaxaCambio);
            decimal totalTarifaEmReais = (cliente.SaldoContaCorrente * 0.015m) + ((cliente.SaldoContaInternacional * cliente.TaxaCambio) * 0.025m);

            string cpf = cliente.CPF;
            string nome = cliente.Nome; // Obtenha o nome do cliente

            callback(cpf);

            ArquivoGerado?.Invoke(cpf, nome, totalSaldoEmReais, totalTarifaEmReais, cliente.TaxaCambio); // Inclua o nome do cliente no evento
        }
    }
}

class Program
{
    static void Main()
    {
        List<Cliente> clientes = LerArquivoClientes("clientes.txt");

        SistemaTarifas sistema = new SistemaTarifas();

        sistema.ArquivoGerado += (cpf, nome, saldo, tarifa, taxaCambioCliente) =>
        {
            string nomeArquivo = $"{cpf}.txt";
            EscreverArquivoCliente(nomeArquivo, nome, saldo, tarifa, taxaCambioCliente);
            Console.WriteLine($"Arquivo gerado para o CPF {cpf}");
        };

        sistema.CalcularSomatorios(clientes, cpf => Console.WriteLine($"Calculando para o CPF {cpf}"));

        Console.WriteLine("Processo concluído. Pressione qualquer tecla para sair.");
        Console.ReadKey();
    }

    static List<Cliente> LerArquivoClientes(string nomeArquivo)
    {
        List<Cliente> clientes = new List<Cliente>();

        using (StreamReader reader = new StreamReader(nomeArquivo))
        {
            while (!reader.EndOfStream)
            {
                string[] linha = reader.ReadLine().Split('|');
                Cliente cliente = new Cliente
                {
                    CPF = linha[0],
                    Nome = linha[1],
                    SaldoContaCorrente = decimal.Parse(linha[2], CultureInfo.InvariantCulture),
                    SaldoContaInternacional = decimal.Parse(linha[3], CultureInfo.InvariantCulture),
                    SaldoContaCripto = decimal.Parse(linha[4], CultureInfo.InvariantCulture),
                    TaxaCambio = 5.50m // Defina a taxa de câmbio adequada aqui
                };
                clientes.Add(cliente);
            }
        }

        return clientes;
    }

    static void EscreverArquivoCliente(string nomeArquivo, string nome, decimal saldo, decimal tarifa, decimal taxaCambioCliente)
    {
        using (StreamWriter writer = new StreamWriter(nomeArquivo))
        {
            writer.WriteLine($"Nome do Cliente: {nome}");
            writer.WriteLine($"Saldo em Reais: {saldo:F2}");
            writer.WriteLine($"Tarifa em Reais: {tarifa:F2}");
            writer.WriteLine($"Taxa de Câmbio: {taxaCambioCliente:F2}");
        }
    }
}

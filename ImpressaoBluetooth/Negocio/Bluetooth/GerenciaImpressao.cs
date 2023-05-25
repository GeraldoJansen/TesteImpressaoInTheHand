using ImpressaoBluetooth.Interface.Impressao;
using InTheHand.Net.Bluetooth;
using InTheHand.Net.Sockets;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using static ImpressaoBluetooth.Util.EnumeradoresU;

namespace ImpressaoBluetooth.Negocio.Impressao.Bluetooth
{
    public class GerenciaImpressao
    {
        // Github do Nuget InTheHand.Net.Bluetooth:
        // https://github.com/inthehand/32feet
        public static BluetoothClient BluetoothCliente { get; set; }
        public static List<BluetoothDeviceInfo> Devices { get; set; }
        public static Impressora TipoImpressora { get; set; } = Impressora.i80mm;
        public static string NomeImpressora { get; set; }
        private static Stream Stream { get; set; } = null;
        private static bool DispositivosProcurados { get; set; } = false;

        public static async Task<List<BluetoothDeviceInfo>> ListarDispositivosAsync()
        {
            try
            {
                DispositivosProcurados = false;

                await VerificarBluetoothLigadoAsync();

                if (BluetoothCliente == null)
                    BluetoothCliente = new BluetoothClient();

                //BluetoothClient bluetoothClient = new BluetoothClient();
                Devices = await Task.Run(() => BluetoothCliente.DiscoverDevices()?.ToList());

                // Verificar se algum dispositivo foi encontrado
                if (Devices.Count == 0)
                    throw new Exception("Não foi possível encontrar impressoras bluetooth");

                DispositivosProcurados = true;

                return Devices;
            }
            catch (Exception erro)
            {
                throw erro;
            }
        }

        public static async Task<List<BluetoothDeviceInfo>> ListarDispositivosPareadosAsync()
        {
            try
            {
                DispositivosProcurados = false;

                await VerificarBluetoothLigadoAsync();

                if (BluetoothCliente == null)
                    BluetoothCliente = new BluetoothClient();

                //BluetoothClient bluetoothClient = new BluetoothClient();
                //var devices = await Task.Run(() => bluetoothClient.DiscoverDevices()?.ToList());
                //devices = bluetoothClient.PairedDevices.ToList();

                Devices = BluetoothCliente.PairedDevices.ToList();

                // Verificar se algum dispositivo foi encontrado
                if (Devices.Count == 0)
                    throw new Exception("Não foi possível encontrar impressoras bluetooth pareadas");

                DispositivosProcurados = true;

                await Task.Delay(0);

                return Devices;
            }
            catch (Exception erro)
            {
                throw erro;
            }
        }

        public static async Task<bool> ConectarAsync(string nomeImpressora, Impressora tipoImpressora)
        {
            bool encontrou = false;

            try
            {
                NomeImpressora = nomeImpressora;
                TipoImpressora = tipoImpressora;

                if (await ConectadoAsync())
                    await FecharAsync();

                if (!DispositivosProcurados)
                    await ListarDispositivosPareadosAsync();
                else
                    await VerificarBluetoothLigadoAsync();

                if (Devices == null)
                    Devices = new List<BluetoothDeviceInfo>();

                if (Devices.Count == 0)
                    Devices = await ListarDispositivosPareadosAsync();

                var dispositivos = await Task.Run(() => Devices.Where(x => $"{x.DeviceName}".ToUpper().Equals(nomeImpressora.ToUpper())).FirstOrDefault());

                if (dispositivos == null)
                    throw new Exception("Nenhuma impressora encontrada para conectar");

                if (BluetoothCliente == null)
                    BluetoothCliente = new BluetoothClient();

                // Conecta-se à impressora
                await Task.Run(() => BluetoothCliente.Connect(dispositivos.DeviceAddress, BluetoothService.SerialPort));

                // Aguarda um breve momento para garantir a conexão
                await Task.Delay(1000);

                if (!BluetoothCliente.Connected)
                    throw new Exception("Não foi possível conectar com a impressora");

                Stream = await Task.Run(() => BluetoothCliente.GetStream());

                if (Stream == null)
                    throw new Exception("Não foi possível conectar com a impressora");

                encontrou = true;
            }
            catch (Exception erro)
            {
                //if (erro.Message.ToUpper().Contains("gattcallback error".ToUpper()))
                //    throw new Exception($"Não foi possível estabelecer conexão com o aparelho. {erro.Message}");
                //else
                throw erro;
            }

            return encontrou;
        }

        public static async Task<bool> EscreverAsync(byte[] dados)
        {
            bool resposta = false;

            try
            {
                if (dados == null)
                    return resposta;

                await VerificarBluetoothLigadoAsync();
                await VerificarSeDispositivoConectadoAsync(false);

                if (!BluetoothCliente.Connected)
                    await ConectarAsync(NomeImpressora, TipoImpressora);

                if (Stream == null)
                    throw new Exception("Não foi possível enviar dados para a impressora. A conexão não foi aberta");

                var partesDadosBytes = await QuebrarBytesEmBlocosAsync(dados, 1000);

                foreach (byte[] dadoBytes in partesDadosBytes)
                {
                    await EscreverBytesAsync(dadoBytes, 0, dadoBytes.Length);
                    await Task.Delay(7);
                }
            }
            catch (Exception erro)
            {
                throw erro;
            }

            return resposta;
        }

        public static async Task<bool> EscreverAsync(byte[] buffer, int offset, int count)
        {
            bool resposta = false;

            try
            {
                if (buffer == null)
                    return resposta;

                await VerificarBluetoothLigadoAsync();
                await VerificarSeDispositivoConectadoAsync(false);

                if (!BluetoothCliente.Connected)
                    await ConectarAsync(NomeImpressora, TipoImpressora);

                if (Stream == null)
                    throw new Exception("Não foi possível enviar dados para a impressora. A conexão não foi aberta");

                await EscreverBytesAsync(buffer, offset, count);
            }
            catch (Exception erro)
            {
                throw erro;
            }

            return resposta;
        }

        public static async Task<bool> EscreverAsync(string texto)
        {
            bool resposta = false;

            try
            {
                if (string.IsNullOrEmpty(texto))
                    return resposta;

                var dados = Encoding.UTF8.GetBytes(texto);
                await EscreverAsync(dados);
            }
            catch (Exception erro)
            {
                throw erro;
            }

            return resposta;
        }

        private static async Task EscreverBytesAsync(byte[] buffer, int offset, int count)
        {
            await Task.Run(() => Stream.Write(buffer, offset, count));
        }

        public static async Task FecharAsync()
        {
            try
            {
                await VerificarBluetoothLigadoAsync();
                await VerificarSeDispositivoConectadoAsync();

                await Task.Run(() => BluetoothCliente.Close());

                await Task.Delay(1000);
            }
            catch (Exception erro)
            {
                //throw erro;
            }
        }

        public static async Task<bool> ConectadoAsync()
        {
            bool resposta = false;

            try
            {
                await VerificarBluetoothLigadoAsync();
                await VerificarSeDispositivoConectadoAsync();

                resposta = true;
            }
            catch (Exception erro)
            {
                //throw erro;
            }

            return resposta;
        }

        public static async Task VerificarBluetoothLigadoAsync()
        {
            if (!await DependencyService.Get<IBluetoothServices>().BluetoothLigadoAsync())
                throw new Exception("Bluetooth não conectado");
        }

        public static async Task<bool> VerificarSeDispositivoConectadoAsync(bool verificarConexao = true)
        {
            bool resposta = false;
            string descricao = "Nenhuma impressora conectada";
            bool falha = false;

            if (BluetoothCliente == null)
                falha = true;
            else if (BluetoothCliente.PairedDevices == null)
                falha = true;
            else if (BluetoothCliente.PairedDevices.Count() == 0)
                falha = true;
            else if (Stream == null)
                falha = true;
            else if (!BluetoothCliente.Connected && verificarConexao)
                falha = true;
            else if (!DispositivosProcurados)
                falha = true;

            if (falha)
                throw new Exception(descricao);

            resposta = true;

            await Task.Delay(0);

            return resposta;
        }

        private static async Task<List<byte[]>> QuebrarBytesEmBlocosAsync(byte[] dados, int chunkSize)
        {
            List<byte[]> chunks = new List<byte[]>();

            int start = 0;
            while (start < dados.Length)
            {
                int chunkLength = Math.Min(chunkSize, dados.Length - start);
                byte[] chunk = new byte[chunkLength];
                Array.Copy(dados, start, chunk, 0, chunkLength);
                chunks.Add(chunk);
                start += chunkSize;
            }

            await Task.Delay(0);

            return chunks;
        }
    }
}

using ImpressaoBluetooth.Negocio.Impressao.Bluetooth;
using ImpressaoBluetooth.Util;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ImpressaoBluetooth.Util.EnumeradoresU;

namespace ImpressaoBluetooth.Negocio.Impressao.EscPos
{
    public class ImpressaoEscPos
    {
        private static Impressora Impressora { get; set; } = Impressora.i80mm;
        /// <summary>
        /// Imprimi um texto na impressora térmica
        /// </summary>
        /// <param name="texto">Texto que será impresso</param>
        /// <returns></returns>
        public static async Task ImprimirAsync(string texto)
        {
            try
            {
                await VerificarBluetooth();

                // Iniciando um tipo de espaçamento
                await FonteNormal();
                await EspacoPadrao();
                await Esquerda();
                await Escrever(texto);
                await Encerrar();
            }
            catch (Exception erro)
            {
                throw erro;
            }
        }

        public static async Task EncerrarAsync() => await Encerrar();

        public static async Task<int> ImprimirLinhaAsync()
        {
            int ret = -1;

            try
            {
                await VerificarBluetooth();

                await FonteNormal();
                ret = await Traco();
                await Encerrar();
            }
            catch (Exception erro)
            {
                throw erro;
            }

            return ret;
        }

        public static async Task<int> ImprimirQuebraAsync(int quantidadeQuebra)
        {
            int ret = -1;

            try
            {
                await VerificarBluetooth();

                await FonteNormal();
                ret = await Quebra(quantidadeQuebra);
                await Encerrar();
            }
            catch (Exception erro)
            {
                throw erro;
            }

            return ret;
        }

        #region Métodos de apoio para executar na impressora

        private static async Task VerificarBluetooth()
        {
            if (GerenciaImpressao.BluetoothCliente == null)
                throw new Exception("A conexão precisa ser aberta para iniciar o uso da impressora");

            if (!await GerenciaImpressao.ConectadoAsync())
                throw new Exception("Não foi possível conectar através do bluetooth");

            await Task.Delay(0);
        }

        private static async Task<int> FonteNormal()
        {
            int ret = -1;

            try
            {
                string comando = (((char)0x1B) + string.Empty) + (((char)0x21) + string.Empty) + (((char)0x00) + string.Empty);
                await GerenciaImpressao.EscreverAsync(comando);
                ret = 1;
            }
            catch (Exception erro)
            {
                throw erro;
            }

            return ret;
        }

        private static async Task<int> EspacoPadrao()
        {
            int ret = -1;

            try
            {
                string comando = (((char)0x1B) + string.Empty) + (((char)0x32) + string.Empty) + (((char)0x0D) + string.Empty);

                await GerenciaImpressao.EscreverAsync(comando);
                ret = 1;
            }
            catch (Exception erro)
            {
                throw erro;
            }

            return ret;
        }

        private static async Task<int> Escrever(string texto)
        {
            int ret = -1;

            try
            {
                await GerenciaImpressao.EscreverAsync(texto);
                ret = 1;
            }
            catch (Exception erro)
            {
                throw erro;
            }

            return ret;
        }

        private static async Task<int> Escrever(byte[] buffer, int offset, int count)
        {
            int ret = -1;

            try
            {
                await GerenciaImpressao.EscreverAsync(buffer, offset, count);

                ret = 1;
            }
            catch (Exception erro)
            {
                throw erro;
            }

            return ret;
        }

        private static async Task<int> Esquerda()
        {
            int ret = -1;

            try
            {
                string comando = (((char)0x1B) + string.Empty) + (((char)0x61) + string.Empty) + (((char)0x00) + string.Empty);

                await GerenciaImpressao.EscreverAsync(comando);
                ret = 1;
                //return await DependencyService.Get<IImpressao>().EnviarComando(comando);
            }
            catch (Exception erro)
            {
                throw erro;
            }

            return ret;
        }

        private static async Task<int> Encerrar()
        {
            int ret = -1;

            try
            {
                string comando = "\r\n";//((char)0x0A) + string.Empty;

                await GerenciaImpressao.EscreverAsync(comando);
                ret = 1;
            }
            catch (Exception erro)
            {
                throw erro;
            }

            return ret;
        }

        private static async Task<int> Quebra(int qtdeQuebras)
        {
            int ret = -1;

            try
            {
                string comando = string.Empty;

                for (int i = 0; i < qtdeQuebras; i++)
                    comando += ((char)0x0A);

                await GerenciaImpressao.EscreverAsync(comando);
                ret = 1;
            }
            catch (Exception erro)
            {
                throw erro;
            }

            return ret;
        }

        private static async Task<int> Espaco(int qtdeQuebras)
        {
            int ret = -1;

            try
            {
                string comando = string.Empty;

                for (int i = 1; i <= qtdeQuebras; i++)
                    comando += " ";

                if (!string.IsNullOrEmpty(comando))
                {
                    await GerenciaImpressao.EscreverAsync(comando);
                    ret = 1;
                }
            }
            catch (Exception erro)
            {
                throw erro;
            }

            return ret;
        }

        private static async Task<int> Traco()
        {
            int ret = -1;

            try
            {
                string comando = string.Empty;

                if (Impressora == Impressora.i80mm)
                    comando = "________________________________________________\n";
                else
                    comando = "________________________________\n";

                await GerenciaImpressao.EscreverAsync(comando);

                ret = 1;
            }
            catch (Exception erro)
            {
                throw erro;
            }

            return ret;
        }

        #endregion;
    }
}

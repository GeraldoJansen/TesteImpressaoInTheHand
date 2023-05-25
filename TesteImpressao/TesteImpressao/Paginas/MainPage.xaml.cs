using ImpressaoBluetooth.Negocio.Impressao.Bluetooth;
using ImpressaoBluetooth.Negocio.Impressao.EscPos;
using InTheHand.Net.Sockets;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Xamarin.Forms;
using static ImpressaoBluetooth.Util.EnumeradoresU;

namespace TesteImpressao
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();

            try
            {
                var dispositivos = GerenciaImpressao.ListarDispositivosPareadosAsync().Result;

                ListarDispositivos(dispositivos);
            }
            catch (Exception erro)
            {
                DisplayAlert("Notice", $"{erro.Message}", "OK");
            }
        }

        private async void btnAtualizarDispositivos_Clicked(object sender, EventArgs e)
        {
            try
            {
                var dispositivos = await GerenciaImpressao.ListarDispositivosPareadosAsync();

                ListarDispositivos(dispositivos);
            }
            catch (Exception erro)
            {
                await DisplayAlert("Notice", $"{erro.Message}", "OK");
            }
        }

        private void ListarDispositivos(List<BluetoothDeviceInfo> dispositivos)
        {
            pDispositivos.Items.Clear();
            foreach (var d in dispositivos.Where(x => !string.IsNullOrEmpty(x.DeviceName)).ToList())
                pDispositivos.Items.Add(d.DeviceName);
        }

        private async void btnImprimirTextoGrande_Clicked(object sender, EventArgs e)
        {
            try
            {
                if (pDispositivos.SelectedItem == null)
                {
                    await DisplayAlert("Atenção", "A impressora deve ser informada", "Ok");
                    pDispositivos.Focus();
                    return;
                }

                string impressora = pDispositivos.Items[pDispositivos.SelectedIndex];
                Impressora tipoImpressora = Impressora.i80mm;

                await GerenciaImpressao.ConectarAsync(impressora, tipoImpressora);

                string textoTestes = "Foi efetuado um sorteio para ver quem escolheria em primeiro lugar um desses caminhos. O primeiro sorteado escolheu, naturalmente, o Primeiro caminho. O segundo sorteado escolheu o Segundo caminho. O terceiro sorteado, sem nenhuma outra opção, aceitou o Terceiro caminho.\n\nEles partiram juntos, no mesmo horário, levando consigo apenas uma mochila contendo alimentos, agasalhos e algumas ferramentas.";

                await ImpressaoEscPos.ImprimirAsync(textoTestes);
                await ImpressaoEscPos.EncerrarAsync();
                await GerenciaImpressao.FecharAsync();
            }
            catch (Exception erro)
            {
                await DisplayAlert("Ops", erro.Message, "Ok");
            }
        }
    }
}

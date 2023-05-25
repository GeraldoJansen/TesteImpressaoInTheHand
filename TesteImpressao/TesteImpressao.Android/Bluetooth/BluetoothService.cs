using Android.Bluetooth;
using ImpressaoBluetooth.Interface.Impressao;
using System;
using System.Threading.Tasks;
using TesteImpressao.Droid.Bluetooth;
using Xamarin.Forms;

[assembly: Dependency(typeof(BluetoothService))]
namespace TesteImpressao.Droid.Bluetooth
{
    public class BluetoothService : IBluetoothServices
    {
        public async Task<bool> BluetoothLigadoAsync()
        {
            bool bluetoothLigado = false;

            try
            {
                var adapter = BluetoothAdapter.DefaultAdapter;

                if (adapter != null)
                {
                    if (adapter.IsEnabled && adapter.State == State.On)
                        bluetoothLigado = true;
                }

                await Task.Delay(0);
            }
            catch (Exception erro)
            {
                //throw erro;
            }

            return bluetoothLigado;
        }
    }
}
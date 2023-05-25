using System.Threading.Tasks;

namespace ImpressaoBluetooth.Interface.Impressao
{
    public interface IBluetoothServices
    {
        Task<bool> BluetoothLigadoAsync();
    }
}

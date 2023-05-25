using System.ComponentModel;
using System.Xml.Serialization;

namespace ImpressaoBluetooth.Util
{
    public class EnumeradoresU
    {
        public enum TipoAmbiente
        {
            [XmlEnum("1")]
            [Description("Produção")]
            Producao = 1,

            [XmlEnum("2")]
            [Description("Homologação")]
            Homologacao = 2
        }

        public enum TipoComunicacao
        {
            [XmlEnum("1")]
            [Description("Emissão")]
            Emissao = 1,
            
            [XmlEnum("2")]
            [Description("Consulta")]
            Consulta = 2,
            
            [XmlEnum("3")]
            [Description("Cancelamento")]
            Cancelamento = 3,

            [XmlEnum("4")]
            [Description("Inutilização")]
            Inutilizacao = 4,

            [XmlEnum("5")]
            [Description("Nenhuma")]
            Nenhuma = 5
        }

        public enum TipoConversao
        {
            [XmlEnum("1")]
            [Description("Inteiro")]
            Inteiro,

            [XmlEnum("2")]
            [Description("InteiroLongo")]
            InteiroLongo,

            [XmlEnum("3")]
            [Description("PontoFlutuante")]
            PontoFlutuante,

            [XmlEnum("4")]
            [Description("PontoFlutuanteLongo")]
            PontoFlutuanteLongo,

            [XmlEnum("5")]
            [Description("Decimal")]
            Decimal
        }

        public enum NivelRetorno
        {
            [XmlEnum("0")]
            [Description("MontandoTags")]
            MontandoTags,
            
            [XmlEnum("1")]
            [Description("Comunicacao")]
            Comunicacao,

            [XmlEnum("2")]
            [Description("PortalInvoicy")]
            PortalInvoicy,

            [XmlEnum("3")]
            [Description("Sefaz")]
            Sefaz,

            [XmlEnum("4")]
            [Description("ComunicacaoAnormalidade")]
            ComunicacaoAnormalidade
        }

        public enum Impressora
        {
            i58mm,
            i80mm
        }

        public enum QrCodeTamanho
        {
            ExtraGrande = 1,
            Grande = 2,
            Medio = 3,
            Pequeno = 4
        }
    }
}

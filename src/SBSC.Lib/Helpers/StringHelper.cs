using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SBSC.Lib.Helpers
{
    public class StringHelper
    {
        /// <summary>
        /// Remove todos caracteres, deixando apenas números.
        /// </summary>
        /// <param name="Value">Valor com a máscara.</param>
        /// <returns>Retorna apanas números em uma String.</returns>
        public static String RemoveMask(String value)
        {
            return string.IsNullOrEmpty(value) ? string.Empty : new string(value.Where(char.IsLetterOrDigit).ToArray());
        }

        /// <summary>
        /// Adiciona a máscara escolhida.
        /// </summary>
        /// <param name="Type">Tipo de máscara</param>
        /// <param name="Value">Valor que receberá a máscara.</param>
        /// <returns>Retorna o valor com a máscara escolhida aplicada.</returns>
        public static String AddMask(MaskType Type, String Value)
        {
            if (!string.IsNullOrEmpty(Value))
                switch (Type)
                {
                    case MaskType.CPF:
                        if (Value.Length == 11)
                            Value = string.Format("{0}.{1}.{2}-{3}", Value.Substring(0, 3), Value.Substring(3, 3), Value.Substring(6, 3), Value.Substring(9, 2));
                        break;
                    case MaskType.CNPJ:
                        if (Value.Length == 14)
                            Value = string.Format("{0}.{1}.{2}/{3}-{4}", Value.Substring(0, 2), Value.Substring(2, 3), Value.Substring(5, 3), Value.Substring(8, 4), Value.Substring(12, 2));
                        break;
                }

            return Value;
        }

        public enum MaskType
        {
            CPF,
            CNPJ
        }
    }
}

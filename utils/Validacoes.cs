namespace ClienteApi.utils
{
    public class Validacoes
    {
        public static bool CepValido(string cep)
        {
            if (string.IsNullOrEmpty(cep))
                return false;

            cep = cep.Replace("-", "").Trim();

            return cep.Length == 8 && cep.All(char.IsDigit);
        }
    }
}
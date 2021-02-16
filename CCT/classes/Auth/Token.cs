using System;

namespace CCT
{
    public class Token
    {
        /// <summary>
        /// JSON Web Token (JWT) contendo as informações do usuário. Conforme o padrão JWT, esse token poderá ser decodificado (Base64) a fim de se extrair as informações do usuário para que as mesmas sejam utilizadas na aplicação cliente. O token é assinado digitalmente pelo servidor e verificado a cada requisição, garantindo a sua inviolabilidade.
        /// </summary>
        public string SetToken { get; set; }

        /// <summary>
        /// Token de prevenção contra ataques CSRF (Cross-Site Request Forgery). Ao contrário do JWT, esse token é criptografado e pode ser decodificado apenas no servidor. Esse token possui um tempo de vida de 60 minutos. A cada nova requisição, o token é regerado pelo servidor a fim de atualizar o seu tempo de expiração.
        /// </summary>
        public string CsrfToken { get; set; }

        /// <summary>
        /// Data de expiração do X-CSRF-Token, em milisegundos. Após essa data, o token não será mais aceito no servidor.
        /// </summary>
        public string CsrfExpiration { get; set; }

        /// <summary>
        /// Verifica se o Token é válido
        /// </summary>
        /// <returns></returns>
        public bool Valido() => SetToken?.Length > 0 && CsrfToken?.Length > 0
                && TimeSpan.FromMilliseconds(Convert.ToDouble(CsrfExpiration ?? "0")).TotalMinutes > 0;
    }

}

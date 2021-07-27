namespace Security.Extensions
{
    public struct SecureString
    {
        public string Value;

        public SecureString(string text)
        {
            this = text;
        }

        public static implicit operator SecureString(string v)
        {
            return new SecureString()
            {
                Value = v.ToAes256Base64()
            };
        }

        public static implicit operator string(SecureString v)
        {
            return v.Value?.FromAes256Base64();
        }
    }
}

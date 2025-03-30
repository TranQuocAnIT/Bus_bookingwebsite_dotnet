namespace DOANCOSO26.Services
{
    public class InvoiceCodeGenerator
    {
        public string GenerateInvoiceCode()
        {
            return $"INV-{DateTime.UtcNow:yyyyMMddHHmmssfff}";
        }
    }

}
